import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Vehicle } from '../directory-tech/vehicle.service';
import { Tracker } from '../directory-tech/tracker.service';

@Injectable({
  providedIn: 'root'
})
export class TechService {
  /**
   * Get monitoring vehicles.
   *
   * @returns An `Observable` of the `MonitoringVehicle[]` stream.
   */
  getVehicles() {
    return this.httpClient.get<MonitoringVehicle[]>('/api/telematica/monitoring/vehicles');
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

export type MonitoringVehicle = Pick<Vehicle, 'id' | 'name'> & {
  lastMessageTime?: string;
  movementStatus: MovementStatus;
  connectionStatus: ConnectionStatus;
  numberOfSatellites?: number;
  trackerExternalId?: Tracker['externalId'];
  trackerIMEI?: Tracker['imei'];
};
