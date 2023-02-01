import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

import { BehaviorSubject, filter, map, Observable, switchMap, tap } from 'rxjs';

import { Sensor, Sensors, TrackerService } from '../tracker.service';

import { TableDataSource } from '../table.data-source';

@Component({
  selector: 'bio-tracker',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule
  ],
  templateUrl: './tracker.component.html',
  styleUrls: ['./tracker.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TrackerComponent implements OnInit {
  protected sensors$!: Observable<Sensors | undefined>;
  protected sensorsDataSource!: TableDataSource<SensorDataSource>;
  protected sensorColumns = sensorColumns;
  protected sensorColumnKeys!: string[];
  #sensors$ = new BehaviorSubject<Sensors | undefined>(undefined);

  /**
   * Map sensors data source.
   *
   * @param sensors Sensors with pagination.
   *
   * @returns Mapped sensors data source.
   */
  #mapSensorsDataSource({ sensors }: Sensors) {
    return Object
      .freeze(sensors)
      .map(({
        id,
        name,
        sensorType: type,
        description,
        formula,
        unit,
        visibility
      }): SensorDataSource => ({ id, name, type, description, formula, unit, visibility }));
  }

  /**
   * Initialize `TableDataSource` and set sensors data source.
   *
   * @param sensors Sensors.
   */
  #setSensorsDataSource(sensors: Sensors) {
    const sensorsDataSource = this.#mapSensorsDataSource(sensors);

    this.sensorsDataSource = new TableDataSource<SensorDataSource>(sensorsDataSource);
  }

  /**
   * Get and set tracker sensors data.
   */
  #setSensors() {
    this.sensors$ = this.route.paramMap.pipe(
      map(paramMap => paramMap.get('id')),
      filter(id => id !== null),
      map(Number),
      switchMap(trackerId => this.sensorService.getSensors({ trackerId })),
      tap(sensors => {
        this.#sensors$.next(sensors);
        this.#setSensorsDataSource(sensors);
      }),
      switchMap(() => this.#sensors$)
    );
  }

  /**
   * Set column keys.
   */
  #setColumnKeys() {
    this.sensorColumnKeys = this.sensorColumns.map(({ key }) => key);
  }

  constructor(private route: ActivatedRoute, private sensorService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setSensors();
    this.#setColumnKeys();
  }
}

export enum SensorColumn {
  Action = 'action',
  Name = 'name',
  Type = 'type',
  Unit = 'unit',
  Formula = 'formula',
  Description = 'description',
  Visibility = 'visibility'
}

interface SensorDataSource extends Pick<Sensor, 'id' | 'name' | 'unit' | 'formula' | 'description' | 'visibility'> {
  type: Sensor['sensorType']
}

export const sensorColumns: KeyValue<SensorColumn, string | undefined>[] = [
  {
    key: SensorColumn.Action,
    value: undefined
  },
  {
    key: SensorColumn.Name,
    value: 'Имя'
  },
  {
    key: SensorColumn.Type,
    value: 'Тип'
  },
  {
    key: SensorColumn.Unit,
    value: 'Ед. измерения'
  },
  {
    key: SensorColumn.Formula,
    value: 'Параметр'
  },
  {
    key: SensorColumn.Description,
    value: 'Описание'
  },
  {
    key: SensorColumn.Visibility,
    value: 'Видимость'
  }
];
