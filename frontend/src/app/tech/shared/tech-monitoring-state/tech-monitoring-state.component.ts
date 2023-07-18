import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { ConnectionStatus, MonitoringVehicle, MovementStatus } from '../../tech.service';

import { StopClickPropagationDirective } from 'src/app/shared/stop-click-propagation/stop-click-propagation.directive';

@Component({
  selector: 'bio-tech-monitoring-state',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    StopClickPropagationDirective
  ],
  templateUrl: './tech-monitoring-state.component.html',
  styleUrls: ['./tech-monitoring-state.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TechMonitoringStateComponent {
  @Input() vehicle!: MonitoringVehicle;

  protected MovementStatus = MovementStatus;
  protected ConnectionStatus = ConnectionStatus;
}
