import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatListModule } from '@angular/material/list';

import { Observable } from 'rxjs';

import { MonitoringVehicle, TechService } from './tech.service';

import { MapComponent } from '../shared/map/map.component';
import { TechMonitoringStateComponent } from './shared/tech-monitoring-state/tech-monitoring-state.component';

@Component({
  selector: 'bio-tech',
  standalone: true,
  imports: [
    CommonModule,
    MatCheckboxModule,
    MatListModule,
    TechMonitoringStateComponent,
    MapComponent
  ],
  templateUrl: './tech.component.html',
  styleUrls: ['./tech.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TechComponent implements OnInit {
  protected vehicles$!: Observable<MonitoringVehicle[]>;

  /**
   * Get and set vehicles.
   */
  #setVehicles() {
    this.vehicles$ = this.techService.getVehicles();
  }

  constructor(private techService: TechService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setVehicles();
  }
}
