import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { BehaviorSubject, switchMap, Observable, tap, Subscription, filter } from 'rxjs';

import { TrackersSortBy, Tracker, Trackers, TrackersOptions, TrackerService } from '../tracker.service';

import { TableActionsTriggerDirective } from '../shared/table-actions-trigger/table-actions-trigger.directive';
import { TrackerDialogComponent } from '../tracker-dialog/tracker-dialog.component';

import { SortDirection } from '../shared/sort';

import { TableDataSource } from '../shared/table/table.data-source';

@Component({
  selector: 'bio-trackers',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    TableActionsTriggerDirective
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
  protected DATE_FORMAT = DATE_FORMAT;

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
    this.#subscription?.unsubscribe();

    const dialogRef = this.dialog.open<TrackerDialogComponent, any, true | '' | undefined>(TrackerDialogComponent);

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean)
      )
      .subscribe(() => {
        this.#updateTrackers();
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

  constructor(private dialog: MatDialog, private trackerService: TrackerService) { }

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

interface TrackerDataSource extends Pick<Tracker, 'id' | 'name' | 'imei' | 'description' | 'vehicle'> {
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
    value: '????????????????????????&#10;??????????????'
  },
  {
    key: TrackerColumn.External,
    value: '?????????????? ID&#10;??????????????'
  },
  {
    key: TrackerColumn.Type,
    value: '??????&#10;??????????????????'
  },
  {
    key: TrackerColumn.Sim,
    value: '?????????? SIM-&#10;??????????'
  },
  {
    key: TrackerColumn.IMEI,
    value: 'IMEI&#10;??????????????'
  },
  {
    key: TrackerColumn.Start,
    value: '??????????&#10;????????????'
  },
  {
    key: TrackerColumn.Vehicle,
    value: '????????????'
  }
];

export const DATE_FORMAT = 'd MMMM y, h:mm';
