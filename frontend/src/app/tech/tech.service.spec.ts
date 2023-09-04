import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  ConnectionStatus,
  LocationAndTrackRequest,
  LocationAndTrackResponse,
  LocationOptions,
  MonitoringVehicle,
  MonitoringVehiclesOptions,
  MovementStatus,
  TechService,
  VehicleMonitoringInfo
} from './tech.service';

import { testVehicles } from '../directory-tech/vehicle.service.spec';
import { testParameters, testTracker, testTrackers } from '../directory-tech/tracker.service.spec';
import { testSensors } from '../directory-tech/sensor.service.spec';
import { SEARCH_MIN_LENGTH } from './tech.component';

describe('TechService', () => {
  let httpTestingController: HttpTestingController;
  let service: TechService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(TechService);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
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

    let vehiclesRequest = httpTestingController.expectOne('/api/telematica/monitoring/vehicles', 'vehicles request');

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

    vehiclesRequest = httpTestingController.expectOne('/api/telematica/monitoring/vehicles', 'vehicles request');

    vehiclesRequest.flush(testMonitoringVehicles);
  });

  it('should get vehicles location and track', (done: DoneFn) => {
    let vehiclesOptions: LocationOptions[] = testMonitoringVehicles.map(({ id }) => ({
      vehicleId: id
    }));

    let subscription = service
      .getVehiclesLocationAndTrack(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles location without track')
          .toEqual(testLocationAndTrackResponse);
      });

    let date = new Date();

    date.setHours(0, 0, 0, 0);

    let todayStart = date.toISOString();

    let vehiclesLocationAndTrackRequest = httpTestingController.expectOne(
      `/api/telematica/monitoring/locations-and-tracks?trackStartTime=${todayStart}`,
      'vehicles location and track request'
    );

    let body = vehiclesOptions.map<LocationAndTrackRequest>(({ vehicleId, needReturnTrack }) => ({
      vehicleId,
      needReturnTrack: needReturnTrack ?? false
    }));

    expect(vehiclesLocationAndTrackRequest.request.body)
      .withContext('valid request body')
      .toEqual(body);

    vehiclesLocationAndTrackRequest.flush(testLocationAndTrackResponse);

    subscription.unsubscribe();

    vehiclesOptions = vehiclesOptions.map(({ vehicleId }) => ({
      vehicleId,
      needReturnTrack: false
    }));

    subscription = service
      .getVehiclesLocationAndTrack(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles location without track')
          .toEqual(testLocationAndTrackResponse);
      });

    date = new Date();

    date.setHours(0, 0, 0, 0);

    todayStart = date.toISOString();

    vehiclesLocationAndTrackRequest = httpTestingController.expectOne(
      `/api/telematica/monitoring/locations-and-tracks?trackStartTime=${todayStart}`,
      'vehicles location and track request'
    );

    body = vehiclesOptions.map<LocationAndTrackRequest>(({ vehicleId, needReturnTrack }) => ({
      vehicleId,
      needReturnTrack: needReturnTrack ?? false
    }));

    expect(vehiclesLocationAndTrackRequest.request.body)
      .withContext('valid request body')
      .toEqual(body);

    vehiclesLocationAndTrackRequest.flush(testLocationAndTrackResponse);

    subscription.unsubscribe();

    vehiclesOptions = vehiclesOptions.map(({ vehicleId }) => ({
      vehicleId,
      needReturnTrack: true
    }));

    service
      .getVehiclesLocationAndTrack(vehiclesOptions)
      .subscribe(vehicles => {
        expect(vehicles)
          .withContext('get vehicles location with track')
          .toEqual(testLocationAndTrackResponse);

        done();
      });

    date = new Date();

    date.setHours(0, 0, 0, 0);

    todayStart = date.toISOString();

    vehiclesLocationAndTrackRequest = httpTestingController.expectOne(
      `/api/telematica/monitoring/locations-and-tracks?trackStartTime=${todayStart}`,
      'vehicles location and track request'
    );

    body = vehiclesOptions.map<LocationAndTrackRequest>(({ vehicleId, needReturnTrack }) => ({
      vehicleId,
      needReturnTrack: needReturnTrack ?? false
    }));

    expect(vehiclesLocationAndTrackRequest.request.body)
      .withContext('valid request body')
      .toEqual(body);

    vehiclesLocationAndTrackRequest.flush(testLocationAndTrackResponse);
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
      `/api/telematica/monitoring/vehicles?findCriterion=${encodeURIComponent(vehiclesOptions.findCriterion!)
      }`,
      'find vehicles request'
    );

    vehiclesRequest.flush(testFoundMonitoringVehicles);
  });

  it('should get vehicle info', (done: DoneFn) => {
    service
      .getVehicleInfo(testMonitoringVehicles[0].id)
      .subscribe(vehicle => {
        expect(vehicle)
          .withContext('emit vehicle')
          .toBe(testVehicleMonitoringInfo);

        done();
      });

    const vehicleRequest = httpTestingController.expectOne(
      `/api/telematica/monitoring/vehicle/${testMonitoringVehicles[0].id}`,
      'get vehicle request'
    );

    vehicleRequest.flush(testVehicleMonitoringInfo);
  });
});

