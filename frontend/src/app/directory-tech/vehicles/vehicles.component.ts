import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatTableModule } from '@angular/material/table';

import { BehaviorSubject, forkJoin, mergeMap, Observable, tap } from 'rxjs';

import { Fuel, Vehicle, VehicleGroup, Vehicles, VehicleService } from '../vehicle.service';

import { TableDataSource } from '../table.data-source';

@Component({
  selector: 'bio-vehicles',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule
  ],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehiclesComponent implements OnInit {
  protected vehiclesData$!: Observable<[Vehicles, VehicleGroup[], Fuel[], KeyValue<string, string>[], KeyValue<string, string>[]]>;
  protected vehiclesDataSource!: TableDataSource<VehicleDataSource>;
  protected columns = columns;
  protected columnsKeys!: string[];
  protected VehicleColumn = VehicleColumn;
  #vehicles$ = new BehaviorSubject(undefined);

  /**
   * Map vehicles data source.
   */
  #mapVehiclesDataSource(
    { vehicles }: Vehicles,
    groups: VehicleGroup[],
    fuels: Fuel[],
    types: KeyValue<string, string>[],
    subtypes: KeyValue<string, string>[]
  ) {
    return Object
      .freeze(vehicles)
      .map(({
        id,
        name,
        type: typeKey,
        vehicleGroupId,
        make,
        model,
        subType: subtypeKey,
        fuelTypeId,
        manufacturingYear: year,
        registrationNumber: registration,
        inventoryNumber: inventory,
        serialNumber: serial,
        description,
        tracker: {
          name: tracker
        } = {
          name: undefined
        }
      }): VehicleDataSource => {
        const type = types.find(({ key }) => key === typeKey);
        const subtype = subtypes.find(({ key }) => key === subtypeKey);
        const group = groups.find(({ id }) => id === vehicleGroupId);
        const fuel = fuels.find(({ id }) => id === fuelTypeId);

        return { id, name, make, model, type, subtype, group, year, fuel, registration, inventory, serial, tracker, description };
      });
  }

  /**
   * Initialize `TableDataSource` and set vehicles data source.
   *
   * @param vehiclesData Vehicles data.
   */
  #setVehiclesDataSource(vehiclesData: [Vehicles, VehicleGroup[], Fuel[], KeyValue<string, string>[], KeyValue<string, string>[]]) {
    const vehiclesDataSource = this.#mapVehiclesDataSource(...vehiclesData);

    this.vehiclesDataSource = new TableDataSource<VehicleDataSource>(vehiclesDataSource);
  }

  /**
   * Get vehicles, groups, fuels, type, subtype. Set vehicles data.
   */
  #setVehiclesData() {
    this.vehiclesData$ = this.#vehicles$.pipe(
      mergeMap(() => forkJoin([
        this.vehiclesService.getVehicles(),
        this.vehiclesService.vehicleGroups$,
        this.vehiclesService.fuels$,
        this.vehiclesService.vehicleType$,
        this.vehiclesService.vehicleSubType$
      ])),
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

  constructor(private vehiclesService: VehicleService) { }

  ngOnInit() {
    this.#setVehiclesData();
    this.#setColumnKeys();
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

export interface VehicleDataSource extends Pick<Vehicle, 'id' | 'name' | 'make' | 'model' | 'description'> {
  type?: KeyValue<string, string>;
  subtype?: KeyValue<string, string>;
  group?: VehicleGroup;
  year: number;
  fuel?: Fuel;
  registration?: string;
  inventory?: string;
  serial?: string;
  tracker?: string;
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
