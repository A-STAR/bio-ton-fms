import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';

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
