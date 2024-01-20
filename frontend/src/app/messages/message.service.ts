import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { LocationAndTrackResponse, MonitoringSensor, MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { PAGE_NUM, PAGE_SIZE, Pagination, PaginationOptions } from '../directory-tech/shared/pagination';
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
      fromObject = {
        pageNum: PAGE_NUM,
        pageSize: PAGE_SIZE,
        ...fromObject
      };
    }

    const { pageNum, pageSize, vehicleId, periodStart, periodEnd, viewMessageType, parameterType } = fromObject;

    // just to keep request url params order always the same
    fromObject = { pageNum, pageSize, vehicleId, periodStart, periodEnd, viewMessageType };

    if (viewMessageType === MessageType.DataMessage) {
      fromObject.parameterType = parameterType;
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<Messages>('/api/telematica/messagesview/list', { params });
  }

  /**
   * Delete a data message.
   *
   * @param body Deleted messages ID.
   *
   * @returns An `Observable` of deleting message stream.
   */
  deleteMessages(body: DataMessage['id'][]) {
    return this.httpClient.delete<null>('/api/telematica/messagesview/delete-messages', { body });
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
    const { vehicleId, periodStart, periodEnd, viewMessageType, parameterType } = fromObject;

    // just to keep request url params order always the same
    fromObject = { vehicleId, periodStart, periodEnd, viewMessageType };

    if (parameterType) {
      fromObject.parameterType = parameterType;
    }

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

type TrackerMessage = DataMessage & {
  parameters?: TrackerParameter[];
};

type SensorMessage = DataMessage & {
  sensors?: MonitoringSensor[];
};

export type CommandMessage = {
  id: number;
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
