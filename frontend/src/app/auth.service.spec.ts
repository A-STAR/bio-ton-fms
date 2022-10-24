import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { firstValueFrom } from 'rxjs';

import { AuthService, Credentials, CredentialsResponse } from './auth.service';

import { TokenKey } from './token.service';

describe('AuthService', () => {
  let httpTestingController: HttpTestingController;
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    service = TestBed.inject(AuthService);

  });

  afterEach(() => {
    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should initialize authenticated state', async () => {
    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('initialize `autnenticated$`')
      .toBeResolvedTo(false);
  });

  it('should authenticate', async () => {
    const tokenService = TestBed.inject(TokenService);

    spyOnProperty(tokenService, 'token')
      .and.returnValue(testCredentialsResponse.accessToken);

    const authenticate$ = service.authenticate();

    firstValueFrom(authenticate$);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `true` if there\'s token')
      .toBeResolvedTo(true);
  });

  it('should sign in', async () => {
    testSignIn(httpTestingController, service);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `true`')
      .toBeResolvedTo(true);
  });

  it('should sign out', async () => {
    testSignIn(httpTestingController, service);

    await firstValueFrom(service.signOut$);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `false`')
      .toBeResolvedTo(false);
  });
});

/* eslint-disable max-len */

export const testCredentials: Credentials = {
  username: 'admin',
  password: 'root'
};

export const testCredentialsResponse: CredentialsResponse = {
  accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJKV1QuQWNjZXNzVG9rZW4iOiIiLCJuYmYiOjE2NjYzMTkyOTgsImV4cCI6MTY2NjMyMDE5OCwiaXNzIjoiaHR0cHM6Ly9iaW90b24tZm1zLnJ1LyIsImF1ZCI6Imh0dHBzOi8vYmlvdG9uLWZtcy5ydS8ifQ.j7URlrRFrt60fIJ_lX1juBIwZHQKzIRX2iXhMmLDxmE',
  refreshToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJKV1QuUmVmcmVzaFRva2VuIjoiIiwibmJmIjoxNjY2MzE5Mjk4LCJleHAiOjE2Njg5MTEyOTgsImlzcyI6Imh0dHBzOi8vYmlvdG9uLWZtcy5ydS8iLCJhdWQiOiJodHRwczovL2Jpb3Rvbi1mbXMucnUvIn0.18XB6m0lcdXTnGUDGC1uEHXRDKbu-0mdICox-HBTK8Q'
};

/* eslint-enable max-len */

export function testSignIn(httpTestingController: HttpTestingController, authService: AuthService) {
  const signIn$ = authService.signIn(testCredentials);

  firstValueFrom(signIn$);

  const loginRequest = httpTestingController.expectOne({
    method: 'POST',
    url: '/api/auth/login'
  }, 'login request');

  loginRequest.flush(testCredentialsResponse);

  httpTestingController.verify();
}
