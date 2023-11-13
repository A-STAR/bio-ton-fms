import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { MessageService } from './message.service';

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
});
