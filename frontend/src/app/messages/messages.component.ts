import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';

import { Observable, debounceTime, defer, distinctUntilChanged, filter, map, skipWhile, startWith, switchMap } from 'rxjs';

import { MessageService } from './message.service';

import { MapComponent } from '../shared/map/map.component';

import { DEBOUNCE_DUE_TIME, MonitoringTech, SEARCH_MIN_LENGTH } from '../tech/tech.component';

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
    MatButtonModule,
    MapComponent
  ],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MessagesComponent implements OnInit {
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

      const parseTime = (value: string) => value
        .split(':')
        .map(Number);

      const [startHours, startMinutes] = parseTime(startTimeControl.value);

      startDate.setUTCHours(startHours);
      startDate.setUTCMinutes(startMinutes);

      const [endHours, endMinutes] = parseTime(endTimeControl.value);

      endDate.setUTCHours(endHours);
      endDate.setUTCMinutes(endMinutes);

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
          time: this.fb.nonNullable.control<string | undefined>(undefined, [
            Validators.required,
            Validators.pattern(TIME_PATTERN)
          ])
        }),
        end: this.fb.group({
          date: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
          time: this.fb.nonNullable.control<string | undefined>(undefined, [
            Validators.required,
            Validators.pattern(TIME_PATTERN)
          ])
        })
      }, {
        validators: this.#rangeTimeValidator
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

  constructor(private fb: FormBuilder, private messageService: MessageService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSelectionForm();
    this.#setTech();
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
}>;

export const TIME_PATTERN = /^(0?[0-9]|1\d|2[0-3]):(0[0-9]|[1-5]\d)$/;
