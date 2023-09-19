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
      .subscribe(info => {
        expect(info)
          .withContext('emit vehicle info')
          .toBe(testVehicleMonitoringInfo);

        done();
      });

    const vehicleRequest = httpTestingController.expectOne(
      `/api/telematica/monitoring/vehicle/${testMonitoringVehicles[0].id}`,
      'get vehicle info request'
    );

    vehicleRequest.flush(testVehicleMonitoringInfo);
  });

  it('should get message', (done: DoneFn) => {
    service
      .getMessage(testMonitoringVehicles[0].id)
      .subscribe(message => {
        expect(message)
          .withContext('emit message')
          .toBe(testMonitoringMessageInfo);

        done();
      });

    const messageRequest = httpTestingController.expectOne(
      `/api/telematica/monitoring/trackPoint/${testMonitoringVehicles[0].id}`,
      'get message request'
    );

    messageRequest.flush(testMonitoringMessageInfo);
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
      vehicleName: testMonitoringVehicles[0].name,
      latitude: 50.14918,
      longitude: 53.20434,
      track: [
        {
          messageId: 44086,
          time: '2023-09-08T12:08:40.141885Z',
          speed: 0,
          numberOfSatellites: 15,
          latitude: 53.20434,
          longitude: 50.14918
        },
        {
          messageId: 44088,
          time: '2023-09-08T12:08:40.83533Z',
          speed: 0,
          numberOfSatellites: 15,
          latitude: 53.20434,
          longitude: 50.14918
        },
        {
          messageId: 44085,
          time: '2023-09-08T12:08:39.811887Z',
          speed: 0,
          numberOfSatellites: 15,
          latitude: 53.20434,
          longitude: 50.14918
        },
        {
          messageId: 44087,
          time: '2023-09-08T12:08:40.521726Z',
          speed: 0,
          numberOfSatellites: 15,
          latitude: 53.20434,
          longitude: 50.14918
        },
        {
          messageId: 44090,
          time: '2023-09-08T12:08:41.54515Z',
          speed: 0,
          numberOfSatellites: 15,
          latitude: 53.20434,
          longitude: 50.14918
        }
      ]
    },
    {
      vehicleId: testMonitoringVehicles[1].id,
      vehicleName: testMonitoringVehicles[1].name,
      latitude: 45.33645,
      longitude: 52.312251
    },
    {
      vehicleId: testMonitoringVehicles[2].id,
      vehicleName: testMonitoringVehicles[2].name,
      latitude: 50.295905,
      longitude: 53.287243,
      track: [
        {
          messageId: 41640,
          speed: 101.8,
          latitude: 52.650876,
          longitude: 45.866778
        },
        {
          messageId: 41641,
          time: '2023-09-08T11:53:07.393943Z',
          speed: 97.7,
          latitude: 52.649831,
          longitude: 45.86571
        },
        {
          messageId: 41642,
          time: '2023-09-08T11:53:07.405803Z',
          speed: 93.7,
          latitude: 52.648866,
          longitude: 45.864563
        },
        {
          messageId: 41643,
          time: '2023-09-08T11:53:07.417391Z',
          speed: 93.5,
          latitude: 52.647958,
          longitude: 45.863328
        }
      ]
    }
  ]
};

const testMonitoringMessageInfo = {
  generalInfo: {
    messageTime: testVehicleMonitoringInfo.generalInfo.lastMessageTime,
    speed: testVehicleMonitoringInfo.generalInfo.speed,
    numberOfSatellites: testVehicleMonitoringInfo.generalInfo.satellitesNumber,
    latitude: testVehicleMonitoringInfo.generalInfo.latitude,
    longitude: testVehicleMonitoringInfo.generalInfo.longitude
  },
  trackerInfo: { ...testVehicleMonitoringInfo.trackerInfo }
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
