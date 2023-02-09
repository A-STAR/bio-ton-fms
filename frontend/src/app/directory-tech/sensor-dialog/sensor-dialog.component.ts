import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { forkJoin, map, Observable } from 'rxjs';

import { SensorGroup, SensorService, SensorType, Unit } from '../sensor.service';

import { Tracker } from '../tracker.service';

@Component({
  selector: 'bio-sensor-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sensor-dialog.component.html',
  styleUrls: ['./sensor-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SensorDialogComponent implements OnInit {
  protected sensorData$!: Observable<{
    groups: SensorGroup[];
    validators: SensorType[];
    units: Unit[];
  }>;

  /**
   * Get sensor groups, sensor types, units. Set sensor data.
   */
  #setSensorData() {
    this.sensorData$ = forkJoin([
      this.sensorService.sensorGroups$,
      this.sensorService.sensorTypes$,
      this.sensorService.units$,
    ])
      .pipe(
        map(([groups, validators, units]) => ({groups, validators, units}))
      );
  }

  constructor(private sensorService: SensorService) {}

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setSensorData();
  }
}

export type SensorDialogData = Partial<{
  trackerId: Tracker['id'];
}>
