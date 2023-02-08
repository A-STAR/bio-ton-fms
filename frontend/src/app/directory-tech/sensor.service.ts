import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { KeyValue } from '@angular/common';

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

export type SensorsOptions = Partial<{
  pageNum: number;
  pageSize: number;
  trackerId: number;
}>

export type Sensor = {
  id: number;
  tracker: KeyValue<number, string>;
  name?: string;
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

type Pagination = {
  pageIndex: number;
  total: number;
}

export type Sensors = {
  sensors: Sensor[];
  pagination: Pagination;
}

export const pageNum = 1;
export const pageSize = 50;
