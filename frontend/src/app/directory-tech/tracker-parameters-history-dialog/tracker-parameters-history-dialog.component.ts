import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { Tracker, TrackerParametersHistory, TrackerService } from '../tracker.service';

@Component({
  selector: 'bio-tracker-parameters-history-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './tracker-parameters-history-dialog.component.html',
  styleUrls: ['./tracker-parameters-history-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerParametersHistoryDialogComponent implements OnInit {
  protected parameters$!: Observable<TrackerParametersHistory>;

  /**
   * Get and set tracker parameters history.
   */
  #setParameters() {
    this.parameters$ = this.trackerService.getParametersHistory({
      trackerId: this.data
    });
  }

  constructor(@Inject(MAT_DIALOG_DATA) protected data: Tracker['id'], private trackerService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setParameters();
  }
}
