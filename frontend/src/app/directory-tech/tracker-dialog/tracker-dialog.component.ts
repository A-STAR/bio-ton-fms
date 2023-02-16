import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';

import { Observable } from 'rxjs';

import { TrackerService, TrackerTypeEnum } from '../tracker.service';

@Component({
  selector: 'bio-tracker-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './tracker-dialog.component.html',
  styleUrls: ['./tracker-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrackerDialogComponent implements OnInit {
  protected trackerType$!: Observable<KeyValue<TrackerTypeEnum, string>[]>;

  constructor(private trackerService: TrackerService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.trackerType$ = this.trackerService.trackerType$;
  }
}
