import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { SensorDataTypeEnum, Sensors, SensorService, SensorsOptions, ValidationTypeEnum } from './sensor.service';

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

    httpTestingController.verify();
  });
});

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
      validationType: ValidationTypeEnum.LogicalAnd,
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
      validationType: ValidationTypeEnum.ZeroTest,
      fuelUse: 6
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
      validator: {
        key: 1,
        value: 'Валидатор скорости'
      },
      validationType: ValidationTypeEnum.LogicalOr,
      fuelUse: 7
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};

export const TEST_TRACKER_ID = 1;
