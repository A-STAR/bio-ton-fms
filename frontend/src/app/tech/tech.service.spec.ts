import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ConnectionStatus, MonitoringVehicle, MonitoringVehiclesOptions, MovementStatus, TechService } from './tech.service';

import { testVehicles } from '../directory-tech/vehicle.service.spec';
import { testTrackers } from '../directory-tech/tracker.service.spec';
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
      findCriterion: testFindCriterion
    };

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
});

export const testMonitoringVehicles: MonitoringVehicle[] = [
  {
    id: testVehicles.vehicles[0].id,
    name: testVehicles.vehicles[0].name,
    lastMessageTime: '2023-07-13T00:56:27.766Z',
    movementStatus: MovementStatus.Moving,
    connectionStatus: ConnectionStatus.Connected,
    numberOfSatellites: 20,
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

export const testFoundMonitoringVehicles = testMonitoringVehicles.filter(
  ({ name, tracker }) => name.includes(testFindCriterion)
    || [tracker?.externalId, tracker?.imei].includes(testFindCriterion)
);
