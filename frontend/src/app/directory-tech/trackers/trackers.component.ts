import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BehaviorSubject, switchMap, Observable, tap } from 'rxjs';

import { Tracker, Trackers, TrackersService } from '../trackers.service';

import { TableDataSource } from '../shared/table/table.data-source';

@Component({
  selector: 'bio-trackers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './trackers.component.html',
  styleUrls: ['./trackers.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TrackersComponent implements OnInit {
  protected trackers$!: Observable<Trackers>;
  protected trackersDataSource!: TableDataSource<TrackerDataSource>;
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

  constructor(private trackersService: TrackersService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setTrackers();
  }
}

interface TrackerDataSource extends Pick<Tracker, 'id' | 'name' | 'imei' | 'description' | 'vehicle'> {
  external: Tracker['externalId'],
  type: Tracker['trackerType'],
  sim: Tracker['simNumber'],
  start: Tracker['startDate']
}
