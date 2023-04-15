import { ChangeDetectionStrategy, Component, ErrorHandler, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { BehaviorSubject, switchMap, Observable, tap, Subscription, filter, mergeMap } from 'rxjs';

import { TrackersSortBy, Tracker, Trackers, TrackersOptions, TrackerService, NewTracker } from '../tracker.service';

import { TableActionsTriggerDirective } from '../shared/table-actions-trigger/table-actions-trigger.directive';
import { StopClickPropagationDirective } from 'src/app/shared/stop-click-propagation/stop-click-propagation.directive';
import { TrackerDialogComponent } from '../tracker-dialog/tracker-dialog.component';
import {
  TrackerCommandDialogComponent,
  trackerCommandDialogConfig
} from '../shared/tracker-command-dialog/tracker-command-dialog.component';
import {
  ConfirmationDialogComponent,
  confirmationDialogConfig,
  ConfirmationDialogData,
  getConfirmationDialogContent
} from '../../shared/confirmation-dialog/confirmation-dialog.component';

import { SortDirection } from '../shared/sort';

import { TableDataSource } from '../shared/table/table.data-source';

@Component({
  selector: 'bio-trackers',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    TableActionsTriggerDirective,
    StopClickPropagationDirective
  ],
  templateUrl: './trackers.component.html',
  styleUrls: ['./trackers.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TrackersComponent implements OnInit, OnDestroy {
  protected trackers$!: Observable<Trackers>;
  protected trackersDataSource!: TableDataSource<TrackerDataSource>;
  protected columns = trackerColumns;
  protected columnKeys!: string[];
  protected TrackerColumn = TrackerColumn;

  /**
   * `sortChange` handler sorting trackers.
   *
   * @param sort Sort state.
   */
  protected onSortChange({ active, direction }: Sort) {
    const trackersOptions: TrackersOptions = {};

    if (active && direction) {
      switch (active) {
        case TrackerColumn.Name:
          trackersOptions.sortBy = TrackersSortBy.Name;

          break;

        case TrackerColumn.External:
          trackersOptions.sortBy = TrackersSortBy.External;

          break;

        case TrackerColumn.Type:
          trackersOptions.sortBy = TrackersSortBy.Type;

          break;

        case TrackerColumn.Sim:
          trackersOptions.sortBy = TrackersSortBy.Sim;

          break;

        case TrackerColumn.Start:
          trackersOptions.sortBy = TrackersSortBy.Start;

          break;

        case TrackerColumn.Vehicle:
          trackersOptions.sortBy = TrackersSortBy.Vehicle;
      }

      switch (direction) {
        case 'asc':
          trackersOptions.sortDirection = SortDirection.Acending;

          break;

        case 'desc':
          trackersOptions.sortDirection = SortDirection.Descending;
      }
    }

    this.#trackers$.next(trackersOptions);
  }

  /**
   * Add a new GPS-tracker to table.
   */
  protected onCreateTracker() {
    const dialogRef = this.dialog.open<TrackerDialogComponent, void, true | '' | undefined>(TrackerDialogComponent);

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(() => {
        this.#updateTrackers();
      });
  }

  /**
   * Update a GPS-tracker in table.
   *
   * @param trackerDataSource Tracker data source.
   */
  protected onUpdateTracker({ id, external, name, sim, imei, type, start, description }: TrackerDataSource) {
    const data: NewTracker = {
      id,
      externalId: external,
      name,
      simNumber: sim,
      imei,
      trackerType: type.key,
      startDate: start,
      description
    };

    const dialogRef = this.dialog.open<TrackerDialogComponent, NewTracker, true | '' | undefined>(TrackerDialogComponent, { data });

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(() => {
        this.#updateTrackers();
      });
  }

  /**
   * Send a command to GPS-tracker.
   *
   * @param trackerDataSource Tracker data source.
   */
  protected onSendTrackerCommand({ id }: TrackerDataSource) {
    const data: Tracker['id'] = id;

    this.dialog.open<TrackerCommandDialogComponent, Tracker['id'], '' | undefined>(
      TrackerCommandDialogComponent,
      { ...trackerCommandDialogConfig, data }
    );
  }

  /**
   * Delete a GPS-tracker in table.
   *
   * @param vehicleDataSource Tracker data source.
   */
  protected async onDeleteTracker({ id, name }: TrackerDataSource) {
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
        mergeMap(() => this.trackerService.deleteTracker(id))
      )
      .subscribe({
        next: () => {
          this.snackBar.open(TRACKER_DELETED);

          this.#updateTrackers();
        },
        error: error => {
          this.errorHandler.handleError(error);

          this.#updateTrackers();
        }
      });
  }

  #trackers$ = new BehaviorSubject<TrackersOptions>({});
  #subscription: Subscription | undefined;

  /**
   * Emit trackers update.
   */
  #updateTrackers() {
    this.#trackers$.next(this.#trackers$.value);
  }

  /**
   * Map trackers data source.
   *
   * @param trackers Trackers with pagination.
   *
   * @returns Mapped trackers data source.
   */
  #mapTrackersDataSource({ trackers }: Trackers) {
    return Object
      .freeze(trackers)
      .map(({
        id,
        externalId: external,
        name,
        simNumber: sim,
        imei,
        trackerType: type,
        startDate: start,
        description,
        vehicle
      }): TrackerDataSource => ({ id, name, external, type, sim, imei, start, description, vehicle }));
  }

  /**
   * Initialize `TableDataSource` and set trackers data source.
   *
   * @param trackers Trackers.
   */
  #setTrackersDataSource(trackers: Trackers) {
    const trackersDataSource = this.#mapTrackersDataSource(trackers);

    this.trackersDataSource = new TableDataSource<TrackerDataSource>(trackersDataSource);
  }

  /**
   * Get and set trackers.
   */
  #setTrackers() {
    this.trackers$ = this.#trackers$.pipe(
      switchMap(trackersOptions => this.trackerService.getTrackers(trackersOptions)),
      tap(trackers => {
        this.#setTrackersDataSource(trackers);
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
    private trackerService: TrackerService
  ) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setTrackers();
    this.#setColumnKeys();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

export enum TrackerColumn {
  Action = 'action',
  Name = 'name',
  External = 'external',
  Type = 'type',
  Sim = 'sim',
  IMEI = 'imei',
  Start = 'start',
  Vehicle = 'vehicle'
}

export interface TrackerDataSource extends Pick<Tracker, 'id' | 'name' | 'imei' | 'description' | 'vehicle'> {
  external: Tracker['externalId'],
  type: Tracker['trackerType'],
  sim: Tracker['simNumber'],
  start: Tracker['startDate']
}

export const trackerColumns: KeyValue<TrackerColumn, string | undefined>[] = [
  {
    key: TrackerColumn.Action,
    value: undefined
  },
  {
    key: TrackerColumn.Name,
    value: 'Наименование&#10;трекера'
  },
  {
    key: TrackerColumn.External,
    value: 'Внешний ID&#10;трекера'
  },
  {
    key: TrackerColumn.Type,
    value: 'Тип&#10;устройств'
  },
  {
    key: TrackerColumn.Sim,
    value: 'Номер SIM-&#10;карты'
  },
  {
    key: TrackerColumn.IMEI,
    value: 'IMEI&#10;трекера'
  },
  {
    key: TrackerColumn.Start,
    value: 'Время&#10;начала'
  },
  {
    key: TrackerColumn.Vehicle,
    value: 'Машина'
  }
];

export const TRACKER_DELETED = 'GPS-трекер удален';
