import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import {
  ConnectionStatus,
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

const testVehicleMonitoringInfo: VehicleMonitoringInfo = {
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
