import { TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { Fuel, pageNum, pageSize, SortBy, SortDirection, VehicleGroup, Vehicles, VehicleService, VehiclesOptions } from './vehicle.service';

describe('VehicleService', () => {
  let httpTestingController: HttpTestingController;
  let service: VehicleService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(VehicleService);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should get vehicles', (done: DoneFn) => {
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
  });

  it('should get sorted vehicles', (done: DoneFn) => {
    let subscription = service
      .getVehicles({
        sortBy: SortBy.Name,
        sortDirection: SortDirection.Acending
      })
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles sorted by name')
          .toEqual(testVehicles);
      });

    let vehiclesRequest = httpTestingController.expectOne(
      `/api/telematica/vehicles?pageNum=${pageNum}&pageSize=${pageSize}&sortBy=${SortBy.Name}&sortDirection=${SortDirection.Acending}`,
      'sorted by name vehicles request'
    );

    vehiclesRequest.flush(testVehicles);

    subscription.unsubscribe();

    subscription = service
      .getVehicles({
        sortBy: SortBy.Fuel,
        sortDirection: SortDirection.Descending
      })
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles sorted by fuel in descending direction')
          .toEqual(testVehicles);
      });

    vehiclesRequest = httpTestingController.expectOne(
      `/api/telematica/vehicles?pageNum=${pageNum}&pageSize=${pageSize}&sortBy=${SortBy.Fuel}&sortDirection=${SortDirection.Descending}`,
      'descendingly sorted by fuel vehicles request'
    );

    vehiclesRequest.flush(testVehicles);

    subscription.unsubscribe();

    service
      .getVehicles({
        sortBy: SortBy.Name
      })
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get unsorted vehicles with missing sort direction')
          .toEqual(testVehicles);

        done();
      });

    vehiclesRequest = httpTestingController.expectOne(
      `/api/telematica/vehicles?pageNum=${pageNum}&pageSize=${pageSize}`,
      'unsorted vehicles request'
    );

    vehiclesRequest.flush(testVehicles);
  });

  it('should get vehicle groups', (done: DoneFn) => {
    service.vehicleGroups$.subscribe(groups => {
      expect(groups)
        .withContext('emit vehicle groups')
        .toEqual(testVehicleGroups);

      done();
    });

    const vehicleGroupsRequest = httpTestingController.expectOne('/api/telematica/vehiclegroups', 'vehicle groups request');

    vehicleGroupsRequest.flush(testVehicleGroups);
  });

  it('should get fuel types', (done: DoneFn) => {
    service.fuels$.subscribe(fuels => {
      expect(fuels)
        .withContext('emit fuels')
        .toEqual(testFuels);

      done();
    });

    const fuelsRequest = httpTestingController.expectOne('/api/telematica/fueltypes', 'fuels request');

    fuelsRequest.flush(testFuels);
  });
});

export const testVehicleGroups: VehicleGroup[] = [
  {
    id: 0,
    name: 'Комбайны CLAAS'
  },
  {
    id: 1,
    name: 'Тракторы Кировцы'
  },
  {
    id: 2,
    name: 'Легковые автомобили'
  }
];

export const testFuels: Fuel[] = [
  {
    id: 0,
    name: 'Бензин'
  },
  {
    id: 1,
    name: 'Дизельное топливо'
  }
];

export const testVehicles: Vehicles = {
  vehicles: [
    {
      id: 1,
      name: 'Марьевка',
      type: {
        key: 'Agro',
        value: 'Для работы на полях'
      },
      vehicleGroup: {
        key: testVehicleGroups[0].id.toString(),
        value: testVehicleGroups[0].name
      },
      make: 'CLAAS',
      model: 'Tucano 460',
      subType: {
        key: 'Harvester',
        value: 'Комбайн'
      },
      fuelType: {
        key: testFuels[1].id.toString(),
        value: testFuels[1].name
      },
      manufacturingYear: 2022,
      registrationNumber: '1200 AM 63',
      inventoryNumber: 'С293823729',
      serialNumber: '202039293834',
      description: 'Марьевское',
      tracker: {
        key: '1',
        value: '18-07-2539'
      }
    },
    {
      id: 2,
      name: 'Легковая машина',
      type: {
        key: 'Transport',
        value: 'Для перевозок'
      },
      vehicleGroup: {
        key: testVehicleGroups[2].id.toString(),
        value: testVehicleGroups[2].name
      },
      make: 'Ford',
      model: 'Focus',
      subType: {
        key: 'Car',
        value: 'Легковой автомобиль'
      },
      fuelType: {
        key: testFuels[0].id.toString(),
        value: testFuels[0].name
      },
      manufacturingYear: 2019,
      registrationNumber: '1290 AM 63',
      inventoryNumber: 'FF800110350',
      serialNumber: '800110350305',
      description: 'Частное'
    },
    {
      id: 3,
      name: 'Кировец',
      type: {
        key: 'Agro',
        value: 'Для работы на полях'
      },
      vehicleGroup: {
        key: testVehicleGroups[1].id.toString(),
        value: testVehicleGroups[1].name
      },
      make: 'Кировец',
      model: 'K-744',
      subType: {
        key: 'Tractor',
        value: 'Трактор'
      },
      fuelType: {
        key: testFuels[1].id.toString(),
        value: testFuels[1].name
      },
      manufacturingYear: 2017,
      registrationNumber: '1202 AК 63',
      inventoryNumber: 'М465890560',
      serialNumber: '678896767968',
      description: 'Кировское',
      tracker: {
        key: '2',
        value: '18-07-2557'
      }
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};
