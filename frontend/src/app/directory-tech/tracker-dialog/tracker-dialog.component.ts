import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Observable, Subscription } from 'rxjs';

import { NewTracker, TrackerService, TrackerTypeEnum } from '../tracker.service';

import { NumberOnlyInputDirective } from 'src/app/shared/number-only-input/number-only-input.directive';

@Component({
  selector: 'bio-tracker-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    NumberOnlyInputDirective
  ],
  templateUrl: './tracker-dialog.component.html',
  styleUrls: ['./tracker-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerDialogComponent implements OnInit, OnDestroy {
  protected trackerType$!: Observable<KeyValue<TrackerTypeEnum, string>[]>;
  protected trackerForm!: FormGroup<TrackerForm>;

  /**
   * Submit Tracker form, checking validation state.
   */
  protected submitTrackerForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.trackerForm;

    if (invalid) {
      return;
    }

    const {
      basic,
      registration: registrationGroup,
      additional
    } = value;

    const { name, type, start } = basic!;
    const { external, sim, imei } = registrationGroup!;
    const { description } = additional!;

    const tracker: NewTracker = {
      externalId: external!,
      name: name!,
      simNumber: sim!,
      imei: imei!,
      trackerType: type!,
      description: description ?? undefined
    };

    if (start) {
      const [day, month, year, hours, minutes, seconds = 0] = start
        .split(/[\.\s:]/)
        .map(Number);

      const monthIndex = month - 1;

      const startDate = new Date(year, monthIndex, day, hours, minutes, seconds)
        .toISOString();

      tracker.startDate = startDate;
    }

    this.#subscription = this.trackerService
      .createTracker(tracker)
      .subscribe(() => {
        this.snackBar.open(TRACKER_CREATED);

        this.dialogRef.close(true);
      });
  }

  #subscription: Subscription | undefined;

  /**
 * Initialize Vehicle form.
 */
  #initTrackerForm() {
    this.trackerForm = this.fb.group({
      basic: this.fb.group({
        name: this.fb.nonNullable.control<NewTracker['name'] | undefined>(undefined, [
          Validators.required,
          Validators.maxLength(100)
        ]),
        type: this.fb.nonNullable.control<NewTracker['trackerType'] | undefined>(undefined, Validators.required),
        start: this.fb.nonNullable.control<string | undefined>(undefined, [
          Validators.pattern(DATE_PATTERN)
        ])
      }),
      registration: this.fb.group({
        external: this.fb.nonNullable.control<NewTracker['externalId'] | undefined>(undefined, [
          Validators.required,
          Validators.max(99999999999)
        ]),
        sim: this.fb.nonNullable.control<NewTracker['simNumber'] | undefined>(undefined, [
          Validators.required,
          Validators.pattern(SIM_PATTERN)
        ]),
        imei: this.fb.nonNullable.control<NewTracker['imei'] | undefined>(undefined, [
          Validators.required,
          Validators.pattern(IMEI_PATTERN)
        ])
      }),
      additional: this.fb.group({
        description: this.fb.nonNullable.control<NewTracker['description'] | undefined>(
          undefined,
          Validators.maxLength(500)
        )
      })
    });
  }

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<TrackerDialogComponent, true | '' | undefined>,
    private trackerService: TrackerService
  ) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.trackerType$ = this.trackerService.trackerType$;

    this.#initTrackerForm();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

type TrackerForm = {
  basic: FormGroup<{
    name: FormControl<NewTracker['name'] | undefined>;
    type: FormControl<NewTracker['trackerType'] | undefined>;
    start: FormControl<string | undefined>;
  }>;
  registration: FormGroup<{
    external: FormControl<NewTracker['externalId'] | undefined>;
    sim: FormControl<NewTracker['simNumber'] | undefined>;
    imei: FormControl<NewTracker['imei'] | undefined>;
  }>;
  additional: FormGroup<{
    description: FormControl<NewTracker['description'] | undefined>;
  }>;
}

const DATE_PATTERN = /^(0?[1-9]|[12]\d|3[01])\.(0?[1-9]|1[012])\.\d{4}\s(0?[0-9]|1\d|2[0-3])(:(0[0-9]|[1-5]\d)){1,2}$/;
const SIM_PATTERN = /^\+7\d{10}$/;
const IMEI_PATTERN = /^\d{15}$/;

export const TRACKER_CREATED = 'GPS-трекер создан';
