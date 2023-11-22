import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { MessageService, MessageStatistics, MessageStatisticsOptions } from './message.service';

import { DataMessageParameter, MessageType } from './messages.component';

import { MonitoringVehiclesOptions } from '../tech/tech.service';

import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from '../tech/tech.service.spec';

describe('MessageService', () => {
  let httpTestingController: HttpTestingController;
  let service: MessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(MessageService);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should get vehicles', (done: DoneFn) => {
    service
      .getVehicles()
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles')
          .toEqual(testMonitoringVehicles);

        done();
      });

    const vehiclesRequest = httpTestingController.expectOne('/api/telematica/messagesview/vehicles', 'vehicles request');

    vehiclesRequest.flush(testMonitoringVehicles);

    httpTestingController.verify();
  });

  it('should get vehicles', (done: DoneFn) => {
    let vehiclesOptions: MonitoringVehiclesOptions | undefined;

    const subscription = service
      .getVehicles(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles without options')
          .toEqual(testMonitoringVehicles);
      });

    let vehiclesRequest = httpTestingController.expectOne('/api/telematica/messagesview/vehicles', 'vehicles request');

    vehiclesRequest.flush(testMonitoringVehicles);

    subscription.unsubscribe();

    vehiclesOptions = {};

    service
      .getVehicles(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles with blank options')
          .toEqual(testMonitoringVehicles);

        done();
      });

    vehiclesRequest = httpTestingController.expectOne('/api/telematica/messagesview/vehicles', 'vehicles request');

    vehiclesRequest.flush(testMonitoringVehicles);
  });

  it('should get found vehicles', (done: DoneFn) => {
    const vehiclesOptions: MonitoringVehiclesOptions = {
      findCriterion: testFindCriterion.toLocaleLowerCase()
    };

    const testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles();

    service
      .getVehicles(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get found vehicles')
          .toEqual(testFoundMonitoringVehicles);

        done();
      });

    const vehiclesRequest = httpTestingController.expectOne(
      `/api/telematica/messagesview/vehicles?findCriterion=${encodeURIComponent(vehiclesOptions.findCriterion!)
      }`,
      'find vehicles request'
    );

    vehiclesRequest.flush(testFoundMonitoringVehicles);
  });

  it('should get message statistics', (done: DoneFn) => {
    const testMessageStatisticsOptions: MessageStatisticsOptions = {
      vehicleId: testMonitoringVehicles[0].id,
      periodStart: '2023-05-01T20:00:00.000Z',
      periodEnd: '2023-05-17T19:59:59.999Z',
      viewMessageType: MessageType.DataMessage,
      parameterType: DataMessageParameter.TrackerData
    };

    const testMessageStatistics: MessageStatistics = {
      numberOfMessages: 13,
      totalTime: 40,
      distance: 245,
      mileage: 8895,
      averageSpeed: 25.0,
      maxSpeed: 90.0
    };

    service
      .getStatistics(testMessageStatisticsOptions)
      .subscribe(statistics => {
        expect(statistics)
          .withContext('get statistics')
          .toEqual(testMessageStatistics);

        done();
      });

    const statisticsRequest = httpTestingController.expectOne(
      `/api/telematica/messagesview/statistics?vehicleId=${testMessageStatisticsOptions.vehicleId}&periodStart=${
        testMessageStatisticsOptions.periodStart
      }&periodEnd=${testMessageStatisticsOptions.periodEnd}&viewMessageType=${testMessageStatisticsOptions.viewMessageType}&parameterType=${
        testMessageStatisticsOptions.parameterType!
      }`,
      'statistics request'
    );

    statisticsRequest.flush(testMessageStatistics);
  });
});
