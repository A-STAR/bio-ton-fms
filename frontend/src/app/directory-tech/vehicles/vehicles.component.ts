import { ChangeDetectionStrategy, Component, ErrorHandler, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { BehaviorSubject, switchMap, Observable, tap, Subscription, filter, mergeMap } from 'rxjs';

import { NewVehicle, VehiclesSortBy, Vehicle, Vehicles, VehicleService, VehiclesOptions } from '../vehicle.service';

import { TableActionsTriggerDirective } from '../shared/table-actions-trigger/table-actions-trigger.directive';
import { VehicleDialogComponent } from '../vehicle-dialog/vehicle-dialog.component';
import {
  TrackerCommandDialogComponent,
  trackerCommandDialogConfig
} from '../shared/tracker-command-dialog/tracker-command-dialog.component';

import {
  ConfirmationDialogData,
  ConfirmationDialogComponent,
  confirmationDialogConfig,
  getConfirmationDialogContent
} from '../../shared/confirmation-dialog/confirmation-dialog.component';

import { SortDirection } from '../shared/sort';

import { TableDataSource } from '../shared/table/table.data-source';

import { Tracker } from '../tracker.service';

@Component({
  selector: 'bio-vehicles',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatTableModule,
    MatSortModule,
    MatIconModule,
    MatDialogModule,
    TableActionsTriggerDirective
  ],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class VehiclesComponent implements OnInit, OnDestroy {
  REGISTRATION_NUMBER_PATTERN = REGISTRATION_NUMBER_PATTERN;

  protected vehiclesData$!: Observable<Vehicles>;
  protected vehiclesDataSource!: TableDataSource<VehicleDataSource>;
  protected columns = vehicleColumns;
  protected columnKeys!: string[];
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
          vehiclesOptions.sortBy = VehiclesSortBy.Name;

          break;

        case VehicleColumn.Type:
          vehiclesOptions.sortBy = VehiclesSortBy.Type;

          break;

        case VehicleColumn.Subtype:
          vehiclesOptions.sortBy = VehiclesSortBy.Subtype;

          break;

        case VehicleColumn.Group:
          vehiclesOptions.sortBy = VehiclesSortBy.Group;

          break;

        case VehicleColumn.Fuel:
          vehiclesOptions.sortBy = VehiclesSortBy.Fuel;
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
    const dialogRef = this.dialog.open<VehicleDialogComponent, void, true | '' | undefined>(VehicleDialogComponent);

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
      vehicleGroupId: group?.id,
      make,
      model,
      subType: subtype.key,
      fuelTypeId: fuel.id,
      manufacturingYear: year,
      registrationNumber: registration,
      inventoryNumber: inventory,
      serialNumber: serial,
      trackerId: tracker?.id,
      description
    };

    const dialogRef = this.dialog.open<VehicleDialogComponent, NewVehicle, true | '' | undefined>(VehicleDialogComponent, { data });

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
   * Send a command to vehicle GPS-tracker.
   *
   * @param trackerDataSource Tracker data source.
   */
  protected onSendTrackerCommand({ tracker }: VehicleDataSource) {
    const data: Tracker['id'] = tracker!.id;

    this.dialog.open<TrackerCommandDialogComponent, Tracker['id'], '' | undefined>(
      TrackerCommandDialogComponent,
      { ...trackerCommandDialogConfig, data }
    );
  }

  /**
   * Delete a vehicle in table.
   *
   * @param vehicleDataSource Vehicle data source.
   */
  protected async onDeleteVehicle({ id, name }: VehicleDataSource) {
    const data: ConfirmationDialogData = {
      content: getConfirmationDialogContent(name)
    };

    const dialogRef = this.dialog.open<ConfirmationDialogComponent, ConfirmationDialogData, boolean | undefined>(
      ConfirmationDialogComponent,
      { ...confirmationDialogConfig, data }
    );

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean),
        mergeMap(() => this.vehicleService.deleteVehicle(id))
      )
      .subscribe({
        next: () => {
          this.snackBar.open(VEHICLE_DELETED);

          this.#updateVehicles();
        },
        error: error => {
          this.errorHandler.handleError(error);

          this.#updateVehicles();
        }
      });
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
    this.columnKeys = this.columns.map(({ key }) => key);
  }

  constructor(
    private errorHandler: ErrorHandler,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private vehicleService: VehicleService
  ) { }

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

export const vehicleColumns: KeyValue<VehicleColumn, string | undefined>[] = [
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
  }
];

const REGISTRATION_NUMBER_PATTERN = /[a-zA-Zа-яА-ЯёЁ]+|[0-9]+/g;
export const VEHICLE_DELETED = 'Машина удалена';
