import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { ConnectionStatus, MovementStatus } from '../../tech.service';
import { Tracker } from '../../../directory-tech/tracker.service';
import { MonitoringTech } from '../../tech.component';

@Component({
  selector: 'bio-tech-monitoring-state',
  standalone: true,
  imports: [
    CommonModule,
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

  protected MovementStatus = MovementStatus;
  protected ConnectionStatus = ConnectionStatus;
}