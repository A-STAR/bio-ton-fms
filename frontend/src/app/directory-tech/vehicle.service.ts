import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { KeyValue } from '@angular/common';

export const pageNum = 1;
export const pageSize = 50;

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  /**
   * Get vehicle groups.
   *
   * @returns An `Observable' of vehicle groups stream.
   */
  get vehicleGroups$() {
    return this.httpClient.get<VehicleGroup[]>('/api/telematica/vehiclegroups');
  }

  /**
   * Get fuel types.
   *
   * @returns An `Observable' of fuels stream.
   */
  get fuels$() {
    return this.httpClient.get<Fuel[]>('/api/telematica/fueltypes');
  }

  /**
   * Get vehicle type enum.
   *
   * @returns An `Observable' of vehicle type enum stream.
   */
  get vehicleType$() {
    return this.httpClient.get<KeyValue<string, string>[]>('/api/telematica/enums/VehicleTypeEnum');
  }

  /**
   * Get vehicle sub type enum.
   *
   * @returns An `Observable' of vehicle sub type enum stream.
   */
  get vehicleSubType$() {
    return this.httpClient.get<KeyValue<string, string>[]>('/api/telematica/enums/VehicleSubTypeEnum');
  }

  /**
   * Get vehicles.
   *
   * @param fromObject Vehicles params options.
   *
   * @returns An `Observable' of vehicles stream.
   */
  getVehicles(fromObject: VehiclesOptions = { pageNum, pageSize }) {
    if (!fromObject.pageNum || !fromObject.pageSize) {
      fromObject = { pageNum, pageSize };
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<Vehicles>('/api/telematica/vehicles', { params });
  }

  constructor(private httpClient: HttpClient) { }
}

export type VehiclesOptions = Partial<{
  pageNum: number;
  pageSize: number;
}>

type Tracker = {
  id: number;
  name: string;
}

export type VehicleGroup = {
  id: number;
  name: string;
}

export type Fuel = {
  id: number;
  name: string;
}

type Vehicle = {
  id: number;
  name?: string;
  type: string;
  vehicleGroupId: number;
  make?: string;
  model?: string;
  subType: string;
  fuelTypeId: number;
  manufacturingYear: number;
  registrationNumber?: string;
  inventoryNumber?: string;
  serialNumber?: string;
  description?: string;
  tracker?: Tracker
}

type Pagination = {
  pageIndex: number;
  total: number;
}

export type Vehicles = {
  vehicles: Vehicle[];
  pagination: Pagination;
}
