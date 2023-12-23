import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { LocationAndTrackResponse, MonitoringSensor, MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { PAGE_NUM as pageNum, PAGE_SIZE as pageSize, Pagination, PaginationOptions } from '../directory-tech/shared/pagination';
import { TrackerCommand, TrackerCommandResponse, TrackerCommandTransport, TrackerParameter } from '../directory-tech/tracker.service';

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
   * Get messages.
   *
   * @param fromObject Message params options.
   *
   * @returns An `Observable` of the `Messages` stream.
   */
  getMessages(fromObject: MessagesOptions) {
    if (!fromObject.pageNum || !fromObject.pageSize) {
      fromObject = { pageNum, pageSize, ...fromObject };
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<Messages>('/api/telematica/messagesview/list', { params });
  }

  /**
   * Get messages location and track.
   *
   * @param fromObject Message track params options.
   *
   * @returns An `Observable` of the `LocationAndTrackResponse` stream.
   */
  getTrack(fromObject: MessageTrackOptions) {
    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<LocationAndTrackResponse>('/api/telematica/messagesview/track', { params });
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

export type MessagesOptions = PaginationOptions & {
  vehicleId: MonitoringVehicle['id'];
  periodStart: string;
  periodEnd: string;
  viewMessageType: MessageType;
  parameterType?: DataMessageParameter;
};

export type MessageStatisticsOptions = Exclude<MessagesOptions, 'pageNum' | 'pageSize'>;

export type MessageTrackOptions = Pick<MessagesOptions, 'vehicleId' | 'periodStart' | 'periodEnd'>;

export type DataMessage = {
  id: number;
  num: number;
  trackerDateTime?: string;
  serverDateTime: string;
  speed?: number;
  latitude?: number;
  longitude?: number;
  satNumber?: number;
  altitude?: number;
};

export type TrackerMessage = DataMessage & {
  parameters: TrackerParameter[];
};

type SensorMessage = DataMessage & {
  sensors: MonitoringSensor[];
};

export type CommandMessage = {
  num: number;
  commandDateTime: string;
  commandText?: TrackerCommand['commandText'];
  executionTime?: string;
  channel: TrackerCommandTransport;
  commandResponseText?: TrackerCommandResponse['commandResponse'];
};

export type Messages = Pagination & Partial<{
  trackerDataMessages: TrackerMessage[];
  sensorDataMessages: SensorMessage[];
  commandMessages: CommandMessage[];
}>;

export type MessageStatistics = {
  numberOfMessages: number;
  totalTime: number;
  distance?: number;
  mileage: number;
  averageSpeed: number;
  maxSpeed?: number;
};
