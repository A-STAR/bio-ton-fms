import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-map',
  standalone: true,
  imports: [CommonModule],
  template: '',
  styleUrls: ['./map.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapComponent { }
