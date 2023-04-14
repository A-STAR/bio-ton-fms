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

import { Tracker, TrackerParameter, TrackerParameterName, TrackerService, TrackerStandardParameter } from '../tracker.service';
import { Sensor, SensorService } from '../sensor.service';

import { TableActionsTriggerDirective } from '../shared/table-actions-trigger/table-actions-trigger.directive';
import { TrackerParametersHistoryDialogComponent } from '../tracker-parameters-history-dialog/tracker-parameters-history-dialog.component';
import { SensorDialogComponent, SensorDialogData } from '../sensor-dialog/sensor-dialog.component';
import {
  ConfirmationDialogComponent,
  confirmationDialogConfig,
  getConfirmationDialogContent
} from '../../shared/confirmation-dialog/confirmation-dialog.component';

import { TableDataSource } from '../shared/table/table.data-source';

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
  protected parameters$!: Observable<TrackerParameter[]>;
  protected parametersDataSource!: TableDataSource<ParameterDataSource>;
  protected sensors$!: Observable<Sensor[] | undefined>;
  protected standardParametersDataSource!: TableDataSource<StandardParameterDataSource>;
  protected sensorsDataSource!: TableDataSource<SensorDataSource>;
  protected parameterColumns = trackerParameterColumns;
  protected sensorColumns = sensorColumns;
  protected parameterColumnKeys!: string[];
  protected sensorColumnKeys!: string[];
  protected ParameterColumn = TrackerParameterColumn;
  protected TrackerParameterName = TrackerParameterName;
  protected SensorColumn = SensorColumn;

  /**
   * Open tracker parameters history dialog.
   */
  protected onParametersHistory() {
    const data: Tracker['id'] = Number(
      this.route.snapshot.paramMap.get('id')!
    );

    this.dialog.open(TrackerParametersHistoryDialogComponent, { data });
  }

  /**
   * Add a new sensor to sensor table.
   */
  protected onCreateSensor() {
    const data: SensorDialogData<Tracker['id']> = Number(
      this.route.snapshot.paramMap.get('id')!
    );

    const dialogRef = this.dialog.open<SensorDialogComponent, SensorDialogData<Tracker['id']>, Sensor>(SensorDialogComponent, { data });

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

  /**
   * Update a sensor in table.
   *
   * @param sensorDataSource Sensor data source.
   */
  protected onUpdateSensor({ id }: SensorDataSource) {
    const data: SensorDialogData<Sensor> = this.#sensors$.value!.find(sensor => sensor.id === id)!;

    const dialogRef = this.dialog.open<SensorDialogComponent, SensorDialogData<Sensor>, Sensor>(SensorDialogComponent, { data });

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(sensor => {
        const sensors = Array.from(this.#sensors$.value!);

        const index = sensors.findIndex(({ id }) => id === sensor.id);

        sensors[index] = sensor;

        this.#sensors$.next(sensors);
      });
  }

  /**
   * Duplicate a sensor in table.
   *
   * @param dataSource Sensor data source.
   */
  protected onDuplicateSensor(dataSource: SensorDataSource) {
    const { id, ...data } = this.#sensors$.value!.find(({ id }) => id === dataSource.id)!;

    const dialogRef = this.dialog.open<
      SensorDialogComponent,
      SensorDialogData<Omit<Sensor, 'id'>>,
      Sensor
    >(SensorDialogComponent, { data });

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

  /**
   * Delete a sensor.
   *
   * @param sensorDataSource Sensor data source.
   */
  protected async onDeleteSensor({ name }: SensorDataSource) {
    const data: InnerHTML['innerHTML'] = getConfirmationDialogContent(name);

    this.dialog.open<ConfirmationDialogComponent, InnerHTML['innerHTML'], boolean | undefined>(
      ConfirmationDialogComponent,
      { ...confirmationDialogConfig, data }
    );
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
   * Map parameters data source.
   *
   * @param parameters Parameters.
   *
   * @returns Mapped parameters data source.
   */
  #mapParametersDataSource(parameters: TrackerParameter[]) {
    return Object
      .freeze(parameters)
      .map(({ paramName, lastValueDateTime, lastValueDecimal, lastValueString }): ParameterDataSource => ({
        param: paramName,
        value: lastValueDecimal ?? lastValueDateTime ?? lastValueString
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
        formula: parameter,
        unit,
        visibility
      }): SensorDataSource => ({ id, name, type, parameter, unit, visibility }));
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
   * Initialize `TableDataSource` with parameters data source.
   *
   * @param parameters Parameters.
   */
  #setParametersDataSource(parameters: TrackerParameter[]) {
    const parametersDataSource = this.#mapParametersDataSource(parameters);

    this.parametersDataSource = new TableDataSource<ParameterDataSource>(parametersDataSource);
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
   * Get and set tracker standard parameters, parameters, sensors.
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

    this.parameters$ = trackerID$.pipe(
      switchMap(id => this.trackerService.getParameters(id)),
      tap(parameters => {
        this.#setParametersDataSource(parameters);
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
  Parameter = 'parameter',
  Visibility = 'visibility'
}

interface StandardParameterDataSource extends Pick<TrackerStandardParameter, 'name'> {
  param: TrackerStandardParameter['paramName'];
  value: TrackerStandardParameter['lastValueDateTime'] | TrackerStandardParameter['lastValueDecimal'];
}

interface ParameterDataSource {
  param: TrackerParameter['paramName'];
  value: TrackerParameter['lastValueDateTime'] | TrackerStandardParameter['lastValueDecimal'] | TrackerParameter['lastValueString'];
}

interface SensorDataSource extends Pick<Sensor, 'id' | 'name' | 'unit' | 'visibility'> {
  type: Sensor['sensorType'],
  parameter: Sensor['formula']
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
    key: SensorColumn.Parameter,
    value: 'Параметр'
  },
  {
    key: SensorColumn.Visibility,
    value: 'Видимость'
  }
];
