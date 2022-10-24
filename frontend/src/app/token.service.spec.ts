import { TestBed } from '@angular/core/testing';

import { TokenKey, TokenService } from './token.service';

import { testCredentialsResponse } from './auth.service.spec';

describe('TokenService', () => {
  let service: TokenService;

  beforeEach(() => {
    service = TestBed.inject(TokenService);

    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });

  afterEach(() => {
    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should save token', () => {
    spyOn(localStorage, 'setItem').and.callThrough();

    service.saveToken(testCredentialsResponse.accessToken);

    expect(localStorage.setItem)
      .toHaveBeenCalledOnceWith(TokenKey.Token, testCredentialsResponse.accessToken);
  });

  it('should get token', () => {
    service.saveToken(testCredentialsResponse.accessToken);

    expect(service.token)
      .withContext('save token')
      .toBe(testCredentialsResponse.accessToken);
  });

  it('should save refresh token', () => {
    spyOn(localStorage, 'setItem').and.callThrough();

    service.saveRefreshToken(testCredentialsResponse.refreshToken);

    expect(localStorage.setItem)
      .toHaveBeenCalledOnceWith(TokenKey.RefreshToken, testCredentialsResponse.refreshToken);
  });
});
