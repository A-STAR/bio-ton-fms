import { TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  NewSensor,
  Sensor,
  SensorDataTypeEnum,
  SensorGroup,
  SensorService,
  Sensors,
  SensorsOptions,
  Unit,
  ValidationTypeEnum
} from './sensor.service';

import { PAGE_NUM, PAGE_SIZE } from './shared/pagination';
import { TEST_TRACKER_ID } from './tracker.service.spec';

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
      sensors: testSensors.sensors.filter(({ tracker }) => tracker.id === testSensorsOptions?.trackerId),
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

    const sensorGroupsRequest = httpTestingController.expectOne('/api/telematica/sensorgroups', 'sensor groups request');

    sensorGroupsRequest.flush(testSensorGroups);
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

  it('should create sensor', (done: DoneFn) => {
    const { id, ...sensor } = testNewSensor;

    service
      .createSensor(sensor)
      .subscribe(sensor => {
        expect(sensor)
          .withContext('emit new sensor')
          .toBe(testSensor);

        done();
      });

    const createSensorRequest = httpTestingController.expectOne({
      method: 'POST',
      url: '/api/telematica/sensor'
    }, 'create sensor request');

    expect(createSensorRequest.request.body)
      .withContext('valid request body')
      .toBe(sensor);

    createSensorRequest.flush(testSensor);
  });

  it('should update sensor', (done: DoneFn) => {
    service
      .updateSensor(testNewSensor)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const updateSensorRequest = httpTestingController.expectOne({
      method: 'PUT',
      url: `/api/telematica/sensor/${testNewSensor.id}`
    }, 'update sensor request');

    expect(updateSensorRequest.request.body)
      .withContext('valid request body')
      .toBe(testNewSensor);

    updateSensorRequest.flush(null);
  });

  it('should delete tracker', (done: DoneFn) => {
    service
      .deleteSensor(testSensors.sensors[0].id)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const deleteTrackerRequest = httpTestingController.expectOne({
      method: 'DELETE',
      url: `/api/telematica/sensor/${testSensors.sensors[0].id}`
    }, 'delete tracker request');

    deleteTrackerRequest.flush(null);
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

export const testSensorGroups: SensorGroup[] = [
  {
    id: 1,
    name: 'Пробег',
    description: 'Группа пробега',
    sensorTypes: [
      {
        id: 1,
        name: 'Относительный одометр',
        description: 'Датчик, показывающий расстояние, пройденное объектом.',
        sensorGroupId: 1,
        dataType: SensorDataTypeEnum.Number,
        unitId: testUnits[1].id
      },
      {
        id: 2,
        name: 'Датчик пробега',
        description: 'Датчик, показывающий пройденное объектом расстояние.',
        sensorGroupId: 1,
        dataType: SensorDataTypeEnum.Number,
        unitId: testUnits[1].id
      }
    ]
  },
  {
    id: 2,
    name: 'Цифровые',
    sensorTypes: [
      {
        id: 3,
        name: 'Датчик зажигания',
        description: 'Датчик, показывающий, включено или выключено зажигание.',
        sensorGroupId: 2,
        dataType: SensorDataTypeEnum.Boolean,
        unitId: testUnits[0].id
      },
      {
        id: 4,
        name: 'Тревожная кнопка',
        description: 'Датчик, ненулевое значение которого позволяет отмечать сообщение как тревожное (SOS).',
        sensorGroupId: 2,
        dataType: SensorDataTypeEnum.Number,
        unitId: testUnits[0].id
      },
      {
        id: 5,
        name: 'Датчик мгновенного определения движения',
        description: 'Датчик, определяющий состояние движения объектов в реальном времени.',
        sensorGroupId: 2,
        dataType: SensorDataTypeEnum.Boolean,
        unitId: testUnits[0].id
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
        id: 1,
        value: 'Трекер Arnavi'
      },
      name: 'Пробег',
      visibility: true,
      dataType: SensorDataTypeEnum.String,
      sensorType: {
        id: 2,
        value: 'Датчик пробега'
      },
      description: 'Умный цифровой датчик для пробега',
      formula: '(param1 - #param1) / const2',
      unit: {
        id: 3,
        value: 'км/ч'
      },
      useLastReceived: true,
      validator: {
        id: 2,
        value: 'Разгон'
      },
      fuelUse: 8,
      startTimeout: 5,
      fixErrors: true,
      fuelUseCalculation: true,
      fuelUseTimeCalculation: true
    },
    {
      id: 2,
      tracker: {
        id: 1,
        value: 'Трекер TKSTAR'
      },
      name: 'Разгон',
      visibility: false,
      dataType: SensorDataTypeEnum.Boolean,
      sensorType: {
        id: 2,
        value: 'Датчик разгона'
      },
      formula: '(param1 + #param1) * const2',
      unit: {
        id: 2,
        value: 'мАч'
      },
      useLastReceived: false,
      validator: {
        id: 3,
        value: 'Скорость'
      },
      validationType: ValidationTypeEnum.ZeroTest,
      minRefueling: 10,
      refuelingTimeout: 3600,
      fullRefuelingTimeout: 300,
      refuelingLookup: true,
      refuelingCalculation: true,
      refuelingRawCalculation: false
    },
    {
      id: 3,
      tracker: {
        id: 2,
        value: 'Трекер Micodus MV720'
      },
      name: 'Скорость',
      visibility: true,
      dataType: SensorDataTypeEnum.Number,
      sensorType: {
        id: 1,
        value: 'Датчик скорости'
      },
      description: 'Оповещает о превышении скорости',
      unit: {
        id: 1,
        value: 'мАч'
      },
      useLastReceived: true,
      fuelUse: 7,
      minDrain: 2.75,
      drainTimeout: 1800,
      drainStopTimeout: 900,
      drainLookup: false,
      drainCalculation: false,
      drainRawCalculation: true
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};

export const testSensor: Sensor = {
  id: 1,
  tracker: {
    id: TEST_TRACKER_ID,
    value: 'Galileo Sky'
  },
  name: 'Парковочный радар',
  sensorType: {
    id: testSensorGroups[1].sensorTypes![0].id,
    value: testSensorGroups[1].sensorTypes![0].name
  },
  dataType: testSensorDataTypeEnum[0].key,
  formula: '(param1 + param2) * param3',
  unit: {
    id: testUnits[1].id,
    value: testUnits[1].name
  },
  validator: {
    id: testSensors.sensors[1].id,
    value: testSensors.sensors[1].name
  },
  validationType: testValidationTypeEnum[0].key,
  useLastReceived: false,
  visibility: false,
  fuelUse: 10,
  description: 'Устройство с отличным функционалом для парковки в плохих условиях видимости.'
};

export const testNewSensor: NewSensor = {
  id: testSensor.id,
  trackerId: testSensor.tracker.id,
  name: testSensor.name,
  sensorTypeId: testSensor.sensorType.id,
  dataType: testSensor.dataType,
  formula: testSensor.formula,
  unitId: testSensor.unit.id,
  validatorId: testSensors.sensors[1].id,
  validationType: testSensor.validationType,
  useLastReceived: testSensor.useLastReceived,
  visibility: testSensor.visibility,
  fuelUse: testSensor.fuelUse,
  description: testSensor.description
};
