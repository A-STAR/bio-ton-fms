import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  /**
   * Get message vehicles.
   *
   * @param fromObject Vehicles params options.
   *
   * @returns An `Observable` of the `MonitoringVehicle[]` stream.
   */
  getVehicles(fromObject?: MonitoringVehiclesOptions) {
    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<MonitoringVehicle[]>('/api/telematica/messagesview/vehicles', { params });
  }

  constructor(private httpClient: HttpClient) { }
}
