import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogConfig, MatDialogModule } from '@angular/material/dialog';

import { BehaviorSubject, switchMap, Observable, tap, Subscription, filter } from 'rxjs';

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
export class VehiclesComponent implements OnInit, OnDestroy {
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
  onCreateVehicle() {
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
  onUpdateVehicle({
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

    this.dialog.open<VehicleDialogComponent, NewVehicle, true | ''>(VehicleDialogComponent, { ...dialogConfig, data });
  }

  #vehicles$ = new BehaviorSubject<VehiclesOptions>({});
  #subscription: Subscription | undefined;

  /**
   * Emit vehicles update.
   */
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
  #setColumnKeys() {
    this.columnsKeys = this.columns.map(({ key }) => key);
  }

  constructor(private dialog: MatDialog, private vehicleService: VehicleService) { }

  ngOnInit() {
    this.#setVehiclesData();
    this.#setColumnKeys();
  }

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

const dialogConfig: MatDialogConfig<NewVehicle> = {
  width: '70vw',
  height: '85vh'
};
