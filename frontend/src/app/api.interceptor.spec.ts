import { TestBed } from '@angular/core/testing';
import { HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { APIInterceptor } from './api.interceptor';

import { environment } from '../environments/environment';

describe('APIInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [
      {
        provide: HTTP_INTERCEPTORS,
        useClass: APIInterceptor,
        multi: true
      },
      APIInterceptor
    ]
  }));

  it('should be created', () => {
    const interceptor = TestBed.inject(APIInterceptor);

    expect(interceptor).toBeTruthy();
  });

  it('should modify HTTP request URL to API URL', (done: DoneFn) => {
    const httpClient = TestBed.inject(HttpClient);
    const httpTestingController = TestBed.inject(HttpTestingController);

    const testRequestURL = '/';

    httpClient
      .get(testRequestURL)
      .subscribe(() => {
        done();
      });

    const apiRequestURL = `${environment.api}${testRequestURL}`;

    const testRequest = httpTestingController.expectOne(apiRequestURL, 'test API request');

    expect(testRequest.request.url)
      .withContext('prepend API to HTTP request URL')
      .toBe(apiRequestURL);

    testRequest.flush(null);

    httpTestingController.verify();
  });
});
