import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-tracker-command-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tracker-command-dialog.component.html',
  styleUrls: ['./tracker-command-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerCommandDialogComponent { }
