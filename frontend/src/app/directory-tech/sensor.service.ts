import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { KeyValue } from '@angular/common';

import { Tracker } from './tracker.service';

import { PAGE_NUM as pageNum, PAGE_SIZE as pageSize, Pagination, PaginationOptions } from './shared/pagination';

@Injectable({
  providedIn: 'root'
})
export class SensorService {
  /**
   * Get sensors.
   *
   * @param fromObject Sensors params options.
   *
   * @returns An `Observable` of sensors stream.
   */
  getSensors(fromObject: SensorsOptions = { pageNum, pageSize }) {
    if (!fromObject.pageNum || !fromObject.pageSize) {
      fromObject = { pageNum, pageSize, ...fromObject };
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<Sensors>(`/api/telematica/sensors`, { params });
  }

  constructor(private httpClient: HttpClient) { }
}

export enum SensorDataTypeEnum {
  Boolean = 'boolean',
  Number = 'number',
  String = 'string'
}

export enum ValidationTypeEnum {
  LogicalAnd = 'logicalAnd',
  LogicalOr = 'logicalOr',
  ZeroTest = 'zeroTest'
}

export type SensorsOptions = PaginationOptions & Partial<{
  trackerId: number;
}>

export type Sensor = {
  id: number;
  tracker: KeyValue<Tracker['id'], Tracker['name']>;
  name: string;
  dataType: SensorDataTypeEnum;
  sensorType: KeyValue<number, string>;
  description?: string;
  formula?: string;
  unit: KeyValue<number, string>;
  useLastReceived: boolean;
  validator: KeyValue<number, string>;
  validationType: ValidationTypeEnum;
  fuelUse: number;
  visibility: boolean;
}

export interface Sensors extends Pagination {
  sensors: Sensor[];
}
