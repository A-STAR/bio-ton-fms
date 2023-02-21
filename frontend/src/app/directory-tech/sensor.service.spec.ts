import { TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  SensorDataTypeEnum,
  SensorGroup,
  Sensors,
  SensorService,
  SensorsOptions,
  SensorType,
  Unit,
  ValidationTypeEnum
} from './sensor.service';

import { PAGE_NUM, PAGE_SIZE } from './shared/pagination';

describe('SensorService', () => {
  let httpTestingController: HttpTestingController;
  let service: SensorService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(SensorService);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should get tracker sensors', (done: DoneFn) => {
    let testSensorsOptions: SensorsOptions | undefined;

    let subscription = service
      .getSensors(testSensorsOptions)
      .subscribe(sensors => {
        expect(sensors)
          .withContext('get tracker sensors without options')
          .toEqual(testSensors);
      });

    let sensorsRequest = httpTestingController.expectOne(
      `/api/telematica/sensors?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}`,
      'sensors request'
    );

    sensorsRequest.flush(testSensors);

    subscription.unsubscribe();

    testSensorsOptions = {};

    subscription = service
      .getSensors(testSensorsOptions)
      .subscribe(sensors => {
        expect(sensors)
          .withContext('get tracker sensors with blank options')
          .toEqual(testSensors);
      });

    sensorsRequest = httpTestingController.expectOne(
      `/api/telematica/sensors?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}`,
      'sensors request'
    );

    sensorsRequest.flush(testSensors);

    subscription.unsubscribe();

    testSensorsOptions = {
      trackerId: TEST_TRACKER_ID
    };

    const testTrackerSensors: Sensors = {
      sensors: testSensors.sensors.filter(({ tracker }) => tracker.key === testSensorsOptions?.trackerId),
      pagination: testSensors.pagination
    };

    service.getSensors(testSensorsOptions)
      .subscribe(sensors => {
        expect(sensors)
          .withContext('get tracker sensors with tracker ID')
          .toEqual(testTrackerSensors);

        done();
      });

    sensorsRequest = httpTestingController.expectOne(
      `/api/telematica/sensors?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}&trackerId=${testSensorsOptions.trackerId}`,
      'sensors request'
    );

    sensorsRequest.flush(testTrackerSensors);
  });

  it('should get sensor groups', (done: DoneFn) => {
    service.sensorGroups$.subscribe(groups => {
      expect(groups)
        .withContext('emit sensor groups')
        .toEqual(testSensorGroups);

      done();
    });

    const sensorGroupsRequest = httpTestingController.expectOne('/api/telematica/sensorGroups', 'sensor groups request');

    sensorGroupsRequest.flush(testSensorGroups);
  });

  it('should get sensor types', (done: DoneFn) => {
    service.sensorTypes$.subscribe(sensorTypes => {
      expect(sensorTypes)
        .withContext('emit sensor types')
        .toEqual(testSensorTypes);

      done();
    });

    const sensorTypesRequest = httpTestingController.expectOne('/api/telematica/sensorTypes', 'sensor types request');

    sensorTypesRequest.flush(testSensorTypes);
  });

  it('should get units', (done: DoneFn) => {
    service.units$.subscribe(units => {
      expect(units)
        .withContext('emit units')
        .toEqual(testUnits);

      done();
    });

    const unitsRequest = httpTestingController.expectOne('/api/telematica/units', 'units request');

    unitsRequest.flush(testUnits);
  });

  it('should get sensor data type enum', (done: DoneFn) => {
    service.sensorDataType$.subscribe(sensorDataType => {
      expect(sensorDataType)
        .withContext('emit sensor data type enum')
        .toEqual(testSensorDataTypeEnum);

      done();
    });

    const sensorDataTypeRequest = httpTestingController.expectOne(
      '/api/telematica/enums/sensordatatypeenum',
      'sensor data type enum request'
    );

    sensorDataTypeRequest.flush(testSensorDataTypeEnum);
  });

  it('should get validation type enum', (done: DoneFn) => {
    service.validationType$.subscribe(validationType => {
      expect(validationType)
        .withContext('emit validation type enum')
        .toEqual(testValidationTypeEnum);

      done();
    });

    const validationTypeRequest = httpTestingController.expectOne(
      '/api/telematica/enums/validationtypeenum',
      'validation type enum request'
    );

    validationTypeRequest.flush(testValidationTypeEnum);
  });
});

export const testSensorDataTypeEnum: KeyValue<SensorDataTypeEnum, string>[] = [
  {
    key: SensorDataTypeEnum.Boolean,
    value: 'Булево'
  },
  {
    key: SensorDataTypeEnum.String,
    value: 'Строка'
  },
  {
    key: SensorDataTypeEnum.Number,
    value: 'Число'
  }
];

export const testValidationTypeEnum: KeyValue<ValidationTypeEnum, string>[] = [
  {
    key: ValidationTypeEnum.LogicalAnd,
    value: 'Тип валидации «Логическое И»'
  },
  {
    key: ValidationTypeEnum.LogicalOr,
    value: 'Тип валидации «Логическое ИЛИ»'
  },
  {
    key: ValidationTypeEnum.ZeroTest,
    value: 'Тип валидации «Проверка на неравенство нулю»'
  }
];

export const testUnits: Unit[] = [
  {
    id: 1,
    name: 'Безразмерная величина'
  },
  {
    id: 2,
    name: 'Километры',
    abbreviated: 'км'
  },
  {
    id: 3,
    name: 'Градусы цельсия',
    abbreviated: 'C°'
  },
  {
    id: 4,
    name: 'Обороты в минуту',
    abbreviated: 'об/мин'
  }
];

export const testSensorTypes: SensorType[] = [
  {
    id: 1,
    name: 'Датчик пробега',
    description: 'Датчик, показывающий пройденное объектом расстояние.',
    sensorGroup: {
      key: 1,
      value: 'Пробег'
    },
    dataType: SensorDataTypeEnum.Number,
    unit: {
      key: testUnits[1].id,
      value: testUnits[1].name
    }
  },
  {
    id: 2,
    name: 'Относительный одометр',
    description: 'Датчик, показывающий расстояние, пройденное объектом.',
    sensorGroup: {
      key: 1,
      value: 'Пробег'
    },
    dataType: SensorDataTypeEnum.Number,
    unit: {
      key: testUnits[1].id,
      value: testUnits[1].name
    }
  },
  {
    id: 3,
    name: 'Датчик зажигания',
    description: 'Датчик, показывающий, включено или выключено зажигание.',
    sensorGroup: {
      key: 2,
      value: 'Цифровые'
    },
    dataType: SensorDataTypeEnum.Boolean,
    unit: {
      key: testUnits[0].id,
      value: testUnits[0].name
    }
  },
  {
    id: 4,
    name: 'Тревожная кнопка',
    description: 'Датчик, ненулевое значение которого позволяет отмечать сообщение как тревожное (SOS).',
    sensorGroup: {
      key: 2,
      value: 'Цифровые'
    },
    dataType: SensorDataTypeEnum.Number,
    unit: {
      key: testUnits[0].id,
      value: testUnits[0].name
    }
  },
  {
    id: 5,
    name: 'Датчик мгновенного определения движения',
    description: 'Датчик, определяющий состояние движения объектов в реальном времени.',
    sensorGroup: {
      key: 2,
      value: 'Цифровые'
    },
    dataType: SensorDataTypeEnum.Boolean,
    unit: {
      key: testUnits[0].id,
      value: testUnits[0].name
    }
  }
];

export const testSensorGroups: SensorGroup[] = [
  {
    id: 1,
    name: 'Пробег',
    description: 'Группа пробега',
    sensorTypes: [
      {
        id: testSensorTypes[0].id,
        name: testSensorTypes[0].name,
        description: testSensorTypes[0].description,
        sensorGroupId: testSensorTypes[0].sensorGroup.key,
        dataType: testSensorTypes[0].dataType,
        unitId: testSensorTypes[0].unit.key
      },
      {
        id: testSensorTypes[1].id,
        name: testSensorTypes[1].name,
        description: testSensorTypes[1].description,
        sensorGroupId: testSensorTypes[1].sensorGroup.key,
        dataType: testSensorTypes[1].dataType,
        unitId: testSensorTypes[1].unit.key
      },
    ]
  },
  {
    id: 2,
    name: 'Цифровые',
    sensorTypes: [
      {
        id: testSensorTypes[2].id,
        name: testSensorTypes[2].name,
        description: testSensorTypes[2].description,
        sensorGroupId: testSensorTypes[2].sensorGroup.key,
        dataType: testSensorTypes[2].dataType,
        unitId: testSensorTypes[2].unit.key
      },
      {
        id: testSensorTypes[3].id,
        name: testSensorTypes[3].name,
        description: testSensorTypes[3].description,
        sensorGroupId: testSensorTypes[3].sensorGroup.key,
        dataType: testSensorTypes[3].dataType,
        unitId: testSensorTypes[3].unit.key
      },
      {
        id: testSensorTypes[4].id,
        name: testSensorTypes[4].name,
        description: testSensorTypes[4].description,
        sensorGroupId: testSensorTypes[4].sensorGroup.key,
        dataType: testSensorTypes[4].dataType,
        unitId: testSensorTypes[4].unit.key
      }
    ]
  },
  {
    id: 3,
    name: 'Топливо',
    description: 'Группа топлива'
  }
];

export const testSensors: Sensors = {
  sensors: [
    {
      id: 1,
      tracker: {
        key: 1,
        value: 'Трекер Arnavi'
      },
      name: 'Пробег',
      visibility: true,
      dataType: SensorDataTypeEnum.String,
      sensorType: {
        key: 2,
        value: 'Датчик пробега'
      },
      description: 'Умный цифровой датчик для пробега',
      formula: '(param1-#param1)/const2',
      unit: {
        key: 3,
        value: 'км/ч'
      },
      useLastReceived: true,
      validator: {
        key: 2,
        value: 'Валидатор пробега'
      },
      fuelUse: 8
    },
    {
      id: 2,
      tracker: {
        key: 1,
        value: 'Трекер TKSTAR'
      },
      name: 'Разгон',
      visibility: false,
      dataType: SensorDataTypeEnum.Boolean,
      sensorType: {
        key: 2,
        value: 'Датчик разгона'
      },
      formula: '(param1+#param1)*const2',
      unit: {
        key: 2,
        value: 'мАч'
      },
      useLastReceived: false,
      validator: {
        key: 1,
        value: 'Валидатор разгона'
      },
      validationType: ValidationTypeEnum.ZeroTest
    },
    {
      id: 3,
      tracker: {
        key: 2,
        value: 'Трекер Micodus MV720'
      },
      name: 'Скорость',
      visibility: true,
      dataType: SensorDataTypeEnum.Number,
      sensorType: {
        key: 1,
        value: 'Датчик скорости'
      },
      description: 'Оповещает о превышении скорости',
      unit: {
        key: 1,
        value: 'мАч'
      },
      useLastReceived: true,
      fuelUse: 7
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};

export const TEST_TRACKER_ID = 1;
