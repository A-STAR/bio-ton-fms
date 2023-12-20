import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  MessagesOptions,
  MessageService,
  MessageStatistics,
  MessageStatisticsOptions,
  MessageTrackOptions,
  Messages
} from './message.service';

import { DataMessageParameter, MessageType } from './messages.component';

import { LocationAndTrackResponse, MonitoringVehiclesOptions } from '../tech/tech.service';

import { PAGE_NUM, PAGE_SIZE, Pagination } from '../directory-tech/shared/pagination';

import {
  mockTestFoundMonitoringVehicles,
  testFindCriterion,
  testLocationAndTrackResponse,
  testMonitoringVehicles,
  testVehicleMonitoringInfo
} from '../tech/tech.service.spec';

import { TrackerCommandTransport } from '../directory-tech/tracker.service';

import { testParameters } from '../directory-tech/tracker.service.spec';

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

  it('should get messages', (done: DoneFn) => {
    const options: MessagesOptions = { ...testMessageOptions };

    service
      .getMessages(options)
      .subscribe(messages => {
        expect(messages)
          .withContext('get messages')
          .toEqual(testTrackerMessages);

        done();
      });

    const messagesRequest = httpTestingController.expectOne(
      `/api/telematica/messagesview/list?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}&vehicleId=${options.vehicleId
      }&periodStart=${options.periodStart}&periodEnd=${options.periodEnd}&viewMessageType=${options.viewMessageType
      }&parameterType=${options.parameterType!}`,
      'messages request'
    );

    messagesRequest.flush(testTrackerMessages);

    /* Coverage for page size defaults. */

    options.pageNum = PAGE_NUM;

    service.getMessages(options);
  });

  it('should get message statistics', (done: DoneFn) => {
    service
      .getStatistics(testMessageOptions)
      .subscribe(statistics => {
        expect(statistics)
          .withContext('get statistics')
          .toEqual(testMessageStatistics);

        done();
      });

    const statisticsRequest = httpTestingController.expectOne(
      `/api/telematica/messagesview/statistics?vehicleId=${testMessageOptions.vehicleId}&periodStart=${
        testMessageOptions.periodStart
      }&periodEnd=${testMessageOptions.periodEnd}&viewMessageType=${testMessageOptions.viewMessageType}&parameterType=${
        testMessageOptions.parameterType!
      }`,
      'statistics request'
    );

    statisticsRequest.flush(testMessageStatistics);
  });

  it('should get message track', (done: DoneFn) => {
    const testMessageTrackOptions: MessageTrackOptions = {
      vehicleId: testMessageOptions.vehicleId,
      periodStart: testMessageOptions.periodStart,
      periodEnd: testMessageOptions.periodEnd
    };

    service
      .getTrack(testMessageTrackOptions)
      .subscribe(track => {
        expect(track)
          .withContext('get track')
          .toEqual(testMessageLocationAndTrack);

        done();
      });

    const statisticsRequest = httpTestingController.expectOne(
      `/api/telematica/messagesview/track?vehicleId=${testMessageTrackOptions.vehicleId}&periodStart=${testMessageTrackOptions.periodStart
      }&periodEnd=${testMessageTrackOptions.periodEnd}`,
      'track request'
    );

    statisticsRequest.flush(testMessageLocationAndTrack);
  });
});

const testMessageOptions: MessagesOptions | MessageStatisticsOptions = {
  vehicleId: testMonitoringVehicles[0].id,
  periodStart: '2023-05-01T20:00:00.000Z',
  periodEnd: '2023-05-17T19:59:59.999Z',
  viewMessageType: MessageType.DataMessage,
  parameterType: DataMessageParameter.TrackerData
};

const pagination: Pagination = {
  pagination: {
    pageIndex: PAGE_NUM,
    total: 10
  }
};

export const testTrackerMessages: Messages = {
  trackerDataMessages: [
    {
      id: 1,
      num: 178,
      serverDateTime: '2023-12-08T21:52:00.273523+00:00',
      trackerDateTime: '2023-12-08T21:52:00.273523+00:00',
      speed: 66.0,
      latitude: 42.152,
      longitude: 12.13,
      satNumber: 2,
      altitude: 23.1,
      parameters: testParameters
    }
  ],
  ...pagination
};

export const testSensorMessages: Messages = {
  sensorDataMessages: [
    {
      id: 1,
      num: 1,
      serverDateTime: '2023-12-08T21:52:00.273523+00:00',
      trackerDateTime: '2023-12-08T21:52:00.273523+00:00',
      speed: 66.0,
      latitude: 42.152,
      longitude: 12.13,
      satNumber: 2,
      altitude: 23.1,
      sensors: testVehicleMonitoringInfo.trackerInfo.sensors!
    }
  ],
  ...pagination
};

export const testCommandMessages: Messages = {
  commandMessages:[
    {
      num: 1,
      commandDateTime: '2023-12-17T03:09:22.9649951Z',
      commandText: 'text 1',
      executionTime: '2023-12-17T03:09:22.9649951Z',
      channel: TrackerCommandTransport.SMS,
      commandResponseText: 'response 1'
    },
    {
      num: 2,
      commandDateTime: '2023-12-17T03:09:22.964995Z',
      commandText: 'text 2',
      executionTime: '2023-12-17T03:09:22.964995Z',
      channel: TrackerCommandTransport.TCP,
      commandResponseText: 'response 2'
    }
  ],
  ...pagination
};

export const testMessageLocationAndTrack: LocationAndTrackResponse = {
  ...testLocationAndTrackResponse,
  tracks: [testLocationAndTrackResponse.tracks[0]]
};

export const testMessageStatistics: MessageStatistics = {
  numberOfMessages: 13,
  totalTime: 40,
  distance: 245,
  mileage: 8895,
  averageSpeed: 25.0,
  maxSpeed: 90.0
};
