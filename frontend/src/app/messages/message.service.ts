import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { MonitoringVehicle } from '../tech/tech.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  /**
   * Get message vehicles.
   *
   * @returns An `Observable` of the `MonitoringVehicle[]` stream.
   */
  getVehicles() {
    return this.httpClient.get<MonitoringVehicle[]>('/api/telematica/messagesview/vehicles');
  }

  constructor(private httpClient: HttpClient) { }
}
