import { TestBed } from '@angular/core/testing';
import { HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { AuthInterceptor } from './auth.interceptor';

import { TokenKey, TokenService } from './token.service';

import { testCredentialsResponse } from './auth.service.spec';

describe('AuthInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AuthInterceptor,
          multi: true
        },
        AuthInterceptor
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);

    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    const interceptor = TestBed.inject(AuthInterceptor);

    expect(interceptor).toBeTruthy();
  });

  it(`should leave HTTP request 'Authorization' header unmodified without token`, (done: DoneFn) => {
    const httpClient = TestBed.inject(HttpClient);
    const httpTestingController = TestBed.inject(HttpTestingController);

    const testRequestURL = `/`;

    httpClient
      .get(testRequestURL)
      .subscribe(() => {
        done();
      });

    const testRequest = httpTestingController.expectOne(testRequestURL, 'test API request');

    expect(testRequest.request.headers.has('Authorization'))
      .withContext(`leave HTTP request headers unmodified by 'Authorization' header`)
      .toBe(false);

    testRequest.flush(null);
  });

  it(`should modify HTTP request 'Authorization' header with token`, (done: DoneFn) => {
    const tokenService = TestBed.inject(TokenService);

    spyOnProperty(tokenService, 'token')
      .and.returnValue(testCredentialsResponse.accessToken);

    const testRequestURL = `/`;

    httpClient
      .get(testRequestURL)
      .subscribe(() => {
        done();
      });

    const testRequest = httpTestingController.expectOne(testRequestURL, 'test API request');

    expect(testRequest.request.headers.get('Authorization'))
      .withContext(`modify 'Authorization' header with token`)
      .toBe(`Bearer ${testCredentialsResponse.accessToken}`);

    testRequest.flush(null);

    tokenService.clear();
  });
});
