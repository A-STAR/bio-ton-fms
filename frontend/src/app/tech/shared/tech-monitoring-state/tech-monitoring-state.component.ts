import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { RelativeTimePipe } from '../relative-time.pipe';

import { ConnectionStatus, MovementStatus } from '../../tech.service';
import { Tracker } from '../../../directory-tech/tracker.service';
import { MonitoringTech } from '../../tech.component';

@Component({
  selector: 'bio-tech-monitoring-state',
  standalone: true,
  imports: [
    CommonModule,
    RelativeTimePipe,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './tech-monitoring-state.component.html',
  styleUrls: ['./tech-monitoring-state.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TechMonitoringStateComponent {
  @Input() tech!: MonitoringTech;

  @Output() protected sendTrackerCommand = new EventEmitter<Tracker['id']>();

  protected selectedTrack?: boolean;
  protected MovementStatus = MovementStatus;
  protected ConnectionStatus = ConnectionStatus;

  /**
   * Handle tech track toggle.
   */
  protected onTrackToggle() {
    this.selectedTrack = !this.selectedTrack;
  }
}
