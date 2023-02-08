import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { BehaviorSubject, switchMap, Observable, tap } from 'rxjs';

import { Tracker, Trackers, TrackersService } from '../trackers.service';

import { TableActionsTriggerDirective } from '../shared/table-actions-trigger/table-actions-trigger.directive';

import { TableDataSource } from '../shared/table/table.data-source';

@Component({
  selector: 'bio-trackers',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    TableActionsTriggerDirective
  ],
  templateUrl: './trackers.component.html',
  styleUrls: ['./trackers.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TrackersComponent implements OnInit {
  protected trackers$!: Observable<Trackers>;
  protected trackersDataSource!: TableDataSource<TrackerDataSource>;
  protected columns = trackerColumns;
  protected columnKeys!: string[];
  protected TrackerColumn = TrackerColumn;
  protected DATE_FORMAT = DATE_FORMAT;
  #trackers$ = new BehaviorSubject(undefined);

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
      switchMap(() => this.trackersService.getTrackers()),
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

  constructor(private trackersService: TrackersService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setTrackers();
    this.#setColumnKeys();
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
  Description = 'description',
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
    key: TrackerColumn.Description,
    value: 'Описание'
  },
  {
    key: TrackerColumn.Vehicle,
    value: 'Машина'
  }
];

export const DATE_FORMAT = 'd MMMM, y г., h:mm';
