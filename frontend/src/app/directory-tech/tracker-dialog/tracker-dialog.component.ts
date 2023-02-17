import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { Observable } from 'rxjs';

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
    NumberOnlyInputDirective
  ],
  templateUrl: './tracker-dialog.component.html',
  styleUrls: ['./tracker-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerDialogComponent implements OnInit {
  protected trackerType$!: Observable<KeyValue<TrackerTypeEnum, string>[]>;
  protected trackerForm!: FormGroup<TrackerForm>;

  /**
 * Initialize Vehicle form.
 */
  #initTrackerForm() {
    this.trackerForm = this.fb.group({
      basic: this.fb.group({
        name: this.fb.nonNullable.control<NewTracker['name'] | undefined>(undefined),
        type: this.fb.nonNullable.control<NewTracker['trackerType'] | undefined>(undefined),
        start: this.fb.nonNullable.control<string | undefined>(undefined)
      }),
      registration: this.fb.group({
        external: this.fb.nonNullable.control<NewTracker['externalId'] | undefined>(undefined),
        sim: this.fb.nonNullable.control<NewTracker['simNumber'] | undefined>(undefined),
        imei: this.fb.nonNullable.control<NewTracker['imei'] | undefined>(undefined)
      }),
      additional: this.fb.group({
        description: this.fb.nonNullable.control<NewTracker['description'] | undefined>(undefined)
      })
    });
  }

  constructor(private fb: FormBuilder, private trackerService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.trackerType$ = this.trackerService.trackerType$;

    this.#initTrackerForm();
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
