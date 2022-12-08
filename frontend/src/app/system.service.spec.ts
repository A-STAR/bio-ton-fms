import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { SystemService } from './system.service';

describe('SystemService', () => {
  let service: SystemService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(SystemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return system version', (done: DoneFn) => {
    const httpTestingController = TestBed.inject(HttpTestingController);

    const testVersion = '1.0.0';

    service.version$.subscribe(version => {
      expect(version)
        .withContext('emit version')
        .toBe(testVersion);

      done();
    });

    const versionRequest = httpTestingController.expectOne('/system/get-version', 'system version request');

    versionRequest.flush('1.0.0');

    httpTestingController.verify();
  });
});
