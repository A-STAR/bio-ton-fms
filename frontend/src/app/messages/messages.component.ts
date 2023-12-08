import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

import {
  Observable,
  Subject,
  Subscription,
  asapScheduler,
  debounceTime,
  defer,
  distinctUntilChanged,
  filter,
  forkJoin,
  map,
  skipWhile,
  startWith,
  switchMap
} from 'rxjs';

import { MessageService, MessageStatistics, MessageStatisticsOptions, MessageTrackOptions } from './message.service';

import { DateCharsInputDirective } from '../shared/date-chars-input/date-chars-input.directive';
import { TimeCharsInputDirective } from '../shared/time-chars-input/time-chars-input.directive';
import { MapComponent } from '../shared/map/map.component';

import { DEBOUNCE_DUE_TIME, MonitoringTech, SEARCH_MIN_LENGTH } from '../tech/tech.component';
import { LocationAndTrackResponse } from '../tech/tech.service';

@Component({
  selector: 'bio-messages',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MatDatepickerModule,
    MatSelectModule,
    MatButtonModule,
    DateCharsInputDirective,
    TimeCharsInputDirective,
    MapComponent
  ],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MessagesComponent implements OnInit, OnDestroy {
  /**
   * Get today's date.
   *
   * @returns Max start date.
   */
  get maxStartDate() {
    return new Date();
  }

  /**
   * Calculate tomorrow's max date.
   *
   * @returns Max end date.
   */
  get maxEndDate() {
    const date = new Date();
    const tomorrowDay = date.getDate() + 1;

    date.setDate(tomorrowDay);

    return date;
  }

  /**
   * Get search stream.
   *
   * @returns An `Observable` of search stream.
   */
  get #search$() {
    return this.selectionForm
      .get('tech')!
      .valueChanges.pipe(
        filter(value => typeof value !== 'object'),
        map(value => value as string | undefined),
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

  protected selectionForm!: MessageSelectionForm;
  protected tech$!: Observable<MonitoringTech[]>;
  protected statistics$!: Observable<MessageStatistics>;
  protected location$!: Observable<LocationAndTrackResponse>;
  protected MessageType = MessageType;
  protected DataMessageParameter = DataMessageParameter;

  /**
   * Map a tech option's control value to its name display value in the trigger.
   *
   * @param tech `MonitoringTech` tech.
   *
   * @returns `MonitoringTech` display value.
   */
  protected displayFn(tech: MonitoringTech): string {
    return tech?.name ?? '';
  }

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
   * Toggle `message` `parameters` control disabled state
   * on `message` `type` selection change conditionally.
   *
   * @param event `MatSelectionChange` event.
   */
  protected onMessageTypeSelectionChange({ value }: MatSelectChange) {
    const parametersControl = this.selectionForm.get('message.parameters');

    value === MessageType.DataMessage ? parametersControl?.enable() : parametersControl?.disable();
  }

  /**
   * Reset selection form default values.
   */
  protected onResetSelectionForm() {
    asapScheduler.schedule(() => {
      this.selectionForm
        .get('range.start.time')
        ?.setValue('00:00');

      this.selectionForm
        .get('range.end.time')
        ?.setValue('00:00');
    }, 1);
  }

  /**
   * Submit selection form, checking validation state.
   *
   * Get message statistics.
   */
  protected submitSelectionForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.selectionForm;

    if (invalid) {
      return;
    }

    const { tech, range, message } = value;

    const { start, end } = range!;
    const { type, parameters } = message!;

    const startDate = new Date(start!.date!);
    const endDate = new Date(end!.date!);

    const [startHours, startMinutes] = parseTime(start!.time!);

    startDate.setHours(startHours);
    startDate.setMinutes(startMinutes);

    const [endHours, endMinutes] = parseTime(end!.time!);

    endDate.setHours(endHours);
    endDate.setMinutes(endMinutes);

    const messageStatisticsOptions: MessageStatisticsOptions = {
      vehicleId: (tech as MonitoringTech).id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString(),
      viewMessageType: type!
    };

    if (parameters) {
      messageStatisticsOptions.parameterType = parameters;
    }

    const messageTrackOptions: MessageTrackOptions = {
      vehicleId: (tech as MonitoringTech).id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString()
    };

    this.#subscription = forkJoin([
      this.messageService.getStatistics(messageStatisticsOptions),
      this.messageService.getTrack(messageTrackOptions)
    ])
      .subscribe(([statistics, location]) => {
        this.#statistics$.next(statistics);
        this.#location$.next(location);
      });
  }

  #location$ = new Subject<LocationAndTrackResponse>();
  #statistics$ = new Subject<MessageStatistics>();
  #subscription: Subscription | undefined;

  /**
   * Validator that requires the control have a tech `object` value from selection.
   *
   * @param control A tech `AbstractControl`.
   *
   * @returns An error map with the `selectionRequired` property
   * if the validation check fails, otherwise `null`.
   */
  #selectionRequiredValidator({ value }: AbstractControl): ValidationErrors | null {
    if (value && typeof value !== 'object') {
      return {
        selectionRequired: true
      };
    }

    return null;
  }

  /**
   * A validator function that requires the range group have a minimum/maximum
   * value of time.
   *
   * @param group A range group.
   *
   * @returns A function that receives a range group and synchronously
   * sets `rangeTimeMin`, `rangeTimeMax` errors, returns a map of `range`
   * validation errors if present, otherwise `null`.
   */
  #rangeTimeValidator(group: AbstractControl): ValidationErrors | null {
    const startDateControl = group.get('start.date');
    const endDateControl = group.get('end.date');
    const startTimeControl = group.get('start.time');
    const endTimeControl = group.get('end.time');

    const isSameDay = startDateControl?.value && endDateControl?.value
      && startDateControl.value.valueOf() === endDateControl.value.valueOf();

    const hasPatternError = startTimeControl?.hasError('pattern') || endTimeControl?.hasError('pattern');

    if (isSameDay && !hasPatternError && startTimeControl?.value && endTimeControl?.value) {
      let startDate = new Date();
      let endDate = new Date();

      if (startDateControl?.value && endDateControl?.value) {
        startDate = new Date(startDateControl.value);
        endDate = new Date(endDateControl.value);
      }

      const [startHours, startMinutes] = parseTime(startTimeControl.value);

      startDate.setHours(startHours);
      startDate.setMinutes(startMinutes);

      const [endHours, endMinutes] = parseTime(endTimeControl.value);

      endDate.setHours(endHours);
      endDate.setMinutes(endMinutes);

      const start = startDate.getTime();
      const end = endDate.getTime();

      if (start >= end) {
        const min = startTimeControl.value;
        const max = endTimeControl.value;

        const startTimeErrors: ValidationErrors = {
          rangeTimeMax: {
            max,
            actual: min
          }
        };

        const endTimeErrors: ValidationErrors = {
          rangeTimeMin: {
            min,
            actual: max
          }
        };

        startTimeControl.setErrors(startTimeErrors);
        endTimeControl.setErrors(endTimeErrors);

        return {
          range: { min, max }
        };
      }
    }

    if (startTimeControl?.hasError('rangeTimeMax')) {
      startTimeControl.setErrors(null);
    }

    if (endTimeControl?.hasError('rangeTimeMin')) {
      endTimeControl.setErrors(null);
    }

    return null;
  }

  /**
   * Initialize Selection form.
   */
  #initSelectionForm() {
    this.selectionForm = this.fb.group({
      tech: this.fb.nonNullable.control<MonitoringTech | string | undefined>(undefined, [
        Validators.required,
        this.#selectionRequiredValidator
      ]),
      range: this.fb.group({
        start: this.fb.group({
          date: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
          time: this.fb.nonNullable.control<string | undefined>('00:00', [
            Validators.required,
            Validators.pattern(TIME_PATTERN)
          ])
        }),
        end: this.fb.group({
          date: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
          time: this.fb.nonNullable.control<string | undefined>('00:00', [
            Validators.required,
            Validators.pattern(TIME_PATTERN)
          ])
        })
      }, {
        validators: this.#rangeTimeValidator
      }),
      message: this.fb.group({
        type: this.fb.nonNullable.control<MessageType | undefined>(undefined, Validators.required),
        parameters: this.fb.nonNullable.control<DataMessageParameter | undefined>({
          value: undefined,
          disabled: true
        }, Validators.required)
      })
    });
  }

  /**
   * Set tech.
   */
  #setTech() {
    const tech$ = defer(
      () => this.messageService.getVehicles()
    );

    const searchTech = (findCriterion: string) => this.messageService.getVehicles({ findCriterion });

    this.tech$ = this.#search$.pipe(
      startWith(undefined),
      switchMap(searchQuery => searchQuery ? searchTech(searchQuery) : tech$)
    );
  }

  constructor(private fb: FormBuilder, private messageService: MessageService) {
    this.statistics$ = this.#statistics$.asObservable();
    this.location$ = this.#location$.asObservable();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSelectionForm();
    this.#setTech();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

type MessageSelectionForm = FormGroup<{
  tech: FormControl<MonitoringTech | string | undefined>;
  range: FormGroup<{
    start: FormGroup<{
      date: FormControl<string | undefined>;
      time: FormControl<string | undefined>;
    }>;
    end: FormGroup<{
      date: FormControl<string | undefined>;
      time: FormControl<string | undefined>;
    }>;
  }>;
  message: FormGroup<{
    type: FormControl<MessageType | undefined>;
    parameters: FormControl<DataMessageParameter | undefined>;
  }>;
}>;

export enum MessageType {
  DataMessage = 'dataMessage',
  CommandMessage = 'commandMessage'
}

export enum DataMessageParameter {
  TrackerData = 'trackerData',
  SensorData = 'sensorData'
}

export const TIME_PATTERN = /^(0?[0-9]|1\d|2[0-3]):(0[0-9]|[1-5]\d)$/;

/**
 * Parsing time from user input.
 *
 * @param value Time user input (00:00, 23:59 etc.).
 *
 * @returns Array of hours and minutes.
 */
export const parseTime = (value: string) => value
  .split(':')
  .map(Number);
