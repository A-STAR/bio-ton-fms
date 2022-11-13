import { firstValueFrom } from 'rxjs';

import { TableDataSource } from './table.data-source';

import { VehicleDataSource } from './vehicles/vehicles.component';

import { testFuels, testVehicleGroups, testVehicleSubtypeEnum, testVehicleTypeEnum } from './vehicle.service.spec';

describe('TableDataSource', () => {
  let tableDataSource: TableDataSource<VehicleDataSource>;

  beforeEach(() => {
    tableDataSource = new TableDataSource<VehicleDataSource>(testDataSource);
  });

  it('should create an instance', () => {
    expect(tableDataSource)
      .toBeTruthy();
  });

  it('should connect', async () => {
    const connect$ = tableDataSource.connect();

    const vehiclesDataSource = await firstValueFrom(connect$);

    expect(vehiclesDataSource)
      .withContext('return data source')
      .toBe(testDataSource);
  });

  it('should disconnect', () => {
    spyOn(tableDataSource, 'setDataSource')
      .and.callThrough();

    tableDataSource.disconnect();

    expect(tableDataSource)
      .not.toHaveSpyInteractions();
  });
});

const testDataSource: VehicleDataSource[] = [
  {
    id: 1,
    name: 'Марьевка',
    make: 'CLAAS',
    model: 'Tucano 460',
    type: testVehicleTypeEnum[1].value,
    subtype: testVehicleSubtypeEnum[4].value,
    group: testVehicleGroups[0].name,
    year: 2022,
    fuel: testFuels[1].name,
    registration: '1200 AM 63',
    inventory: 'С293823729',
    serial: '202039293834',
    tracker: '18-07-2539',
    description: 'Марьевское'
  },
  {
    id: 2,
    name: 'Легковая машина',
    make: 'Ford',
    model: 'Focus',
    type: testVehicleTypeEnum[0].value,
    subtype: testVehicleSubtypeEnum[5].value,
    group: testVehicleGroups[2].name,
    year: 2019,
    fuel: testFuels[0].name,
    registration: '1290 AM 63',
    inventory: 'FF800110350',
    serial: '800110350305',
    description: 'Частное'
  },
  {
    id: 3,
    name: 'Кировец',
    make: 'Кировец',
    model: 'K-744',
    type: testVehicleTypeEnum[0].value,
    subtype: testVehicleSubtypeEnum[7].value,
    group: testVehicleGroups[1].name,
    year: 2017,
    fuel: testFuels[1].name,
    registration: '1202 AК 63',
    inventory: 'М465890560',
    serial: '678896767968',
    tracker: '18-07-2557',
    description: 'Кировское'
  }
];
