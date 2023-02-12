import { TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  Fuel,
  NewVehicle,
  pageNum,
  pageSize,
  SortBy,
  SortDirection,
  VehicleGroup,
  Vehicles,
  VehicleService,
  VehiclesOptions
} from './vehicle.service';

import { testDataSource as testVehiclesDataSource } from './table.data-source.spec';

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

  it('should get vehicle type enum', (done: DoneFn) => {
    service.vehicleType$.subscribe(vehicleType => {
      expect(vehicleType)
        .withContext('emit vehicle type enum')
        .toEqual(testVehicleTypeEnum);

      done();
    });

    const vehicleTypeEnumRequest = httpTestingController.expectOne('/api/telematica/enums/VehicleTypeEnum', 'vehicle type enum request');

    vehicleTypeEnumRequest.flush(testVehicleTypeEnum);
  });

  it('should get vehicle subtype enum', (done: DoneFn) => {
    service.vehicleSubtype$.subscribe(vehicleSubType => {
      expect(vehicleSubType)
        .withContext('emit vehicle sub type enum')
        .toEqual(testVehicleSubtypeEnum);

      done();
    });

    const vehicleSubTypeEnumRequest = httpTestingController.expectOne(
      '/api/telematica/enums/VehicleSubTypeEnum',
      'vehicle sub type enum request'
    );

    vehicleSubTypeEnumRequest.flush(testVehicleSubtypeEnum);
  });

  it('should create vehicle', (done: DoneFn) => {
    service
      .createVehicle(testNewVehicle)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const createVehicleRequest = httpTestingController.expectOne({
      method: 'POST',
      url: '/api/telematica/vehicle'
    }, 'create vehicle request');

    createVehicleRequest.flush(null);
  });

  it('should update vehicle', (done: DoneFn) => {
    service
      .updateVehicle(testNewVehicle)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const updateVehicleRequest = httpTestingController.expectOne({
      method: 'PUT',
      url: `/api/telematica/vehicle/${testNewVehicle.id}`
    }, 'update vehicle request');

    updateVehicleRequest.flush(null);
  });

  it('should delete vehicle', (done: DoneFn) => {
    service
      .deleteVehicle(testVehiclesDataSource[0].id)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const deleteVehicleRequest = httpTestingController.expectOne({
      method: 'DELETE',
      url: `/api/telematica/vehicle/${testNewVehicle.id}`
    }, 'delete vehicle request');

    deleteVehicleRequest.flush(null);
  });
});

export const testVehicleGroups: VehicleGroup[] = [
  {
    id: 1,
    name: 'Комбайны CLAAS'
  },
  {
    id: 2,
    name: 'Тракторы Кировцы'
  },
  {
    id: 3,
    name: 'Легковые автомобили'
  }
];

export const testFuels: Fuel[] = [
  {
    id: 1,
    name: 'Бензин'
  },
  {
    id: 2,
    name: 'Дизельное топливо'
  }
];

export const testVehicleTypeEnum: KeyValue<string, string>[] = [
  {
    key: 'Transport',
    value: 'Для перевозок'
  },
  {
    key: 'Agro',
    value: 'Для работы на полях'
  }
];

export const testVehicleSubtypeEnum: KeyValue<string, string>[] = [
  {
    key: 'Telehandler',
    value: 'Tелескопический погрузчик'
  },
  {
    key: 'Tanker',
    value: 'Бензовоз'
  },
  {
    key: 'Truck',
    value: 'Грузовой автомобиль'
  },
  {
    key: 'Other',
    value: 'Другой транспорт'
  },
  {
    key: 'Harvester',
    value: 'Комбайн'
  },
  {
    key: 'Car',
    value: 'Легковой автомобиль'
  },
  {
    key: 'Sprayer',
    value: 'Опрыскиватель'
  },
  {
    key: 'Tractor',
    value: 'Трактор'
  }
];

const groupId = testVehicleGroups[2].id.toString();
const fuelId = testFuels[0].id.toString();

export const testNewVehicle: NewVehicle = {
  id: 1,
  name: 'Toyota Tundra',
  make: 'Toyota',
  model: 'Tundra',
  manufacturingYear: 2015,
  vehicleGroupId: Number(groupId),
  type: testVehicleTypeEnum[0].key.toString(),
  subType: testVehicleSubtypeEnum[2].key.toString(),
  fuelTypeId: Number(fuelId),
  registrationNumber: '7777TT77',
  inventoryNumber: 'TT77',
  serialNumber: 'TT7777',
  trackerId: 1,
  description: 'Пикап с краном'
};

export const testVehicles: Vehicles = {
  vehicles: [
    {
      id: 1,
      name: 'Марьевка',
      type: testVehicleTypeEnum[1],
      make: 'CLAAS',
      model: 'Tucano 460',
      subType: testVehicleSubtypeEnum[4],
      fuelType: {
        id: testFuels[1].id,
        value: testFuels[1].name
      },
      manufacturingYear: 2022,
      registrationNumber: '1200AM63',
      inventoryNumber: 'С293823729',
      serialNumber: '202039293834',
      description: 'Марьевское',
      tracker: {
        id: 1,
        value: '18-07-2539'
      }
    },
    {
      id: 2,
      name: 'Легковая машина',
      type: testVehicleTypeEnum[0],
      vehicleGroup: {
        id: testVehicleGroups[2].id,
        value: testVehicleGroups[2].name
      },
      make: 'Ford',
      model: 'Focus',
      subType: testVehicleSubtypeEnum[5],
      fuelType: {
        id: testFuels[0].id,
        value: testFuels[0].name
      },
      manufacturingYear: 2019,
      registrationNumber: '1290AM63',
      inventoryNumber: 'FF800110350',
      serialNumber: '800110350305',
      description: 'Частное'
    },
    {
      id: 3,
      name: 'Кировец',
      type: testVehicleTypeEnum[1],
      vehicleGroup: {
        id: testVehicleGroups[1].id,
        value: testVehicleGroups[1].name
      },
      make: 'Кировец',
      model: 'K-744',
      subType: testVehicleSubtypeEnum[7],
      fuelType: {
        id: testFuels[1].id,
        value: testFuels[1].name
      },
      tracker: {
        id: 2,
        value: '18-07-2557'
      }
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};
