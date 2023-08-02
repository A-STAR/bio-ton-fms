import { ChangeDetectionStrategy, Component, OnInit, QueryList } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule, MatListOption, MatSelectionList } from '@angular/material/list';
import { MatDialog } from '@angular/material/dialog';

import { BehaviorSubject, Observable, debounceTime, distinctUntilChanged, map, skipWhile, startWith, switchMap } from 'rxjs';

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
  /**
   * Get search stream.
   *
   * @returns An `Observable` of search stream.
   */
  get #search$() {
    return this.searchForm
      .get('search')!
      .valueChanges.pipe(
        debounceTime(SEARCH_DEBOUNCE_TIME),
        distinctUntilChanged(),
        skipWhile(searchValue => searchValue ? searchValue.length < SEARCH_MIN_LENGTH : true),
        map(searchValue => searchValue !== undefined && searchValue.length < SEARCH_MIN_LENGTH ? undefined : searchValue),
        distinctUntilChanged()
      );
  }

  /**
   * Get tech options.
   *
   * @returns `TechOptions` value.
   */
  get #options() {
    return Object.freeze(this.#options$.value);
  }

  /**
   * Set tech options.
   */
  set #options(options: TechOptions) {
    this.#options$.next({ ...this.#options, ...options });
  }

  protected tech$!: Observable<MonitoringTech[]>;
  protected searchForm!: TechSearchForm;

  /**
   * Tech option selected state.
   *
   * @param id `MonitoringTech` ID.
   *
   * @returns Option selected state.
   */
  protected isSelected(id: MonitoringTech['id']) {
    return this.#options.selected?.has(id);
  }

  /**
   * Selection change handler.
   *
   * @param options `QueryList<MatListOption> | MatListOption[]` of options that have been changed.
   */
  protected handleSelectionChange(options: QueryList<MatListOption> | MatListOption[]) {
    const {
      selected = new Set()
    } = this.#options;

    options.forEach(option => {
      if (option.selected) {
        selected.add(option.value);
      } else {
        selected.delete(option.value);
      }
    });

    this.#options = { selected };
  }

  /**
   * Select all `MatCheckbox` component change event handler.
   *
   * @param event `MatCheckboxChange` change event.
   * @param list `MatSelectionList` list component.
   */
  protected onSelectAllChange({ checked }: MatCheckboxChange, list: MatSelectionList) {
    if (checked) {
      list.selectAll();
    } else {
      list.deselectAll();
    }

    this.handleSelectionChange(list.options);
  }

  /**
   * Send a command to tech GPS-tracker.
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
   * Get and set tech.
   */
  #setTech() {
    this.tech$ = this.#search$.pipe(
      startWith(undefined),
      switchMap(findCriterion => findCriterion ? this.techService.getVehicles({ findCriterion }) : this.techService.getVehicles())
    );
  }

  constructor(private fb: FormBuilder, private dialog: MatDialog, private techService: TechService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSearchForm();
    this.#setTech();
  }
}

type TechSearchForm = FormGroup<{
  search: FormControl<string | undefined>;
}>;

export type MonitoringTech = MonitoringVehicle;

type TechOptions = Partial<{
  selected: Set<MonitoringTech['id']>;
}>;

export const SEARCH_MIN_LENGTH = 3;
export const SEARCH_DEBOUNCE_TIME = 500;
