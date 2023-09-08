import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { Vehicle } from '../directory-tech/vehicle.service';
import { Tracker, TrackerParameter } from '../directory-tech/tracker.service';
import { Sensor, Unit } from '../directory-tech/sensor.service';

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

  /**
   * Get a vehicle monitoring information.
   *
   * @param id An monitoring vehicle ID.
   *
   * @returns An `Observable` of the `VehicleMonitoringInfo` stream.
   */
  getVehicleInfo(id: MonitoringVehicle['id']) {
    return this.httpClient.get<VehicleMonitoringInfo>(`/api/telematica/monitoring/vehicle/${id}`);
  }

  /**
   * Get monitoring vehicles location and track from the start of the day.
   *
   * @param vehiclesOptions Opted monitoring vehicles.
   *
   * @returns An `Observable` of the `LocationsAndTracksResponse` stream.
   */
  getVehiclesLocationAndTrack(vehiclesOptions: LocationOptions[]) {
    const body = vehiclesOptions.map<LocationAndTrackRequest>(({ vehicleId, needReturnTrack }) => ({
      vehicleId,
      needReturnTrack: needReturnTrack ?? false
    }));

    const date = new Date();

    date.setHours(0, 0, 0, 0);

    const todayStart = date.toISOString();

    const paramsOptions: HttpParamsOptions = {
      fromObject: {
        trackStartTime: todayStart
      }
    };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.post<LocationAndTrackResponse>('/api/telematica/monitoring/locations-and-tracks', body, { params });
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

type MonitoringInfo = Partial<{
  lastMessageTime: string;
  speed: number;
  mileage: number;
  engineHours: number;
  satellitesNumber: number;
  latitude: number;
  longitude: number;
}>;

export type MonitoringVehicle = Pick<Vehicle, 'id' | 'name'> & Pick<MonitoringInfo, 'lastMessageTime'> & {
  movementStatus: MovementStatus;
  connectionStatus: ConnectionStatus;
  numberOfSatellites?: MonitoringInfo['satellitesNumber'];
  tracker?: Pick<Tracker, 'id' | 'externalId' | 'imei'>;
};

type MonitoringSensor = Pick<Sensor, 'name'> & {
  value?: string;
  unit?: Unit['name'];
};

type TrackerMonitoringInfo = Pick<Tracker, 'externalId' | 'simNumber' | 'imei'> & {
  trackerType: string;
  parameters?: TrackerParameter[];
  sensors?: MonitoringSensor[];
};

export type VehicleMonitoringInfo = {
  generalInfo: MonitoringInfo;
  trackerInfo: TrackerMonitoringInfo;
};

export type LocationAndTrackRequest = {
  vehicleId: MonitoringVehicle['id'];
  needReturnTrack: boolean;
};

export type LocationOptions = Pick<LocationAndTrackRequest, 'vehicleId'> & Partial<Pick<LocationAndTrackRequest, 'needReturnTrack'>>;

type ViewBounds = {
  upperLeftLatitude: number;
  upperLeftLongitude: number;
  bottomRightLatitude: number;
  bottomRightLongitude: number;
};

type TrackPointInfo = Pick<MonitoringInfo, 'speed' | 'latitude' | 'longitude'> & {
  messageId: number;
  time?: MonitoringInfo['lastMessageTime'];
  numberOfSatellites?: MonitoringInfo['satellitesNumber'];
};

type LocationAndTrack = {
  vehicleId: MonitoringVehicle['id'];
  vehicleName: MonitoringVehicle['name'];
  latitude: number;
  longitude: number;
  track?: TrackPointInfo[];
};

export type LocationAndTrackResponse = {
  viewBounds?: ViewBounds;
  tracks: LocationAndTrack[];
};
