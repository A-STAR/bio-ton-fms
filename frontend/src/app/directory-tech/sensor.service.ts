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
   * Get sensor groups.
   *
   * @returns An `Observable` of sensor groups stream.
   */
  get sensorGroups$() {
    return this.httpClient.get<SensorGroup[]>('/api/telematica/sensorGroups');
  }

  /**
   * Get sensor types.
   *
   * @returns An `Observable` of sensor types stream.
   */
  get sensorTypes$() {
    return this.httpClient.get<SensorType[]>('/api/telematica/sensorTypes');
  }

  /**
   * Get units.
   *
   * @returns An `Observable` of units stream.
   */
  get units$() {
    return this.httpClient.get<Unit[]>('/api/telematica/units');
  }

  /**
   * Get sensor data type enum.
   *
   * @returns An `Observable` of sensor data type enum stream.
   */
  get sensorDataType$() {
    return this.httpClient.get<KeyValue<SensorDataTypeEnum, string>[]>('/api/telematica/enums/sensordatatypeenum');
  }

  /**
   * Get validation type enum.
   *
   * @returns An `Observable` of validation type enum stream.
   */
  get validationType$() {
    return this.httpClient.get<KeyValue<ValidationTypeEnum, string>[]>('/api/telematica/enums/validationtypeenum');
  }

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

export type SensorTypeNested = {
  id: number;
  name: string;
  description?: string;
  sensorGroupId: number;
  dataType: SensorDataTypeEnum;
  unitId?: number;
}

export type SensorGroup = {
  id: number;
  name: string;
  sensorTypes?: SensorTypeNested[];
  description?: string;
}

export type SensorType = {
  id: number;
  name: string;
  description?: string;
  sensorGroup: KeyValue<SensorGroup['id'], SensorGroup['name']>;
  dataType: SensorDataTypeEnum;
  unit: KeyValue<Unit['id'], Unit['name']>;
}

export type Unit = {
  id: number;
  name: string;
  abbreviated?: string;
}

export type Sensor = {
  id: number;
  tracker: KeyValue<Tracker['id'], Tracker['name']>;
  name: string;
  dataType: SensorDataTypeEnum;
  sensorType: KeyValue<SensorType['id'], SensorType['name']>;
  description?: string;
  formula?: string;
  unit: KeyValue<Unit['id'], Unit['name']>;
  useLastReceived: boolean;
  validator?: KeyValue<number, string>;
  validationType?: ValidationTypeEnum;
  fuelUse?: number;
  visibility?: boolean;
}

export interface Sensors extends Pagination {
  sensors: Sensor[];
}

export interface NewSensor extends Partial<Pick<Sensor, 'id'>>, Pick<
  Sensor,
  'name' | 'dataType' | 'formula' | 'validationType' | 'useLastReceived' | 'visibility' | 'fuelUse' | 'description'
> {
  trackerId: Tracker['id'];
  sensorTypeId: SensorType['id'];
  unitId: Unit['id'];
  validatorId?: number;
}
