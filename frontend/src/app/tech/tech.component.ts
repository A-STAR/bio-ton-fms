import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatListModule, MatSelectionList, MatSelectionListChange } from '@angular/material/list';

import { BehaviorSubject, Observable, tap } from 'rxjs';

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
  get #options() {
    return Object.freeze(this.#options$.value);
  }

  set #options(options: TechOptions) {
    this.#options$.next({ ...this.#options, ...options });
  }

  /**
   * Select all `MatCheckbox` component change event handler.
   *
   * @param event `MatCheckboxChange` change event.
   * @param list `MatSelectionList` list component.
   */
  onSelectAllChange({ checked }: MatCheckboxChange, list: MatSelectionList) {
    let selected: Set<MonitoringVehicle['id']>;

    if (checked) {
      list.selectAll();

      const vehicleIDs = this.#vehicles?.map(({ id }) => id);

      selected = new Set(vehicleIDs);
    } else {
      list.deselectAll();

      selected = new Set();
    }

    this.#options = { selected };
  }

  /**
   * `MatSelectionList` component selection change event handler.
   *
   * @param event `MatSelectionListChange` selection change event.
   */
  onSelectionChange({ source }: MatSelectionListChange) {
    const vehicleIDs = source.selectedOptions.selected.map<MonitoringVehicle['id']>(({ value }) => value.id);

    const selected = new Set(vehicleIDs);

    this.#options = { selected };
  }

  protected vehicles$!: Observable<MonitoringVehicle[]>;

  #vehicles: MonitoringVehicle[] | undefined;
  #options$ = new BehaviorSubject<TechOptions>({});

  /**
   * Get and set vehicles.
   */
  #setVehicles() {
    this.vehicles$ = this.techService
      .getVehicles()
      .pipe(
        tap(vehicles => {
          this.#vehicles = vehicles;
        })
      );
  }

  constructor(private techService: TechService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setVehicles();
  }
}

type TechOptions = Partial<{
  selected: Set<MonitoringVehicle['id']>;
}>;
