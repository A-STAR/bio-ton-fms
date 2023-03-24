import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Observable, tap } from 'rxjs';

import { Tracker, TrackerParameterHistory, TrackerParametersHistory, TrackerService } from '../tracker.service';

import { TableDataSource } from '../shared/table/table.data-source';

@Component({
  selector: 'bio-tracker-parameters-history-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './tracker-parameters-history-dialog.component.html',
  styleUrls: ['./tracker-parameters-history-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerParametersHistoryDialogComponent implements OnInit {
  protected parameters$!: Observable<TrackerParametersHistory>;
  protected parametersDataSource!: TableDataSource<ParameterHistoryDataSource>;

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

  constructor(@Inject(MAT_DIALOG_DATA) protected data: Tracker['id'], private trackerService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setParameters();
  }
}

interface ParameterHistoryDataSource extends Pick<TrackerParameterHistory, 'time' | 'speed' | 'altitude' | 'parameters'> {
  coordinates: {
    latitude: TrackerParameterHistory['latitude'];
    longitude: TrackerParameterHistory['longitude'];
  }
}
