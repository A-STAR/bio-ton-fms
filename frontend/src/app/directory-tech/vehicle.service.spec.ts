import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { pageNum, pageSize, Vehicles, VehicleService, VehiclesOptions } from './vehicle.service';

describe('VehicleService', () => {
  let service: VehicleService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(VehicleService);
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should get vehicles', (done: DoneFn) => {
    const httpTestingController = TestBed.inject(HttpTestingController);

    let vehiclesOptions: VehiclesOptions | undefined;

    const subscription = service
      .getVehicles(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles without options')
          .toEqual(testVehicles);
      });

    let vehiclesRequest = httpTestingController.expectOne(
      `/api/telematica/vehicles?pageNum=${pageNum}&pageSize=${pageSize}`,
      'vehicles request'
    );

    vehiclesRequest.flush(testVehicles);

    subscription.unsubscribe();

    vehiclesOptions = {};

    service
      .getVehicles(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles with blank options')
          .toEqual(testVehicles);

        done();
      });

    vehiclesRequest = httpTestingController.expectOne(
      `/api/telematica/vehicles?pageNum=${pageNum}&pageSize=${pageSize}`,
      'vehicles request'
    );

    vehiclesRequest.flush(testVehicles);

    httpTestingController.verify();
  });
});

export const testVehicles: Vehicles = {
  vehicles: [
    {
      id: 1,
      name: 'Марьевка',
      type: 'Agro',
      vehicleGroupId: 1,
      make: 'CLAAS',
      model: 'Tucano 460',
      subType: 'Harvester',
      fuelTypeId: 0,
      manufacturingYear: 2022,
      registrationNumber: '1200 AM 63',
      inventoryNumber: 'С293823729',
      serialNumber: '202039293834',
      description: 'Марьевское',
      tracker: {
        id: 1,
        name: '18-07-2539'
      }
    },
    {
      id: 2,
      name: 'Легковая машина',
      type: 'Transport',
      vehicleGroupId: 2,
      make: 'Ford',
      model: 'Focus',
      subType: 'Car',
      fuelTypeId: 2,
      manufacturingYear: 2019,
      registrationNumber: '1290 AM 63',
      inventoryNumber: 'FF800110350',
      serialNumber: '800110350305',
      description: 'Частное'
    },
    {
      id: 3,
      name: 'Кировец',
      type: 'Agro',
      vehicleGroupId: 1,
      make: 'Кировец',
      model: 'K-744',
      subType: 'Tractor',
      fuelTypeId: 1,
      manufacturingYear: 2017,
      registrationNumber: '1202 AК 63',
      inventoryNumber: 'М465890560',
      serialNumber: '678896767968',
      description: 'Кировское',
      tracker: {
        id: 1,
        name: '18-07-2557'
      }
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};
