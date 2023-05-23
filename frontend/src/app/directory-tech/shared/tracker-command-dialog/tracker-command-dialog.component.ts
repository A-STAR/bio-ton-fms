import { ChangeDetectionStrategy, Component, ErrorHandler, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogConfig, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { BehaviorSubject, Subscription } from 'rxjs';

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
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './tracker-command-dialog.component.html',
  styleUrls: ['./tracker-command-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerCommandDialogComponent implements OnInit, OnDestroy {
  protected commandForm!: CommandForm;

  protected commandResponse$ = new BehaviorSubject<{
    message: TrackerCommandResponse['commandResponse'] | null | undefined;
    error?: true;
    progress?: true;
  }>({
    message: null
  });

  protected TrackerCommandTransport = TrackerCommandTransport;

  /**
   * Submit Command form, checking validation state.
   */
  protected submitCommandForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.commandForm;

    if (invalid) {
      // hide message paragraph
      this.commandResponse$.next({
        message: null
      });

      return;
    }

    const { message, transport } = value;

    const command: TrackerCommand = {
      commandText: message!,
      transport: transport!
    };

    // show/hide message hidden paragraph without text and set progress
    this.commandResponse$.next({
      message: this.commandResponse$.value?.message ? undefined : null,
      progress: true
    });

    this.#subscription = this.trackerService
      .sendTrackerCommand(this.data, command)
      .subscribe({
        next: ({ commandResponse }) => {
          this.commandResponse$.next({
            message: commandResponse
          });
        },
        error: error => {
          const isAuthError = [401, 403].includes(error.status);

          if (isAuthError) {
            // hide message paragraph
            this.commandResponse$.next({
              message: null
            });

            this.errorHandler.handleError(error);
          } else {
            this.commandResponse$.next({
              message: error.error.messages.join(' '),
              error: true
            });
          }
        }
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
    private errorHandler: ErrorHandler,
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
}>;

export const trackerCommandDialogConfig: MatDialogConfig<Tracker['id']> = {
  width: '730px'
};
