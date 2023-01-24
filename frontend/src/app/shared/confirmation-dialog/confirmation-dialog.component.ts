import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogConfig } from '@angular/material/dialog';

@Component({
  selector: 'bio-confirmation-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfirmationDialogComponent { }

export type ConfirmationDialogData = {}

export const confirmationDialogConfig: MatDialogConfig<ConfirmationDialogData> = {
  width: '630px'
};
