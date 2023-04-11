import { ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogConfig, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatButtonModule } from '@angular/material/button';

import { Subject, Subscription } from 'rxjs';

import { Tracker, TrackerCommand, TrackerCommandResponse, TrackerCommandTransport, TrackerService } from '../../tracker.service';

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
export class TrackerCommandDialogComponent implements OnInit, OnDestroy {
  protected commandForm!: CommandForm;
  protected commandResponse$ = new Subject<TrackerCommandResponse['commandResponse'] | null | undefined>();
  protected TrackerCommandTransport = TrackerCommandTransport;

  /**
   * Submit Command form, checking validation state.
   */
  protected submitCommandForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.commandForm;

    const response = invalid ? null : undefined;

    // show/hide command response hidden paragraph without text
    this.commandResponse$.next(response);

    if (invalid) {
      return;
    }

    const { message, transport } = value;

    const command: TrackerCommand = {
      commandText: message!,
      transport: transport!
    };

    this.#subscription = this.trackerService
      .sendTrackerCommand(this.data, command)
      .subscribe(({ commandResponse }) => {
        this.commandResponse$.next(commandResponse);
      });
  }

  #subscription: Subscription | undefined;

  /**
 * Initialize Command form.
 */
  #initCommandForm() {
    this.commandForm = this.fb.group({
      message: this.fb.nonNullable.control<TrackerCommand['commandText'] | undefined>(undefined, Validators.required),
      transport: this.fb.nonNullable.control(TrackerCommandTransport.TCP, Validators.required)
    });
  }

  constructor(
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) protected data: Tracker['id'],
    private trackerService: TrackerService
  ) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initCommandForm();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

type CommandForm = FormGroup<{
  message: FormControl<TrackerCommand['commandText'] | undefined>;
  transport: FormControl<TrackerCommand['transport']>;
}>

export const trackerCommandDialogConfig: MatDialogConfig<Tracker['id']> = {
  width: '730px'
};
