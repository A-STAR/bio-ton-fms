import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { KeyValue } from '@angular/common';

import { Tracker } from './tracker.service';
import { SensorDataSource } from './tracker/tracker.component';

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
    return this.httpClient.get<SensorGroup[]>('/api/telematica/sensorgroups');
  }

  /**
   * Get sensor types.
   *
   * @returns An `Observable` of sensor types stream.
   */
  get sensorTypes$() {
    return this.httpClient.get<SensorType[]>('/api/telematica/sensortypes');
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

  /**
   * Create a new sensor.
   *
   * @param sensor A new sensor.
   *
   * @returns An `Observable` of creating sensor.
   */
  createSensor(sensor: NewSensor) {
    return this.httpClient.post<Sensor>('/api/telematica/sensor', sensor);
  }

  /**
   * Update a sensor.
   *
   * @param sensor An updated sensor.
   *
   * @returns An `Observable` of updating sensor.
   */
  updateSensor(sensor: NewSensor) {
    return this.httpClient.put<null>(`/api/telematica/sensor/${sensor.id}`, sensor);
  }

  /**
   * Delete a sensor.
   *
   * @param id An deleted sensor ID.
   *
   * @returns An `Observable` of deleting sensor.
   */
  deleteSensor(id: SensorDataSource['id']) {
    return this.httpClient.delete<null>(`/api/telematica/sensor/${id}`);
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
  tracker: {
    id: Tracker['id'];
    value: Tracker['name'];
  };
  name: string;
  sensorType: {
    id: SensorType['id'];
    value: SensorType['name'];
  };
  dataType: SensorDataTypeEnum;
  formula?: string;
  unit: {
    id: Unit['id'];
    value: Unit['name'];
  };
  validator?: {
    id: SensorType['id'];
    value: SensorType['name'];
  };
  validationType?: ValidationTypeEnum;
  useLastReceived: boolean;
  visibility?: boolean;
  fuelUse?: number;
  description?: string;
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
