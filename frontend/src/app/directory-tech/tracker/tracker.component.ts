import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

import { BehaviorSubject, filter, map, Observable, switchMap, tap } from 'rxjs';

import { Sensors, TrackerService } from '../tracker.service';

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
  sensors$!: Observable<Sensors | undefined>;
  #sensors$ = new BehaviorSubject<Sensors | undefined>(undefined);

  /**
   * Get and set tracker sensors.
   */
  #setSensors() {
    this.sensors$ = this.route.paramMap.pipe(
      map(paramMap => paramMap.get('id')),
      filter(id => id !== null),
      map(Number),
      switchMap(trackerId => this.sensorService.getSensors({ trackerId })),
      tap(sensors => {
        this.#sensors$.next(sensors);
      }),
      switchMap(() => this.#sensors$)
    );
  }

  constructor(private route: ActivatedRoute, private sensorService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setSensors();
  }
}
