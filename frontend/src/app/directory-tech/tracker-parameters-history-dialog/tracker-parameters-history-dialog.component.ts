import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';

import { Observable, tap } from 'rxjs';

import { Tracker, TrackerParameterHistory, TrackerParametersHistory, TrackerService } from '../tracker.service';

import { TableDataSource } from '../shared/table/table.data-source';

@Component({
  selector: 'bio-tracker-parameters-history-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatTableModule,
    MatChipsModule,
    MatButtonModule
  ],
  templateUrl: './tracker-parameters-history-dialog.component.html',
  styleUrls: ['./tracker-parameters-history-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerParametersHistoryDialogComponent implements OnInit {
  protected parameters$!: Observable<TrackerParametersHistory>;
  protected parametersDataSource!: TableDataSource<ParameterHistoryDataSource>;
  protected columns = trackerParameterHistoryColumns;
  protected columnKeys!: string[];
  protected Column = ParameterHistoryColumn;

  /**
   * Map parameters history data source.
   *
   * @param parameters Parameters history.
   *
   * @returns Mapped parameters history data source.
   */
  #mapParametersDataSource(parameters: TrackerParameterHistory[]) {
    return Object
      .freeze(parameters)
      .map(({ time, speed, latitude, longitude, altitude, parameters }): ParameterHistoryDataSource => ({
        time,
        speed,
        coordinates: { latitude, longitude },
        altitude,
        parameters
      }));
  }

  /**
   * Initialize `TableDataSource` with parameters data source.
   *
   * @param parameters Standard parameters.
   */
  #setParametersDataSource(parameters: TrackerParameterHistory[]) {
    const parametersHistory = this.#mapParametersDataSource(parameters);

    this.parametersDataSource = new TableDataSource<ParameterHistoryDataSource>(parametersHistory);
  }

  /**
   * Get and set tracker parameters history.
   */
  #setParameters() {
    this.parameters$ = this.trackerService
      .getParametersHistory({
        trackerId: this.data
      })
      .pipe(
        tap(({ parameters }) => {
          this.#setParametersDataSource(parameters);
        })
      );
  }

  /**
   * Set column keys.
   */
  #setColumnKeys() {
    this.columnKeys = this.columns.map(({ key }) => key);
  }

  constructor(@Inject(MAT_DIALOG_DATA) protected data: Tracker['id'], private trackerService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setParameters();
    this.#setColumnKeys();
  }
}

export enum ParameterHistoryColumn {
  Hash = 'hash',
  Time = 'time',
  Speed = 'speed',
  Coordinates = 'coordinates',
  Altitude = 'altitude',
  Parameters = 'parameters',
}

interface ParameterHistoryDataSource extends Pick<TrackerParameterHistory, 'time' | 'speed' | 'altitude' | 'parameters'> {
  coordinates: {
    latitude: TrackerParameterHistory['latitude'];
    longitude: TrackerParameterHistory['longitude'];
  };
}

export const trackerParameterHistoryColumns: KeyValue<ParameterHistoryColumn, string>[] = [
  {
    key: ParameterHistoryColumn.Hash,
    value: '#'
  },
  {
    key: ParameterHistoryColumn.Time,
    value: 'Время'
  },
  {
    key: ParameterHistoryColumn.Speed,
    value: 'Скорость, км/ч'
  },
  {
    key: ParameterHistoryColumn.Coordinates,
    value: 'Координаты'
  },
  {
    key: ParameterHistoryColumn.Altitude,
    value: 'Высота, м'
  },
  {
    key: ParameterHistoryColumn.Parameters,
    value: 'Параметры'
  }
];
