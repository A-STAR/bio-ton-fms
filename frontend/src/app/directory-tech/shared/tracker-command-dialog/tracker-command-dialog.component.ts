import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogConfig, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatButtonModule } from '@angular/material/button';

import { Tracker, TrackerCommand, TrackerCommandTransport } from '../../tracker.service';
import { Vehicle } from '../../vehicle.service';

@Component({
  selector: 'bio-tracker-command-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonToggleModule,
    MatButtonModule
  ],
  templateUrl: './tracker-command-dialog.component.html',
  styleUrls: ['./tracker-command-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerCommandDialogComponent implements OnInit {
  protected commandForm!: CommandForm;
  protected TrackerCommandTransport = TrackerCommandTransport;

  /**
 * Initialize Command form.
 */
  #initCommandForm() {
    this.commandForm = this.fb.group({
      message: this.fb.nonNullable.control<TrackerCommand['commandText'] | undefined>(undefined, Validators.required),
      transport: this.fb.nonNullable.control(TrackerCommandTransport.TCP, Validators.required)
    });
  }

  constructor(private fb: FormBuilder, @Inject(MAT_DIALOG_DATA) protected data: TrackerCommandDialogData) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initCommandForm();
  }
}

export interface TrackerCommandDialogData extends Pick<Tracker, 'id'> {
  vehicle?: Vehicle['name']
};

type CommandForm = FormGroup<{
  message: FormControl<TrackerCommand['commandText'] | undefined>;
  transport: FormControl<TrackerCommand['transport']>;
}>

export const trackerCommandDialogConfig: MatDialogConfig<TrackerCommandDialogData> = {
  width: '730px'
};
