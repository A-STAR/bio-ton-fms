import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';

import {
  MatLegacyDialog as MatDialog,
  MatLegacyDialogConfig as MatDialogConfig,
  MatLegacyDialogModule as MatDialogModule
} from '@angular/material/legacy-dialog';

import { MatLegacySnackBar as MatSnackBar } from '@angular/material/legacy-snack-bar';

import { BehaviorSubject, switchMap, Observable, tap, Subscription, filter, firstValueFrom } from 'rxjs';

import { NewVehicle, SortBy, SortDirection, Vehicle, Vehicles, VehicleService, VehiclesOptions } from '../vehicle.service';

import { VehicleDialogComponent } from '../vehicle-dialog/vehicle-dialog.component';

import { TableDataSource } from '../table.data-source';

@Component({
  selector: 'bio-vehicles',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatTableModule,
    MatSortModule,
    MatIconModule,
    MatDialogModule
  ],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class VehiclesComponent implements OnInit, OnDestroy {
  REGISTRATION_NUMBER_PATTERN = REGISTRATION_NUMBER_PATTERN;

  protected vehiclesData$!: Observable<Vehicles>;
  protected vehiclesDataSource!: TableDataSource<VehicleDataSource>;
  protected columns = columns;
  protected columnsKeys!: string[];
  protected VehicleColumn = VehicleColumn;

  /**
   * `sortChange` handler sorting vehicles.
   *
   * @param sort Sort state.
   */
  protected onSortChange({ active, direction }: Sort) {
    const vehiclesOptions: VehiclesOptions = {};

    if (active && direction) {
      switch (active) {
        case VehicleColumn.Name:
          vehiclesOptions.sortBy = SortBy.Name;

          break;

        case VehicleColumn.Type:
          vehiclesOptions.sortBy = SortBy.Type;

          break;

        case VehicleColumn.Subtype:
          vehiclesOptions.sortBy = SortBy.Subtype;

          break;

        case VehicleColumn.Group:
          vehiclesOptions.sortBy = SortBy.Group;

          break;

        case VehicleColumn.Fuel:
          vehiclesOptions.sortBy = SortBy.Fuel;
      }

      switch (direction) {
        case 'asc':
          vehiclesOptions.sortDirection = SortDirection.Acending;

          break;

        case 'desc':
          vehiclesOptions.sortDirection = SortDirection.Descending;
      }
    }

    this.#vehicles$.next(vehiclesOptions);
  }

  /**
   * Create a new vehicle in table.
   */
  protected onCreateVehicle() {
    this.#subscription?.unsubscribe();

    const dialogRef = this.dialog.open<VehicleDialogComponent, any, true | ''>(VehicleDialogComponent, dialogConfig);

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(() => {
        this.#updateVehicles();
      });
  }

  /**
   * Update a vehicle in table.
   *
   * @param vehicleDataSource Vehicle data source.
   */
  protected onUpdateVehicle({
    id,
    name,
    make,
    model,
    type,
    subtype,
    group,
    year,
    fuel,
    registration,
    inventory,
    serial,
    tracker,
    description
  }: VehicleDataSource) {
    const data: NewVehicle = {
      id,
      name,
      type: type.key,
      vehicleGroupId: group ? Number(group.key) : undefined,
      make,
      model,
      subType: subtype.key,
      fuelTypeId: Number(fuel.key),
      manufacturingYear: year,
      registrationNumber: registration,
      inventoryNumber: inventory,
      serialNumber: serial,
      trackerId: tracker ? Number(tracker?.key) : undefined,
      description
    };

    const dialogRef = this.dialog.open<VehicleDialogComponent, NewVehicle, true | ''>(VehicleDialogComponent, { ...dialogConfig, data });

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(() => {
        this.#updateVehicles();
      });
  }

  /**
   * Delete a vehicle in table.
   *
   * @param vehicleDataSource Vehicle data source.
   */
  protected async onDeleteVehicle({ id }: VehicleDataSource) {
    const deleteVehicle$ = this.vehicleService.deleteVehicle(id);

    await firstValueFrom(deleteVehicle$);

    this.snackBar.open(VEHICLE_DELETED);

    this.#updateVehicles();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  #vehicles$ = new BehaviorSubject<VehiclesOptions>({});
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #subscription: Subscription | undefined;

  /**
   * Emit vehicles update.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #updateVehicles() {
    this.#vehicles$.next(this.#vehicles$.value);
  }

  /**
   * Map vehicles data source.
   *
   * @param vehicles Vehicles with pagination.
   *
   * @returns Mapped vehicles data source.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #mapVehiclesDataSource({ vehicles }: Vehicles) {
    return Object
      .freeze(vehicles)
      .map(({
        id,
        name,
        type,
        vehicleGroup: group,
        make,
        model,
        subType: subtype,
        fuelType: fuel,
        manufacturingYear: year,
        registrationNumber: registration,
        inventoryNumber: inventory,
        serialNumber: serial,
        description,
        tracker
      }): VehicleDataSource => {
        return { id, name, make, model, type, subtype, group, year, fuel, registration, inventory, serial, tracker, description };
      });
  }

  /**
   * Initialize `TableDataSource` and set vehicles data source.
   *
   * @param vehicles Vehicles.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #setVehiclesDataSource(vehicles: Vehicles) {
    const vehiclesDataSource = this.#mapVehiclesDataSource(vehicles);

    if (this.vehiclesDataSource) {
      this.vehiclesDataSource.setDataSource(vehiclesDataSource);
    } else {
      this.vehiclesDataSource = new TableDataSource<VehicleDataSource>(vehiclesDataSource);
    }
  }

  /**
   * Get and set vehicles data.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #setVehiclesData() {
    this.vehiclesData$ = this.#vehicles$.pipe(
      switchMap(vehiclesOptions => this.vehicleService.getVehicles(vehiclesOptions)),
      tap(vehiclesData => {
        this.#setVehiclesDataSource(vehiclesData);
      })
    );
  }

  /**
   * Set column keys.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #setColumnKeys() {
    this.columnsKeys = this.columns.map(({ key }) => key);
  }

  constructor(private snackBar: MatSnackBar, private dialog: MatDialog, private vehicleService: VehicleService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setVehiclesData();
    this.#setColumnKeys();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

export enum VehicleColumn {
  Action = 'action',
  Name = 'name',
  Make = 'make',
  Model = 'model',
  Type = 'type',
  Subtype = 'subtype',
  Group = 'group',
  Year = 'year',
  Fuel = 'fuel',
  Registration = 'registration',
  Inventory = 'inventory',
  Serial = 'serial',
  Description = 'description',
  Tracker = 'tracker'
}

export interface VehicleDataSource extends Pick<Vehicle, 'id' | 'name' | 'make' | 'model' | 'type' | 'tracker' | 'description'> {
  subtype: Vehicle['subType'];
  group?: Vehicle['vehicleGroup'];
  year?: Vehicle['manufacturingYear'];
  fuel: Vehicle['fuelType'];
  registration?: Vehicle['registrationNumber'];
  inventory?: Vehicle['inventoryNumber'];
  serial?: Vehicle['serialNumber'];
}

export const columns: KeyValue<VehicleColumn, string | undefined>[] = [
  {
    key: VehicleColumn.Action,
    value: undefined
  },
  {
    key: VehicleColumn.Name,
    value: 'Наименование&#10;машины'
  },
  {
    key: VehicleColumn.Make,
    value: 'Производитель'
  },
  {
    key: VehicleColumn.Model,
    value: 'Модель'
  },
  {
    key: VehicleColumn.Type,
    value: 'Тип&#10;машин'
  },
  {
    key: VehicleColumn.Subtype,
    value: 'Подтип&#10;машины'
  },
  {
    key: VehicleColumn.Group,
    value: 'Группа&#10;машин'
  },
  {
    key: VehicleColumn.Year,
    value: 'Год произ&shy;водства'
  },
  {
    key: VehicleColumn.Fuel,
    value: 'Тип&#10;топлива'
  },
  {
    key: VehicleColumn.Registration,
    value: 'Регистра&shy;ционный&#10;номер'
  },
  {
    key: VehicleColumn.Tracker,
    value: 'GPS&#10;трекер'
  },
  {
    key: VehicleColumn.Description,
    value: 'Описание'
  }
];

const REGISTRATION_NUMBER_PATTERN = /[a-zA-Zа-яА-ЯёЁ]+|[0-9]+/g;
export const VEHICLE_DELETED = 'Машина удалена';

const dialogConfig: MatDialogConfig<NewVehicle> = {
  width: '70vw',
  height: '85vh'
};
