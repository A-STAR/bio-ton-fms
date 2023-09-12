import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';

import { RelativeTimePipe } from '../../relative-time.pipe';

import { TechMonitoringInfo } from '../../../../tech/tech.component';
import { MonitoringSensor } from '../../../tech.service';
import { TrackerParameter } from '../../../../directory-tech/tracker.service';

@Component({
  selector: 'bio-tech-monitoring-info',
  standalone: true,
  imports: [
    CommonModule,
    MatChipsModule,
    RelativeTimePipe
  ],
  templateUrl: './tech-monitoring-info.component.html',
  styleUrls: ['./tech-monitoring-info.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TechMonitoringInfoComponent {
  @Input() info!: TechMonitoringInfo;

  /**
   * `TrackByFunction` to compute the identity of sensor.
   *
   * @param index The index of the item within the iterable.
   * @param tech The `MonitoringSensor` in the iterable.
   *
   * @returns `MonitoringSensor` name.
   */
  protected sensorTrackBy(index: number, { name }: MonitoringSensor) {
    return name;
  }

  /**
   * `TrackByFunction` to compute the identity of parameter.
   *
   * @param index The index of the item within the iterable.
   * @param tech The `TrackerParameter` in the iterable.
   *
   * @returns `TrackerParameter` name.
   */
  protected parameterTrackBy(index: number, { paramName }: TrackerParameter) {
    return paramName;
  }
}
