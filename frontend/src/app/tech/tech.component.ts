import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, QueryList } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule, MatListOption, MatSelectionList } from '@angular/material/list';
import { MatExpansionModule, MatExpansionPanel } from '@angular/material/expansion';
import { MatDialog } from '@angular/material/dialog';

import {
  BehaviorSubject,
  Observable,
  Subscription,
  debounceTime,
  defer,
  distinctUntilChanged,
  interval,
  map,
  skipWhile,
  startWith,
  switchMap,
  tap
} from 'rxjs';

import { MonitoringVehicle, TechService, VehicleMonitoringInfo } from './tech.service';
import { Tracker } from '../directory-tech/tracker.service';

import { StopClickPropagationDirective } from '../shared/stop-click-propagation/stop-click-propagation.directive';
import { TechMonitoringStateComponent } from './shared/tech-monitoring-state/tech-monitoring-state.component';
import { TechMonitoringInfoComponent } from './shared/tech-monitoring-info/tech-monitoring-info/tech-monitoring-info.component';
import { MapComponent } from '../shared/map/map.component';

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
    MatExpansionModule,
    StopClickPropagationDirective,
    TechMonitoringStateComponent,
    TechMonitoringInfoComponent,
    MapComponent
  ],
  templateUrl: './tech.component.html',
  styleUrls: ['./tech.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TechComponent implements OnInit, OnDestroy {
  /**
   * Get search stream.
   *
   * @returns An `Observable` of search stream.
   */
  get #search$() {
    return this.searchForm
      .get('search')!
      .valueChanges.pipe(
        debounceTime(SEARCH_DEBOUNCE_DUE_TIME),
        map(searchValue => searchValue
          ?.trim()
          ?.toLocaleLowerCase()
        ),
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
  protected expandedPanelTechID?: MonitoringTech['id'];
  protected techInfo?: TechMonitoringInfo;

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

  /**
   * Get, set tech monitoring information.
   *
   * Toggle tech panel, save expanded panel tech ID.
   *
   * @param panel `MatExpansionPanel` instance.
   * @param id `MonitoringTech` ID.
   */
  protected onPanelToggle(panel: MatExpansionPanel, id: MonitoringTech['id']) {
    this.#techInfoSubscription?.unsubscribe();

    if (panel.expanded) {
      this.#techInfoSubscription = panel.afterCollapse.subscribe(() => {
        this.techInfo = undefined;
      });

      panel.toggle();

      this.expandedPanelTechID = undefined;
    } else {
      this.#techInfoSubscription = this.techService
        .getVehicleInfo(id)
        .subscribe(info => {
          this.expandedPanelTechID = id;
          this.techInfo = info;

          panel.toggle();
        });
    }
  }

  #options$ = new BehaviorSubject<TechOptions>({});
  #techInfoSubscription: Subscription | undefined;

  /**
   * Initialize Search form.
   */
  #initSearchForm() {
    this.searchForm = this.fb.group({
      search: this.fb.nonNullable.control<string | undefined>(undefined)
    });
  }

  /**
   * Remove diverged tech from selection.
   *
   * @param tech `MonitoringTech[]` tech.
   */
  #convergeSelection(tech: MonitoringTech[]) {
    const { selected } = this.#options;

    if (selected?.size) {
      const techIDs = tech.map(({ id }) => id);
      const ids = new Set(techIDs);

      selected.forEach(id => {
        const hasTechID = ids.has(id);

        if (!hasTechID) {
          selected.delete(id);
        }
      });

      this.#options = { selected };
    }
  }

  /**
   * Get and set tech.
   */
  #setTech() {
    const timer$ = interval(POLL_INTERVAL_PERIOD)
      .pipe(
        startWith(undefined)
      );

    const tech$ = defer(
      () => this.techService.getVehicles()
    )
      .pipe(
        tap(
          this.#convergeSelection.bind(this)
        )
      );

    const searchTech = (findCriterion: string) => this.techService.getVehicles({ findCriterion });

    this.tech$ = this.#search$.pipe(
      startWith(undefined),
      switchMap(searchQuery => timer$.pipe(
        switchMap(() => searchQuery ? searchTech(searchQuery) : tech$)
      ))
    );
  }

  constructor(private fb: FormBuilder, private dialog: MatDialog, private techService: TechService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSearchForm();
    this.#setTech();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#techInfoSubscription?.unsubscribe();
  }
}

type TechSearchForm = FormGroup<{
  search: FormControl<string | undefined>;
}>;

export type MonitoringTech = MonitoringVehicle;

type TechOptions = Partial<{
  selected: Set<MonitoringTech['id']>;
}>;

export type TechMonitoringInfo = VehicleMonitoringInfo;

export const POLL_INTERVAL_PERIOD = 5000;

export const SEARCH_MIN_LENGTH = 3;
export const SEARCH_DEBOUNCE_DUE_TIME = 500;
