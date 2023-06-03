import { TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  NewTracker,
  TRACKER_PARAMETERS_HISTORY_PAGE_SIZE,
  Tracker,
  TrackerCommand,
  TrackerCommandResponse,
  TrackerCommandTransport,
  TrackerParameter,
  TrackerParameterName,
  TrackerParametersHistory,
  TrackerParametersHistoryOptions,
  TrackerService,
  TrackerStandardParameter,
  TrackerTypeEnum,
  Trackers,
  TrackersOptions,
  TrackersSortBy
} from './tracker.service';

import { SortDirection } from './shared/sort';

import { PAGE_NUM, PAGE_SIZE } from './shared/pagination';
import { testVehicles } from './vehicle.service.spec';

describe('TrackerService', () => {
  let httpTestingController: HttpTestingController;
  let service: TrackerService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(TrackerService);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should get trackers', (done: DoneFn) => {
    let trackersOptions: TrackersOptions | undefined;

    const subscription = service
      .getTrackers(trackersOptions)
      .subscribe(trackers => {
        expect(trackers)
          .withContext('get trackers without options')
          .toEqual(testTrackers);
      });

    let trackersRequest = httpTestingController.expectOne(
      `/api/telematica/trackers?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}`,
      'trackers request'
    );

    trackersRequest.flush(testTrackers);

    subscription.unsubscribe();

    trackersOptions = {};

    service
      .getTrackers(trackersOptions)
      .subscribe(trackers => {
        expect(trackers)
          .withContext('get trackers with blank options')
          .toEqual(testTrackers);

        done();
      });

    trackersRequest = httpTestingController.expectOne(
      `/api/telematica/trackers?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}`,
      'trackers request'
    );

    trackersRequest.flush(testTrackers);
  });

  it('should get sorted trackers', (done: DoneFn) => {
    let subscription = service
      .getTrackers({
        sortBy: TrackersSortBy.Name,
        sortDirection: SortDirection.Ascending
      })
      .subscribe(trackers => {
        expect(trackers)
          .withContext('get trackers sorted by name')
          .toEqual(testTrackers);
      });

    const URL = '/api/telematica/trackers';

    let trackersRequest = httpTestingController.expectOne(
      `${URL}?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}&sortBy=${TrackersSortBy.Name}&sortDirection=${SortDirection.Ascending}`,
      'sorted by name trackers request'
    );

    trackersRequest.flush(testTrackers);

    subscription.unsubscribe();

    subscription = service
      .getTrackers({
        sortBy: TrackersSortBy.Start,
        sortDirection: SortDirection.Descending
      })
      .subscribe(trackers => {
        expect(trackers)
          .withContext('get trackers sorted by fuel in descending direction')
          .toEqual(testTrackers);
      });

    trackersRequest = httpTestingController.expectOne(
      `${URL}?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}&sortBy=${TrackersSortBy.Start}&sortDirection=${SortDirection.Descending}`,
      'sorted in descending order by start date trackers request'
    );

    trackersRequest.flush(testTrackers);

    subscription.unsubscribe();

    service
      .getTrackers({
        sortBy: TrackersSortBy.Name
      })
      .subscribe(trackers => {
        expect(trackers)
          .withContext('get unsorted trackers with missing sort direction')
          .toEqual(testTrackers);

        done();
      });

    trackersRequest = httpTestingController.expectOne(
      `/api/telematica/trackers?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}`,
      'unsorted trackers request'
    );

    trackersRequest.flush(testTrackers);
  });

  it('should get tracker type enum', (done: DoneFn) => {
    const httpTestingController = TestBed.inject(HttpTestingController);

    service.trackerType$.subscribe(trackerType => {
      expect(trackerType)
        .withContext('emit tracker type enum')
        .toEqual(testTrackerTypeEnum);

      done();
    });

    const trackerTypeEnumRequest = httpTestingController.expectOne('/api/telematica/enums/trackertypeenum', 'tracker type enum request');

    trackerTypeEnumRequest.flush(testTrackerTypeEnum);
  });

  it('should create tracker', (done: DoneFn) => {
    const { id, ...tracker } = testNewTracker;

    service
      .createTracker(tracker)
      .subscribe(tracker => {
        expect(tracker)
          .withContext('emit new tracker')
          .toBe(testTracker);

        done();
      });

    const createTrackerRequest = httpTestingController.expectOne({
      method: 'POST',
      url: '/api/telematica/tracker'
    }, 'create tracker request');

    expect(createTrackerRequest.request.body)
      .withContext('valid request body')
      .toBe(tracker);

    createTrackerRequest.flush(testTracker);
  });

  it('should update tracker', (done: DoneFn) => {
    service
      .updateTracker(testNewTracker)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const updateTrackerRequest = httpTestingController.expectOne({
      method: 'PUT',
      url: `/api/telematica/tracker/${testNewTracker.id}`
    }, 'update tracker request');

    expect(updateTrackerRequest.request.body)
      .withContext('valid request body')
      .toBe(testNewTracker);

    updateTrackerRequest.flush(null);
  });

  it('should delete tracker', (done: DoneFn) => {
    service
      .deleteTracker(testTracker.id)
      .subscribe(response => {
        expect(response)
          .withContext('emit response')
          .toBeNull();

        done();
      });

    const deleteTrackerRequest = httpTestingController.expectOne({
      method: 'DELETE',
      url: `/api/telematica/tracker/${testTracker.id}`
    }, 'delete tracker request');

    deleteTrackerRequest.flush(null);
  });

  it('should send tracker command', (done: DoneFn) => {
    service
      .sendTrackerCommand(testTrackers.trackers[0].id, testTrackerCommand)
      .subscribe(response => {
        expect(response)
          .withContext('emit tracker command response')
          .toBe(testTrackerCommandResponse);

        done();
      });

    const trackerCommandRequest = httpTestingController.expectOne({
      method: 'POST',
      url: `/api/telematica/tracker-command/${testNewTracker.id}`
    }, 'tracker command request');

    expect(trackerCommandRequest.request.body)
      .withContext('valid request body')
      .toBe(testTrackerCommand);

    trackerCommandRequest.flush(testTrackerCommandResponse);
  });

  it('should get standard parameters', (done: DoneFn) => {
    service
      .getStandardParameters(testTrackers.trackers[0].id)
      .subscribe(standardParameters => {
        expect(standardParameters)
          .withContext('emit standard parameters')
          .toBe(testStandardParameters);

        done();
      });

    const standardParametersRequest = httpTestingController.expectOne(
      `/api/telematica/tracker/standard-parameters/${testNewTracker.id}`,
      'standard parameters request'
    );

    standardParametersRequest.flush(testStandardParameters);
  });

  it('should get parameters', (done: DoneFn) => {
    service
      .getParameters(testTrackers.trackers[0].id)
      .subscribe(parameters => {
        expect(parameters)
          .withContext('emit parameters')
          .toBe(testParameters);

        done();
      });

    const parametersRequest = httpTestingController.expectOne(
      `/api/telematica/tracker/parameters/${testNewTracker.id}`,
      'parameters request'
    );

    parametersRequest.flush(testParameters);
  });

  it('should get parameters history', (done: DoneFn) => {
    let testTrackerParametersHistoryOptions: TrackerParametersHistoryOptions | undefined;

    let subscription = service
      .getParametersHistory(testTrackerParametersHistoryOptions)
      .subscribe(parametersHistory => {
        expect(parametersHistory)
          .withContext('get tracker parameters history without options')
          .toEqual(testParametersHistory);
      });

    let parametersHistoryRequest = httpTestingController.expectOne(
      `/api/telematica/tracker/history?pageNum=${PAGE_NUM}&pageSize=${TRACKER_PARAMETERS_HISTORY_PAGE_SIZE}`,
      'parameters history request'
    );

    parametersHistoryRequest.flush(testParametersHistory);

    subscription.unsubscribe();

    testTrackerParametersHistoryOptions = {};

    subscription = service
      .getParametersHistory(testTrackerParametersHistoryOptions)
      .subscribe(parametersHistory => {
        expect(parametersHistory)
          .withContext('get tracker parameters history with blank options')
          .toEqual(testParametersHistory);
      });

    parametersHistoryRequest = httpTestingController.expectOne(
      `/api/telematica/tracker/history?pageNum=${PAGE_NUM}&pageSize=${TRACKER_PARAMETERS_HISTORY_PAGE_SIZE}`,
      'parameters history request'
    );

    parametersHistoryRequest.flush(testParametersHistory);

    subscription.unsubscribe();

    testTrackerParametersHistoryOptions = {
      trackerId: TEST_TRACKER_ID
    };

    service.getParametersHistory(testTrackerParametersHistoryOptions)
      .subscribe(parametersHistory => {
        expect(parametersHistory)
          .withContext('get tracker parameters history with tracker ID')
          .toEqual(testParametersHistory);

        done();
      });

    parametersHistoryRequest = httpTestingController.expectOne(
      // eslint-disable-next-line max-len
      `/api/telematica/tracker/history?pageNum=${PAGE_NUM}&pageSize=${TRACKER_PARAMETERS_HISTORY_PAGE_SIZE}&trackerId=${testTrackerParametersHistoryOptions.trackerId}`,
      'parameters history request'
    );

    parametersHistoryRequest.flush(testParametersHistory);
  });
});

