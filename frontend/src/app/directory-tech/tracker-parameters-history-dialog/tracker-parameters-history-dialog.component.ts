import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-tracker-parameters-history-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tracker-parameters-history-dialog.component.html',
  styleUrls: ['./tracker-parameters-history-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerParametersHistoryDialogComponent { }
