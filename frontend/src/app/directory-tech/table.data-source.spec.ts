import { firstValueFrom } from 'rxjs';

import { TableDataSource } from './table.data-source';

import { VehicleDataSource } from './vehicles/vehicles.component';

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
    type: {
      key: 'Transport',
      value: 'Для перевозок'
    },
    subtype: {
      key: 'Harvester',
      value: 'Комбайн'
    },
    group: {
      id: 1,
      name: 'Группа 1'
    },
    year: 2022,
    fuel: {
      id: 1,
      name: 'Бензин'
    },
    registration: '1200 AM 63',
    inventory: 'С293823729',
    serial: '202039293834',
    description: 'Марьевское'
  },
  {
    id: 2,
    name: 'Легковая машина',
    make: 'Ford',
    model: 'Focus',
    type: {
      key: 'Transport',
      value: 'Для перевозок'
    },
    subtype: {
      key: 'Car',
      value: 'Легковой автомобиль'
    },
    group: {
      id: 2,
      name: 'Группа 2'
    },
    year: 2019,
    fuel: {
      id: 2,
      name: 'Дизельное топливо'
    },
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
    type: {
      key: 'Transport',
      value: 'Для перевозок'
    },
    subtype: {
      key: 'Tractor',
      value: 'Трактор'
    },
    group: {
      id: 1,
      name: 'Группа 1'
    },
    year: 2017,
    fuel: {
      id: 1,
      name: 'Бензин'
    },
    registration: '1202 AК 63',
    inventory: 'М465890560',
    serial: '678896767968',
    description: 'Кировское'
  }
];