export const testVehicleMonitoringInfo: VehicleMonitoringInfo = {
  generalInfo: {
    lastMessageTime: '2023-07-13T00:56:27.766Z',
    speed: 16.100714299456435,
    mileage: 320000,
    engineHours: 18,
    satellitesNumber: 20,
    latitude: -77.33591583905098,
    longitude: -89.8236521368749
  },
  trackerInfo: {
    externalId: testTracker.externalId,
    simNumber: testTracker.simNumber,
    imei: testTracker.imei,
    trackerType: testTracker.trackerType.value,
    parameters: testParameters,
    sensors: [
      {
        name: testSensors.sensors[0].name,
        value: '73000',
        unit: testSensors.sensors[0].unit.value
      },
      {
        name: testSensors.sensors[1].name,
        value: '10000',
        unit: testSensors.sensors[1].unit.value
      },
      {
        name: testSensors.sensors[2].name
      }
    ]
  }
};

export const testMonitoringVehicles: MonitoringVehicle[] = [
  {
    id: testVehicles.vehicles[0].id,
    name: testVehicles.vehicles[0].name,
    lastMessageTime: testVehicleMonitoringInfo.generalInfo.lastMessageTime,
    movementStatus: MovementStatus.Moving,
    connectionStatus: ConnectionStatus.Connected,
    numberOfSatellites: testVehicleMonitoringInfo.generalInfo.satellitesNumber,
    tracker: {
      id: testTrackers.trackers[0].id,
      externalId: testTrackers.trackers[0].externalId,
      imei: testTrackers.trackers[0].imei
    }
  },
  {
    id: testVehicles.vehicles[1].id,
    name: testVehicles.vehicles[1].name,
    movementStatus: MovementStatus.NoData,
    connectionStatus: ConnectionStatus.NotConnected
  },
  {
    id: testVehicles.vehicles[2].id,
    name: testVehicles.vehicles[2].name,
    lastMessageTime: '2023-07-13T00:58:49.312Z',
    movementStatus: MovementStatus.Stopped,
    connectionStatus: ConnectionStatus.Connected,
    numberOfSatellites: 7,
    tracker: {
      id: testTrackers.trackers[2].id,
      externalId: testTrackers.trackers[2].externalId,
      imei: testTrackers.trackers[2].imei
    }
  }
];

export const testLocationAndTrackResponse: LocationAndTrackResponse = {
  viewBounds: {
    upperLeftLatitude: 53.336375849999996,
    upperLeftLongitude: 45.08847725,
    bottomRightLatitude: 52.263483150000006,
    bottomRightLongitude: 53.53558075
  },
  tracks: [
    {
      vehicleId: testMonitoringVehicles[0].id,
      latitude: 50.14918,
      longitude: 53.20434,
      track: [
        {
          messageId: 44086,
          time: '2023-08-31T12:08:40.141885Z',
          latitude: 50.14918,
          longitude: 53.20434,
          speed: 0,
          altitude: 113
        },
        {
          messageId: 44088,
          time: '2023-08-31T12:08:40.83533Z',
          latitude: 50.14918,
          longitude: 53.20434,
          speed: 0,
          altitude: 113
        },
        {
          messageId: 44085,
          time: '2023-08-31T12:08:39.811887Z',
          latitude: 50.14918,
          longitude: 53.20434,
          speed: 0,
          altitude: 113
        },
        {
          messageId: 44087,
          time: '2023-08-31T12:08:40.521726Z',
          latitude: 50.14918,
          longitude: 53.20434,
          speed: 0,
          altitude: 113
        }
      ]
    },
    {
      vehicleId: testMonitoringVehicles[1].id,
      latitude: 45.33645,
      longitude: 52.312251
    },
    {
      vehicleId: testMonitoringVehicles[2].id,
      latitude: 50.295905,
      longitude: 53.287243,
      track: [
        {
          messageId: 45059,
          time: '2023-08-31T12:18:21.360719Z',
          latitude: 50.149358,
          longitude: 53.204403,
          speed: 10.7,
          altitude: 121
        },
        {
          messageId: 45058,
          time: '2023-08-31T12:18:20.992514Z',
          latitude: 50.149348,
          longitude: 53.204378,
          speed: 10.1,
          altitude: 120
        },
        {
          messageId: 45060,
          time: '2023-08-31T12:18:21.683683Z',
          latitude: 50.149345,
          longitude: 53.204353,
          speed: 9.8,
          altitude: 120
        },
        {
          messageId: 45061,
          time: '2023-08-31T12:18:21.998364Z',
          latitude: 50.14931,
          longitude: 53.204205,
          speed: 12.7,
          altitude: 118
        },
        {
          messageId: 45062,
          time: '2023-08-31T12:18:22.303882Z',
          latitude: 50.14929,
          longitude: 53.204088,
          speed: 11.6,
          altitude: 117
        }
      ]
    }
  ]
};

export const testFindCriterion = testMonitoringVehicles[0].name.substring(0, SEARCH_MIN_LENGTH);

/**
 * Mock test found monitoring vehicles.
 *
 * @param findCriterion Search criterion.
 *
 * @returns Test found monitoring vehicles.
 */
export const mockTestFoundMonitoringVehicles = (
  findCriterion = testFindCriterion.toLocaleLowerCase()
) => testMonitoringVehicles.filter(({ name, tracker }) => name
  .toLocaleLowerCase()
  .includes(findCriterion) || [tracker?.externalId, tracker?.imei].includes(findCriterion)
);
