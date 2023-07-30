import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { MatDialog } from '@angular/material/dialog';

import { BehaviorSubject, Observable, tap } from 'rxjs';

import { MonitoringVehicle, TechService } from './tech.service';
import { Tracker } from '../directory-tech/tracker.service';

import { MapComponent } from '../shared/map/map.component';
import { TechMonitoringStateComponent } from './shared/tech-monitoring-state/tech-monitoring-state.component';

import {
  TrackerCommandDialogComponent,
  trackerCommandDialogConfig
} from '../shared/tracker-command-dialog/tracker-command-dialog.component';

@Component({
  selector: 'bio-tech',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
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

  protected vehicles$!: Observable<MonitoringVehicle[]>;
  protected searchForm!: TechSearchForm;

  /**
   * Select all `MatCheckbox` component change event handler.
   *
   * @param event `MatCheckboxChange` change event.
   * @param list `MatSelectionList` list component.
   */
  protected onSelectAllChange({ checked }: MatCheckboxChange, list: MatSelectionList) {
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
  protected onSelectionChange({ source }: MatSelectionListChange) {
    const vehicleIDs = source.selectedOptions.selected.map<MonitoringVehicle['id']>(({ value }) => value.id);

    const selected = new Set(vehicleIDs);

    this.#options = { selected };
  }

  /**
   * Send a command to vehicle GPS-tracker.
   *
   * @param id `Tracker` ID.
   */
  protected onSendTrackerCommand(id: Tracker['id']) {
    const data: Tracker['id'] = id;

    this.dialog.open<TrackerCommandDialogComponent, Tracker['id'], '' | undefined>(
      TrackerCommandDialogComponent,
      { ...trackerCommandDialogConfig, data }
    );
  }

  #vehicles: MonitoringVehicle[] | undefined;
  #options$ = new BehaviorSubject<TechOptions>({});

  /**
   * Initialize Search form.
   */
  #initSearchForm() {
    this.searchForm = this.fb.group({
      search: this.fb.nonNullable.control<string | undefined>(undefined)
    });
  }

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

  constructor(private fb: FormBuilder, private dialog: MatDialog, private techService: TechService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSearchForm();
    this.#setVehicles();
  }
}

type TechSearchForm = FormGroup<{
  search: FormControl<string | undefined>;
}>;

type TechOptions = Partial<{
  selected: Set<MonitoringVehicle['id']>;
}>;

