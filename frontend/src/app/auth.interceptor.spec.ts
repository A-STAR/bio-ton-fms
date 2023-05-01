import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpErrorResponse, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { catchError, EMPTY, of } from 'rxjs';

import { AuthInterceptor } from './auth.interceptor';

import { AuthService } from './auth.service';
import { TokenService } from './token.service';

import { testCredentialsResponse } from './auth.service.spec';

describe('AuthInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let tokenService: TokenService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatDialogModule
      ],
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

    tokenService = TestBed.inject(TokenService);

    tokenService.clear();
  });

  afterEach(() => {
    httpTestingController.verify();

    tokenService.clear();
  });

  it('should be created', () => {
    const interceptor = TestBed.inject(AuthInterceptor);

    expect(interceptor).toBeTruthy();
  });

  it(`should leave HTTP request 'Authorization' header unmodified without token`, (done: DoneFn) => {
    const httpClient = TestBed.inject(HttpClient);
    const httpTestingController = TestBed.inject(HttpTestingController);

    const testRequestURL = '/';

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
    spyOnProperty(tokenService, 'token')
      .and.returnValue(testCredentialsResponse.accessToken);

    const testRequestURL = '/';

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
  });

  it('should sign out on `401` and `403` error response', (done: DoneFn) => {
    const router = TestBed.inject(Router);
    const dialog = TestBed.inject(MatDialog);
    const authService = TestBed.inject(AuthService);

    const signOutSpy = spyOnProperty(authService, 'signOut$')
      .and.callFake(() => of(undefined));

    const navigateSpy = spyOn(router, 'navigate');
    const closeAllSpy = spyOn(dialog, 'closeAll');

    spyOnProperty(tokenService, 'token')
      .and.returnValue(testCredentialsResponse.accessToken);

    const testRequestURL = '/';

    const testErrorResponse = {
      message: 'Unauthorized'
    };

    const testRequestErrorOptions: {
      headers?: HttpHeaders | {
        [name: string]: string | string[];
      };
      status?: number;
      statusText?: string;
    } = {
      status: 401,
      statusText: 'Unauthorized'
    };

    const testRequest$ = httpClient
      .get(testRequestURL)
      .pipe(
        catchError(({
          status,
          statusText,
          error: { message }
        }: HttpErrorResponse) => {
          expect(status)
            .withContext(`catch ${testRequestErrorOptions.status} error response status`)
            .toBe(testRequestErrorOptions.status!);

          expect(statusText)
            .withContext(`catch ${testRequestErrorOptions.status} error response status text`)
            .toBe(testRequestErrorOptions.statusText!);

          expect(message)
            .withContext(`catch ${testRequestErrorOptions.status} error response error message`)
            .toBe(testErrorResponse.message);

          return EMPTY;
        })
      );

    testRequest$.subscribe();

    let testRequest = httpTestingController.expectOne(testRequestURL, 'test API request');

    testRequest.flush(testErrorResponse, testRequestErrorOptions);

    expect(signOutSpy)
      .toHaveBeenCalledBefore(closeAllSpy);

    expect(closeAllSpy)
      .toHaveBeenCalledBefore(navigateSpy);

    expect(navigateSpy)
      .toHaveBeenCalledWith(['/sign-in'], {
        replaceUrl: true
      });

    signOutSpy.calls.reset();
    closeAllSpy.calls.reset();
    navigateSpy.calls.reset();

    testErrorResponse.message = 'Forbidden';
    testRequestErrorOptions.status = 403;
    testRequestErrorOptions.statusText = 'Forbidden';

    testRequest$.subscribe({
      complete: () => {
        done();
      }
    });

    testRequest = httpTestingController.expectOne(testRequestURL, 'test API request');

    testRequest.flush(testErrorResponse, testRequestErrorOptions);

    expect(signOutSpy)
      .toHaveBeenCalledBefore(closeAllSpy);

    expect(closeAllSpy)
      .toHaveBeenCalledBefore(navigateSpy);

    expect(navigateSpy)
      .toHaveBeenCalledWith(['/sign-in'], {
        replaceUrl: true
      });
  });
});
