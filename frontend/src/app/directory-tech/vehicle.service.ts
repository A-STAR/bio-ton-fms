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
   * Get vehicle subtype enum.
   *
   * @returns An `Observable' of vehicle sub type enum stream.
   */
  get vehicleSubtype$() {
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

  /**
   * Create a new vehicle.
   *
   * @param vehicle A new vehicle.
   *
   * @returns An `Observable' of adding vehicle.
   */
  createVehicle(vehicle: NewVehicle) {
    return this.httpClient.post('/api/telematica/vehicle', vehicle);
  }

  /**
   * Update a vehicle.
   *
   * @param vehicle An updated vehicle.
   *
   * @returns An `Observable' of updating vehicle.
   */
  updateVehicle(vehicle: NewVehicle) {
    return this.httpClient.put(`/api/telematica/vehicle/${vehicle.id}`, vehicle);
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
  name: string;
  type: KeyValue<string, string>;
  vehicleGroup?: KeyValue<string, string>;
  make: string;
  model: string;
  subType: KeyValue<string, string>;
  fuelType: KeyValue<string, string>;
  manufacturingYear?: number;
  registrationNumber?: string;
  inventoryNumber?: string;
  serialNumber?: string;
  tracker?: KeyValue<string, string>;
  description?: string;
}

export interface NewVehicle extends Partial<Pick<Vehicle, 'id'>>, Pick<
  Vehicle,
  'name' | 'make' | 'model' | 'manufacturingYear' | 'registrationNumber' | 'inventoryNumber' | 'serialNumber' | 'description'
> {
  vehicleGroupId?: VehicleGroup['id'];
  type: Vehicle['type']['key'];
  subType: Vehicle['subType']['key'];
  fuelTypeId: Fuel['id'];
  trackerId?: number;
}

type Pagination = {
  pageIndex: number;
  total: number;
}

export type Vehicles = {
  vehicles: Vehicle[];
  pagination: Pagination;
}
