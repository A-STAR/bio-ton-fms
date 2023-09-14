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
  debounce,
  debounceTime,
  defer,
  distinctUntilChanged,
  interval,
  map,
  of,
  skipWhile,
  startWith,
  switchMap,
  tap,
  timer
} from 'rxjs';

import { LocationAndTrackResponse, LocationOptions, MonitoringVehicle, TechService, VehicleMonitoringInfo } from './tech.service';
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
        debounceTime(DEBOUNCE_DUE_TIME),
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
   * Set tech options, reset location view bounds ignorance.
   */
  set #options(options: TechOptions) {
    this.#options$.next({ ...this.#options, ...options });
    this.#location$.next({});
  }

  protected tech$!: Observable<MonitoringTech[]>;
  protected location$!: Observable<LocationAndTrackResponse>;
  protected searchForm!: TechSearchForm;
  protected expandedPanelTechID?: MonitoringTech['id'];
  protected techInfo?: TechMonitoringInfo;

  /**
   * `TrackByFunction` to compute the identity of tech.
   *
   * @param index The index of the item within the iterable.
   * @param tech The `MonitoringTech` in the iterable.
   *
   * @returns `MonitoringTech` ID.
   */
  protected techTrackBy(index: number, { id }: MonitoringTech) {
    return id;
  }

  /**
   * Tech option selected state.
   *
   * @param id `MonitoringTech` ID.
   *
   * @returns Option selected state.
   */
  protected isSelected(id: MonitoringTech['id']) {
    return this.#options.selection?.has(id);
  }

  /**
   * Selection change handler.
   *
   * @param options `QueryList<MatListOption> | MatListOption[]` of options that have been changed.
   */
  protected handleSelectionChange(options: QueryList<MatListOption> | MatListOption[]) {
    const {
      selection = new Set()
    } = this.#options;

    options.forEach(option => {
      if (option.selected) {
        selection.add(option.value);
      } else {
        selection.delete(option.value);
      }
    });

    this.#options = { selection };
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
   * Toggle tech panel, check tech, save expanded panel tech ID, update location bounds.
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

      this.#location$.next({});
    } else {
      this.#techInfoSubscription = this.techService
        .getVehicleInfo(id)
        .subscribe(info => {
          const {
            selection = new Set()
          } = this.#options;

          selection.add(id);

          this.#options = { selection };

          this.expandedPanelTechID = id;
          this.techInfo = info;

          panel.toggle();
        });
    }
  }

  #location$ = new BehaviorSubject<ViewOptions>({});
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
    const { selection, trackSelection } = this.#options;

    if (selection?.size) {
      const techIDs = tech.map(({ id }) => id);
      const ids = new Set(techIDs);

      let isDiverged: true | undefined;

      selection.forEach(id => {
        const hasTechID = ids.has(id);

        if (!hasTechID) {
          selection.delete(id);
          trackSelection?.delete(id);

          isDiverged = true;
        }
      });

      if (isDiverged) {
        this.#options = { selection, trackSelection };
      }
    }
  }

  /**
   * Set tech location and track.
   */
  #setLocations() {
    const getLocation = (selection: TechOptions['selection']) => of(selection)
      .pipe(
        map(selection => Array.from(selection!, (vehicleId): LocationOptions => ({
          vehicleId,
          needReturnTrack: true
        }))),
        switchMap(options => this.techService.getVehiclesLocationAndTrack(options)),
        map(location => {
          if (this.#location$.value.ignoreViewBounds) {
            delete location.viewBounds;
          }

          return location;
        })
      );

    const unselectedLocation$ = of<LocationAndTrackResponse>({
      tracks: []
    });

    this.location$ = this.#location$.pipe(
      switchMap(() => this.#options$),
      debounce(() => timer(DEBOUNCE_DUE_TIME)),
      skipWhile(({ selection }) => selection === undefined),
      switchMap(({ selection }) => selection?.size ? getLocation(selection) : unselectedLocation$)
    );
  }

  /**
   * Set tech, update location, tech info.
   *
   * Update tech info.
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

    const updateTechInfo = (tech: MonitoringTech[]) => {
      this.#techInfoSubscription?.unsubscribe();

      let techInfo$: Observable<TechMonitoringInfo | undefined> = of(undefined);

      if (this.expandedPanelTechID) {
        const hasTech = tech.some(({ id }) => id === this.expandedPanelTechID);

        if (hasTech) {
          techInfo$ = this.techService
            .getVehicleInfo(this.expandedPanelTechID)
            .pipe(
              tap(info => {
                this.techInfo = info;
              })
            );
        }
      }

      return techInfo$.pipe(
        switchMap(() => of(tech))
      );
    };

    this.tech$ = this.#search$.pipe(
      startWith(undefined),
      switchMap(searchQuery => timer$.pipe(
        switchMap(() => searchQuery ? searchTech(searchQuery) : tech$)
      )),
      tap(() => {
        if (this.#options$.value.selection?.size) {
          this.#location$.next({
            ignoreViewBounds: true
          });
        }
      }),
      switchMap(tech => updateTechInfo(tech))
    );
  }

  constructor(private fb: FormBuilder, private dialog: MatDialog, private techService: TechService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSearchForm();
    this.#setLocations();
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
  selection: Set<MonitoringTech['id']>;
  trackSelection: Set<MonitoringTech['id']>;
}>;

type ViewOptions = {
  ignoreViewBounds?: true;
};

export type TechMonitoringInfo = VehicleMonitoringInfo;

export const POLL_INTERVAL_PERIOD = 5000;

export const SEARCH_MIN_LENGTH = 3;

export const DEBOUNCE_DUE_TIME = 500;
