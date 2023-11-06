import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Observable } from 'rxjs';

import { MessageService } from './message.service';

import { MapComponent } from '../shared/map/map.component';

import { MonitoringTech } from '../tech/tech.component';

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
export default class MessagesComponent implements OnInit {
  protected tech$!: Observable<MonitoringTech[]>;

  /**
   * Set tech.
   */
  #setTech() {
    this.tech$ = this.messageService.getVehicles();
  }

  constructor(private messageService: MessageService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setTech();
  }
}
