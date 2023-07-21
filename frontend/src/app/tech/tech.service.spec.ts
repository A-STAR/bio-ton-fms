import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ConnectionStatus, MonitoringVehicle, MovementStatus, TechService } from './tech.service';

import { testVehicles } from '../directory-tech/vehicle.service.spec';
import { testTrackers } from '../directory-tech/tracker.service.spec';

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

    const vehiclesRequest = httpTestingController.expectOne(`/api/telematica/monitoring/vehicles`, 'vehicles request');

    vehiclesRequest.flush(testMonitoringVehicles);

    httpTestingController.verify();
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
    trackerExternalId: testTrackers.trackers[0].externalId,
    trackerIMEI: testTrackers.trackers[0].imei
  },
  {
    id: testVehicles.vehicles[1].id,
    name: testVehicles.vehicles[1].name,
    movementStatus: MovementStatus.NoData,
    connectionStatus: ConnectionStatus.NotConnected,
    numberOfSatellites: 0
  },
  {
    id: testVehicles.vehicles[2].id,
    name: testVehicles.vehicles[2].name,
    lastMessageTime: '2023-07-13T00:58:49.312Z',
    movementStatus: MovementStatus.Stopped,
    connectionStatus: ConnectionStatus.Connected,
    numberOfSatellites: 7,
    trackerExternalId: testTrackers.trackers[2].externalId,
    trackerIMEI: testTrackers.trackers[2].imei,
  }
];
