import { Injectable } from '@angular/core';
import { KeyValue } from '@angular/common';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

import { Vehicle } from './vehicle.service';

@Injectable({
  providedIn: 'root'
})
export class TrackersService {
  /**
   * Get trackers.
   *
   * @param fromObject Trackers params options.
   *
   * @returns An `Observable' of trackers stream.
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

export enum SortDirection {
  Acending = 'ascending',
  Descending = 'descending'
}

export enum TrackerTypeEnum {
  GalileoSkyV50 = 'galileoSkyV50',
  Retranslator = 'retranslator',
  WialonIPS = 'wialonIPS'
}

export type TrackersOptions = Partial<{
  pageNum: number;
  pageSize: number;
  sortBy: TrackersSortBy;
  sortDirection: SortDirection;
}>

export type Tracker = {
  id: number;
  externalId: number;
  name: string;
  simNumber?: string;
  imei: string;
  trackerType: KeyValue<TrackerTypeEnum, string>,
  startDate: Date,
  description?: string,
  vehicle?: KeyValue<Vehicle['id'], Vehicle['name']>;
}

type Pagination = {
  pageIndex: number;
  total: number;
}

export type Trackers = {
  trackers: Tracker[];
  pagination: Pagination;
}

export const pageNum = 1;
export const pageSize = 50;
