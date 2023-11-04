import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MapComponent } from '../shared/map/map.component';

@Component({
  selector: 'bio-messages',
  standalone: true,
  imports: [
    CommonModule,
    MapComponent
  ],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MessagesComponent { }
