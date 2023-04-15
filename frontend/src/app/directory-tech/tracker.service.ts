import { Injectable } from '@angular/core';
import { KeyValue } from '@angular/common';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { SortOptions } from './shared/sort';

import { Vehicle } from './vehicle.service';
import { TrackerDataSource } from './trackers/trackers.component';

import { PAGE_NUM as pageNum, PAGE_SIZE as pageSize, Pagination, PaginationOptions } from './shared/pagination';

@Injectable({
  providedIn: 'root'
})
export class TrackerService {
  /**
   * Get tracker type enum.
   *
   * @returns An `Observable` of tracker type enum stream.
   */
  get trackerType$() {
    return this.httpClient.get<KeyValue<TrackerTypeEnum, string>[]>('/api/telematica/enums/trackertypeenum');
  }

  /**
   * Get trackers.
   *
   * @param fromObject Trackers params options.
   *
   * @returns An `Observable` of trackers stream.
   */
  getTrackers(fromObject: TrackersOptions = { pageNum, pageSize }) {
    if (!fromObject.pageNum || !fromObject.pageSize) {
      fromObject = { pageNum, pageSize, ...fromObject };
    }

    if (!fromObject.sortBy || !fromObject.sortDirection) {
      delete fromObject.sortBy;
      delete fromObject.sortDirection;
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<Trackers>('/api/telematica/trackers', { params });
  }

  /**
   * Create a new tracker.
   *
   * @param tracker A new tracker.
   *
   * @returns An `Observable` of creating tracker stream.
   */
  createTracker(tracker: NewTracker) {
    return this.httpClient.post<Tracker>('/api/telematica/tracker', tracker);
  }

  /**
   * Update a tracker.
   *
   * @param tracker An updated tracker.
   *
   * @returns An `Observable` of updating tracker stream.
   */
  updateTracker(tracker: NewTracker) {
    return this.httpClient.put<null>(`/api/telematica/tracker/${tracker.id}`, tracker);
  }

  /**
   * Send a command to tracker.
   *
   * @param id A tracker ID.
   *
   * @returns An `Observable` of command response.
   */
  sendTrackerCommand(id: Tracker['id'], command: TrackerCommand) {
    return this.httpClient.post<TrackerCommandResponse>(`/api/telematica/tracker-command/${id}`, command);
  }

  /**
   * Delete a tracker.
   *
   * @param id An deleted tracker ID.
   *
   * @returns An `Observable` of deleting tracker stream.
   */
  deleteTracker(id: TrackerDataSource['id']) {
    return this.httpClient.delete<null>(`/api/telematica/tracker/${id}`);
  }

  /**
   * Get tracker standard parameters.
   *
   * @param id A tracker ID.
   *
   * @returns An `Observable` of standard parameters stream.
   */
  getStandardParameters(id: Tracker['id']) {
    return this.httpClient.get<TrackerStandardParameter[]>(`/api/telematica/tracker/standard-parameters/${id}`);
  }

  /**
   * Get tracker parameters.
   *
   * @param id A tracker ID.
   *
   * @returns An `Observable` of parameters stream.
   */
  getParameters(id: Tracker['id']) {
    return this.httpClient.get<TrackerParameter[]>(`/api/telematica/tracker/parameters/${id}`);
  }

  /**
   * Get tracker parameters history.
   *
   * @param fromObject Tracker params options.
   *
   * @returns An `Observable` of parameters stream.
   */
  getParametersHistory(fromObject: TrackerParametersHistoryOptions = {
    pageNum,
    pageSize: TRACKER_PARAMETERS_HISTORY_PAGE_SIZE
  }) {
    if (!fromObject.pageNum || !fromObject.pageSize) {
      fromObject = {
        pageNum,
        pageSize: TRACKER_PARAMETERS_HISTORY_PAGE_SIZE,
        ...fromObject
      };
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<TrackerParametersHistory>(`/api/telematica/tracker/history`, { params });
  }

  constructor(private httpClient: HttpClient) { }
}

export enum TrackersSortBy {
  Name = 'name',
  External = 'externalId',
  Type = 'type',
  Sim = 'simNumber',
  Start = 'startDate',
  Vehicle = 'vehicle'
}

export enum TrackerTypeEnum {
  GalileoSkyV50 = 'galileoSkyV50',
  Retranslator = 'retranslator',
  WialonIPS = 'wialonIPS'
}

export enum TrackerCommandTransport {
  TCP = 'tcp',
  SMS = 'sms'
}

export enum TrackerParameterName {
  Time = 'time',
  Latitude = 'lat',
  Longitude = 'long',
  Altitude = 'alt',
  Speed = 'speed'
}

export type TrackersOptions = PaginationOptions & SortOptions<TrackersSortBy>

export type Tracker = {
  id: number;
  externalId: number;
  name: string;
  simNumber: string;
  imei: string;
  trackerType: KeyValue<TrackerTypeEnum, string>,
  startDate: string,
  description?: string,
  vehicle?: KeyValue<Vehicle['id'], Vehicle['name']>;
}

export interface NewTracker extends Partial<Pick<Tracker, 'id' | 'startDate'>>,
  Pick<Tracker, 'externalId' | 'name' | 'simNumber' | 'imei' | 'description'> {
  trackerType: TrackerTypeEnum;
}

export interface Trackers extends Pagination {
  trackers: Tracker[];
}

export type TrackerCommand = {
  commandText: string;
  transport: TrackerCommandTransport;
}

export type TrackerCommandResponse = {
  commandResponse?: string;
}

export type TrackerStandardParameter = {
  name: string;
  paramName: TrackerParameterName;
  lastValueDateTime?: string;
  lastValueDecimal?: number;
}

export type TrackerParameter = {
  paramName: TrackerParameterName.Time | string;
  lastValueDateTime?: string;
  lastValueDecimal?: number;
  lastValueString?: string;
}

export type TrackerParametersHistoryOptions = PaginationOptions & Partial<{
  trackerId: number;
}>

export type TrackerParameterHistory = {
  time: string;
  speed?: number;
  latitude?: number;
  longitude?: number;
  altitude?: number;
  parameters: string;
}

export interface TrackerParametersHistory extends Pagination {
  parameters: TrackerParameterHistory[];
}

export const TRACKER_PARAMETERS_HISTORY_PAGE_SIZE = 100;
