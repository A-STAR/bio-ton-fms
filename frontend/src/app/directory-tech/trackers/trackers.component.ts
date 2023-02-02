import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BehaviorSubject, switchMap, Observable } from 'rxjs';

import { Trackers, TrackersService } from '../trackers.service';

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
  #trackers$ = new BehaviorSubject(undefined);

  /**
   * Get and set trackers.
   */
  #setTrackers() {
    this.trackers$ = this.#trackers$.pipe(
      switchMap(() => this.trackersService.getTrackers())
    );
  }

  constructor(private trackersService: TrackersService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setTrackers();
  }
}