export const testTrackerTypeEnum: KeyValue<TrackerTypeEnum, string>[] = [
  {
    key: TrackerTypeEnum.GalileoSkyV50,
    value: 'Протокол GalileoSkyV50'
  },
  {
    key: TrackerTypeEnum.Retranslator,
    value: 'Протокол Wialon Retranslator'
  },
  {
    key: TrackerTypeEnum.WialonIPS,
    value: 'Протокол Wialon'
  }
];

export const TEST_TRACKER_ID = 1;

export const testNewTracker: NewTracker = {
  id: 1,
  externalId: 102,
  name: 'Galileo Sky v 5.0',
  simNumber: '+78462777727',
  imei: '542553718824116',
  trackerType: testTrackerTypeEnum[0].key,
  startDate: '2023-02-18T22:57:19.446Z',
  description: 'GPS Ford Focus'
};

export const testTracker: Tracker = {
  id: testNewTracker.id!,
  externalId: testNewTracker.externalId,
  name: testNewTracker.name,
  simNumber: testNewTracker.simNumber,
  imei: testNewTracker.imei,
  trackerType: testTrackerTypeEnum[0],
  startDate: testNewTracker.startDate!,
  description: testNewTracker.description
};

export const testTrackers: Trackers = {
  trackers: [
    {
      id: 1,
      externalId: 12,
      name: 'Galileo Sky',
      simNumber: '+79128371270',
      imei: '497890037671157',
      trackerType: testTrackerTypeEnum[0],
      startDate: '2022-11-07T10:00:00.000Z',
      description: 'GPS комбайна',
      vehicle: {
        key: testVehicles.vehicles[2].id,
        value: testVehicles.vehicles[2].name
      }
    },
    {
      id: 2,
      externalId: 101,
      name: 'Передатчик уборки зерна',
      simNumber: '+79705501161',
      imei: '010894332966088',
      trackerType: testTrackerTypeEnum[1],
      startDate: '2023-02-01T07:20:39.617Z',
      description: 'Ретранслятор'
    },
    {
      id: 3,
      externalId: 7,
      name: 'Wialon IPS',
      simNumber: '+72347732931',
      imei: '527111404753054',
      trackerType: testTrackerTypeEnum[2],
      startDate: '2023-01-21T17:25:19.512Z',
      vehicle: {
        key: testVehicles.vehicles[0].id,
        value: testVehicles.vehicles[0].name
      }
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};

export const testTrackerCommand: TrackerCommand = {
  commandText: 'reset',
  transport: TrackerCommandTransport.SMS
};

export const testTrackerCommandResponse: TrackerCommandResponse = {
  commandResponse: 'Reset of device. Please wait 15 seconds…'
};

export const testStandardParameters: TrackerStandardParameter[] = [
  {
    name: 'Время',
    paramName: TrackerParameterName.Time,
    lastValueDateTime: '2023-03-05T04:39:12.318Z'
  },
  {
    name: 'Широта',
    paramName: TrackerParameterName.Latitude,
    lastValueDecimal: 78.5801259148088
  },
  {
    name: 'Долгота',
    paramName: TrackerParameterName.Longitude,
    lastValueDecimal: -110.63479717264961
  },
  {
    name: 'Высота',
    paramName: TrackerParameterName.Altitude,
    lastValueDecimal: 86.87415885961714
  },
  {
    name: 'Скорость',
    paramName: TrackerParameterName.Speed,
    lastValueDecimal: 7.676381634986047
  }
];

export const testParameters: TrackerParameter[] = [
  {
    paramName: 'acc1',
    lastValueDecimal: 0
  },
  {
    paramName: 'alarm_codeN',
    lastValueString: 'warn'
  },
  {
    paramName: 'blast_air_tempN',
    lastValueDecimal: 86.72323119282430
  },
  {
    paramName: 'ds1923_humidityX',
    lastValueDecimal: 35.13960513159938
  },
  {
    paramName: 'gsm_status',
    lastValueString: 'available'
  },
  {
    paramName: TrackerParameterName.Time,
    lastValueDateTime: '2023-03-16T09:14:36.422Z'
  }
];

export const testParametersHistory: TrackerParametersHistory = {
  parameters: [
    {
      time: '2023-03-23T01:38:11.880998Z',
      latitude: -77.33591583905098,
      longitude: -89.8236521368749,
      altitude: -47.53263511409112,
      speed: 16.100714299456435,
      parameters: 'adc3=329,soft=939,RS485[0]=497,adc4=467,pwr_ext=584,tracker_date=03/23/2023 01:38:11,CAN8BITR4=234,'
    },
    {
      time: '2023-03-23T01:38:11.874915Z',
      latitude: -16.77493846512141,
      longitude: -42.85802677162167,
      altitude: 59.12587789103651,
      parameters: 'rec_sn=857altitude196,term_version=-94,'
    },
    {
      time: '2023-03-23T01:38:11.863854Z',
      parameters: 'Port_4=740,adc4=629,pwr_int=303,'
    }
  ],
  pagination: {
    pageIndex: 1,
    total: 1
  }
};
