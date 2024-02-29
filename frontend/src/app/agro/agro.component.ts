import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MapComponent } from '../shared/map/map.component';

@Component({
  selector: 'bio-agro',
  standalone: true,
  imports: [
    CommonModule,
    MapComponent
  ],
  templateUrl: './agro.component.html',
  styleUrls: ['./agro.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class AgroComponent { }
