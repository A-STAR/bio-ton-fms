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
   * Get vehicles.
   *
   * @param fromObject Vehicles params options.
   *
   * @returns An `Observable' of vehicles stream.
   */
  getVehicles(fromObject: VehiclesOptions = { pageNum, pageSize }) {
    if (!fromObject.pageNum || !fromObject.pageSize) {
      fromObject = { pageNum, pageSize, ...fromObject };
    }

    if (!fromObject.sortBy || !fromObject.sortDirection) {
      delete fromObject.sortBy;
      delete fromObject.sortDirection;
    }

    const paramsOptions: HttpParamsOptions = { fromObject };

    const params = new HttpParams(paramsOptions);

    return this.httpClient.get<Vehicles>('/api/telematica/vehicles', { params });
  }

  constructor(private httpClient: HttpClient) { }
}

export enum SortBy {
  Name = 'name',
  Type = 'type',
  Subtype = 'subtype',
  Group = 'group',
  Fuel = 'fuelType'
}

export enum SortDirection {
  Acending = 'ascending',
  Descending = 'descending'
}

export type VehiclesOptions = Partial<{
  pageNum: number;
  pageSize: number;
  sortBy: SortBy;
  sortDirection: SortDirection;
}>

export type VehicleGroup = {
  id: number;
  name: string;
}

export type Fuel = {
  id: number;
  name: string;
}

export type Vehicle = {
  id: number;
  name?: string;
  type: KeyValue<string, string>;
  vehicleGroup: KeyValue<string, string>;
  make?: string;
  model?: string;
  subType: KeyValue<string, string>;
  fuelType: KeyValue<string, string>;
  manufacturingYear: number;
  registrationNumber?: string;
  inventoryNumber?: string;
  serialNumber?: string;
  description?: string;
  tracker?: KeyValue<string, string>
}

type Pagination = {
  pageIndex: number;
  total: number;
}

export type Vehicles = {
  vehicles: Vehicle[];
  pagination: Pagination;
}
