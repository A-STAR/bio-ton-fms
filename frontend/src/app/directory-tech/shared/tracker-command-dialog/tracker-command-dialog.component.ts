import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogConfig, MatDialogModule } from '@angular/material/dialog';

import { Tracker } from '../../tracker.service';
import { Vehicle } from '../../vehicle.service';

@Component({
  selector: 'bio-tracker-command-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './tracker-command-dialog.component.html',
  styleUrls: ['./tracker-command-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerCommandDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) protected data: TrackerCommandDialogData) {}
}

export interface TrackerCommandDialogData extends Pick<Tracker, 'id'> {
  vehicle?: Vehicle['name']
};

export const trackerCommandDialogConfig: MatDialogConfig<TrackerCommandDialogData> = {
  width: '730px'
};
