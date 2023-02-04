import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { Trackers, TrackersOptions, TrackersService, TrackersSortBy, TrackerTypeEnum } from './trackers.service';

import { SortDirection } from './shared/sort';

import { PAGE_NUM, PAGE_SIZE } from './shared/pagination';
import { testVehicles } from './vehicle.service.spec';

describe('TrackersService', () => {
  let httpTestingController: HttpTestingController;
  let service: TrackersService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(TrackersService);
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
        sortDirection: SortDirection.Acending
      })
      .subscribe(trackers => {
        expect(trackers)
          .withContext('get trackers sorted by name')
          .toEqual(testTrackers);
      });

    const URL = '/api/telematica/trackers';

    let trackersRequest = httpTestingController.expectOne(
      `${URL}?pageNum=${PAGE_NUM}&pageSize=${PAGE_SIZE}&sortBy=${TrackersSortBy.Name}&sortDirection=${SortDirection.Acending}`,
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
      'descendingly sorted by start date trackers request'
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
});

export const testTrackers: Trackers = {
  trackers: [
    {
      id: 1,
      externalId: 12,
      name: 'Galileo Sky',
      simNumber: '79128371270',
      imei: '497890037671157',
      trackerType: {
        key: TrackerTypeEnum.GalileoSkyV50,
        value: 'GalileoSky v.5.0'
      },
      startDate: new Date('2022-11-07T10:00:00.000Z'),
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
      imei: '010894332966088',
      trackerType: {
        key: TrackerTypeEnum.Retranslator,
        value: 'Ретранслятор'
      },
      startDate: new Date('2023-02-01T07:20:39.617Z'),
      description: 'Ретранслятор'
    },
    {
      id: 3,
      externalId: 7,
      name: 'Wialon IPS',
      simNumber: '72347732931',
      imei: '527111404753054',
      trackerType: {
        key: TrackerTypeEnum.WialonIPS,
        value: 'Wialon IPS'
      },
      startDate: new Date('2023-01-21T17:25:19.512Z'),
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
