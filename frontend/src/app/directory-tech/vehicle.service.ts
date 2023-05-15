import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { KeyValue } from '@angular/common';

import { SortOptions } from './shared/sort';
import { VehicleDataSource } from './vehicles/vehicles.component';

import { PAGE_NUM as pageNum, PAGE_SIZE as pageSize, Pagination, PaginationOptions } from './shared/pagination';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  /**
   * Get vehicle groups.
   *
   * @returns An `Observable` of vehicle groups stream.
   */
  get vehicleGroups$() {
    return this.httpClient.get<VehicleGroup[]>('/api/telematica/vehiclegroups');
  }

  /**
   * Get fuel types.
   *
   * @returns An `Observable` of fuels stream.
   */
  get fuels$() {
    return this.httpClient.get<Fuel[]>('/api/telematica/fueltypes');
  }

  /**
   * Get vehicle type enum.
   *
   * @returns An `Observable` of vehicle type enum stream.
   */
  get vehicleType$() {
    return this.httpClient.get<KeyValue<VehicleType, string>[]>('/api/telematica/enums/vehicletypeenum');
  }

  /**
   * Get vehicle subtype enum.
   *
   * @returns An `Observable` of vehicle sub type enum stream.
   */
  get vehicleSubtype$() {
    return this.httpClient.get<KeyValue<VehicleSubtype, string>[]>('/api/telematica/enums/vehiclesubtypeenum');
  }

  /**
   * Get vehicles.
   *
   * @param fromObject Vehicles params options.
   *
   * @returns An `Observable` of vehicles stream.
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
   * @returns An `Observable` of creating vehicle.
   */
  createVehicle(vehicle: NewVehicle) {
    return this.httpClient.post<Vehicle>('/api/telematica/vehicle', vehicle);
  }

  /**
   * Update a vehicle.
   *
   * @param vehicle An updated vehicle.
   *
   * @returns An `Observable` of updating vehicle.
   */
  updateVehicle(vehicle: NewVehicle) {
    return this.httpClient.put<null>(`/api/telematica/vehicle/${vehicle.id}`, vehicle);
  }

  /**
   * Delete a vehicle.
   *
   * @param id An deleted vehicle ID.
   *
   * @returns An `Observable` of deleting vehicle.
   */
  deleteVehicle(id: VehicleDataSource['id']) {
    return this.httpClient.delete<null>(`/api/telematica/vehicle/${id}`);
  }

  constructor(private httpClient: HttpClient) { }
}

export enum VehiclesSortBy {
  Name = 'name',
  Type = 'type',
  Subtype = 'subtype',
  Group = 'group',
  Fuel = 'fuelType'
}

export enum VehicleType {
  Transport = 'transport',
  Argo = 'argo'
}

export enum VehicleSubtype {
  Tanker = 'tanker',
  Truck = 'truck',
  Other = 'other',
  Harvester = 'harvester',
  Car = 'car',
  Sprayer = 'sprayer',
  Telehandler = 'telehandler',
  Tractor = 'tractor'
}

export type VehiclesOptions = PaginationOptions & SortOptions<VehiclesSortBy>;

export type VehicleGroup = {
  id: number;
  name: string;
};

export type Fuel = {
  id: number;
  name: string;
};

export type Vehicle = {
  id: number;
  name: string;
  type: KeyValue<VehicleType, string>;
  vehicleGroup?: {
    id: VehicleGroup['id'];
    value: VehicleGroup['name'];
  };
  make: string;
  model: string;
  subType: KeyValue<VehicleSubtype, string>;
  fuelType: {
    id: Fuel['id'];
    value: Fuel['name'];
  };
  manufacturingYear?: number;
  registrationNumber?: string;
  inventoryNumber?: string;
  serialNumber?: string;
  tracker?: {
    id: number;
    value: string;
  };
  description?: string;
};

export interface NewVehicle extends Partial<Pick<Vehicle, 'id'>>, Pick<
  Vehicle,
  'name' | 'make' | 'model' | 'manufacturingYear' | 'registrationNumber' | 'inventoryNumber' | 'serialNumber' | 'description'
> {
  vehicleGroupId?: VehicleGroup['id'];
  type: VehicleType;
  subType: VehicleSubtype;
  fuelTypeId: Fuel['id'];
  trackerId?: number;
}

export interface Vehicles extends Pagination {
  vehicles: Vehicle[];
}
