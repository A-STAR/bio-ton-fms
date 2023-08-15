import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TechMonitoringInfo } from '../../../../tech/tech.component';

@Component({
  selector: 'bio-tech-monitoring-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tech-monitoring-info.component.html',
  styleUrls: ['./tech-monitoring-info.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TechMonitoringInfoComponent {
  @Input() info!: TechMonitoringInfo;
}
