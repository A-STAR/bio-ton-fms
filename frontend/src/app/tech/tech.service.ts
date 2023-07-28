import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { Vehicle } from '../directory-tech/vehicle.service';
import { Tracker } from '../directory-tech/tracker.service';

@Injectable({
  providedIn: 'root'
})
export class TechService {
  /**
   * Get monitoring vehicles.
   *
   * @param fromObject Vehicles params options.
   *
   * @returns An `Observable` of the `MonitoringVehicle[]` stream.
   */
  getVehicles(fromObject?: MonitoringVehiclesOptions) {
    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<MonitoringVehicle[]>('/api/telematica/monitoring/vehicles', { params });
  }

  constructor(private httpClient: HttpClient) { }
}

export enum MovementStatus {
  NoData = 'noData',
  Moving = 'moving',
  Stopped = 'stopped'
}

export enum ConnectionStatus {
  NotConnected = 'notConnected',
  Connected = 'connected'
}

export type MonitoringVehiclesOptions = Partial<{
  findCriterion: string;
}>;

export type MonitoringVehicle = Pick<Vehicle, 'id' | 'name'> & {
  lastMessageTime?: string;
  movementStatus: MovementStatus;
  connectionStatus: ConnectionStatus;
  numberOfSatellites?: number;
  tracker?: Pick<Tracker, 'id' | 'externalId' | 'imei'>;
};
