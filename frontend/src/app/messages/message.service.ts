import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { DataMessageParameter, MessageType } from './messages.component';

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

  /**
   * Get messages statistics.
   *
   * @param fromObject Message statistics params options.
   *
   * @returns An `Observable` of the `MessageStatistics` stream.
   */
  getStatistics(fromObject: MessageStatisticsOptions) {
    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<MessageStatistics>('/api/telematica/messagesview/statistics', { params });
  }

  constructor(private httpClient: HttpClient) { }
}

export type MessageStatisticsOptions = {
  vehicleId: MonitoringVehicle['id'];
  periodStart: string;
  periodEnd: string;
  viewMessageType: MessageType;
  parameterType?: DataMessageParameter;
};

export type MessageStatistics = {
  numberOfMessages: number;
  totalTime: number;
  distance?: number;
  mileage: number;
  averageSpeed: number;
  maxSpeed?: number;
};
