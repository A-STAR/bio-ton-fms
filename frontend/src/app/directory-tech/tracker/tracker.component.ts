import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { BehaviorSubject, filter, map, Observable, shareReplay, Subscription, switchMap, tap } from 'rxjs';

import { TrackerParameterName, TrackerService, TrackerStandardParameter } from '../tracker.service';
import { Sensor, SensorService } from '../sensor.service';

import { TableActionsTriggerDirective } from '../shared/table-actions-trigger/table-actions-trigger.directive';
import { SensorDialogComponent, SensorDialogData } from '../sensor-dialog/sensor-dialog.component';

import { TableDataSource } from '../shared/table/table.data-source';

import { DATE_FORMAT } from '../trackers/trackers.component';

@Component({
  selector: 'bio-tracker',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule,
    MatDialogModule,
    TableActionsTriggerDirective
  ],
  templateUrl: './tracker.component.html',
  styleUrls: ['./tracker.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TrackerComponent implements OnInit, OnDestroy {
  protected standardParameters$!: Observable<TrackerStandardParameter[]>;
  protected sensors$!: Observable<Sensor[] | undefined>;
  protected standardParametersDataSource!: TableDataSource<StandardParameterDataSource>;
  protected sensorsDataSource!: TableDataSource<SensorDataSource>;
  protected parameterColumns = trackerParameterColumns;
  protected sensorColumns = sensorColumns;
  protected parameterColumnKeys!: string[];
  protected sensorColumnKeys!: string[];
  protected ParameterColumn = TrackerParameterColumn;
  protected TrackerParameterName = TrackerParameterName;
  protected DATE_FORMAT = DATE_FORMAT;
  protected SensorColumn = SensorColumn;

  /**
   * Add a new sensor to sensor table.
   */
  protected onCreateSensor() {
    const data: SensorDialogData = {
      trackerId: Number(
        this.route.snapshot.paramMap.get('id')!
      )
    };

    const dialogRef = this.dialog.open(SensorDialogComponent, { data });

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(sensor => {
        const sensors = Array.from(this.#sensors$.value!);

        sensors.push(sensor);

        this.#sensors$.next(sensors);
      });
  }

  #sensors$ = new BehaviorSubject<Sensor[] | undefined>(undefined);
  #subscription: Subscription | undefined;

  /**
   * Map standard parameters data source.
   *
   * @param parameters Standard parameters.
   *
   * @returns Mapped standard parameters data source.
   */
  #mapStandardParametersDataSource(parameters: TrackerStandardParameter[]) {
    return Object
      .freeze(parameters)
      .map(({ name, paramName, lastValueDateTime, lastValueDecimal }): StandardParameterDataSource => ({
        name,
        param: paramName,
        value: lastValueDecimal ?? lastValueDateTime
      }));
  }

  /**
   * Map sensors data source.
   *
   * @param sensors Sensors.
   *
   * @returns Mapped sensors data source.
   */
  #mapSensorsDataSource(sensors: Sensor[]) {
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
   * Initialize `TableDataSource` with standard parameters data source.
   *
   * @param parameters Standard parameters.
   */
  #setStandardParametersDataSource(parameters: TrackerStandardParameter[]) {
    const standardParametersDataSource = this.#mapStandardParametersDataSource(parameters);

    this.standardParametersDataSource = new TableDataSource<StandardParameterDataSource>(standardParametersDataSource);
  }

  /**
   * Initialize `TableDataSource` and set sensors data source.
   *
   * @param sensors Sensors.
   */
  #setSensorsDataSource(sensors: Sensor[]) {
    const sensorsDataSource = this.#mapSensorsDataSource(sensors);

    if (this.sensorsDataSource) {
      this.sensorsDataSource.setDataSource(sensorsDataSource);
    } else {
      this.sensorsDataSource = new TableDataSource<SensorDataSource>(sensorsDataSource);
    }
  }

  /**
   * Get and set tracker standard parameters, sensors.
   */
  #setTrackerData() {
    const trackerID$ = this.route.paramMap.pipe(
      map(paramMap => paramMap.get('id')),
      filter(id => id !== null),
      map(Number),
      shareReplay()
    );

    this.standardParameters$ = trackerID$.pipe(
      switchMap(id => this.trackerService.getStandardParameters(id)),
      tap(parameters => {
        this.#setStandardParametersDataSource(parameters);
      })
    );

    this.sensors$ = trackerID$.pipe(
      switchMap(trackerId => this.sensorService.getSensors({ trackerId })),
      tap(sensors => {
        this.#sensors$.next(sensors.sensors);
      }),
      switchMap(() => this.#sensors$),
      tap(sensors => {
        this.#setSensorsDataSource(sensors!);
      })
    );
  }

  /**
   * Set column keys.
   */
  #setColumnKeys() {
    this.parameterColumnKeys = this.parameterColumns.map(({ key }) => key);
    this.sensorColumnKeys = this.sensorColumns.map(({ key }) => key);
  }

  constructor(
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private trackerService: TrackerService,
    private sensorService: SensorService
  ) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setTrackerData();
    this.#setColumnKeys();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

export enum TrackerParameterColumn {
  Name = 'name',
  Param = 'param',
  Value = 'value'
}

export enum SensorColumn {
  Action = 'action',
  Name = 'name',
  Type = 'type',
  Unit = 'unit',
  Formula = 'formula',
  Visibility = 'visibility'
}

interface StandardParameterDataSource extends Pick<TrackerStandardParameter, 'name'> {
  param: TrackerStandardParameter['paramName'];
  value: TrackerStandardParameter['lastValueDateTime'] | TrackerStandardParameter['lastValueDecimal'];
}

interface SensorDataSource extends Pick<Sensor, 'id' | 'name' | 'unit' | 'formula' | 'description' | 'visibility'> {
  type: Sensor['sensorType']
}

export const trackerParameterColumns: KeyValue<TrackerParameterColumn, string>[] = [
  {
    key: TrackerParameterColumn.Name,
    value: 'Название'
  },
  {
    key: TrackerParameterColumn.Param,
    value: 'Имя параметра'
  },
  {
    key: TrackerParameterColumn.Value,
    value: 'Последнее значение'
  }
];

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
    key: SensorColumn.Visibility,
    value: 'Видимость'
  }
];
