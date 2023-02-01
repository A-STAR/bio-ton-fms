import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-trackers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './trackers.component.html',
  styleUrls: ['./trackers.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TrackersComponent { }
