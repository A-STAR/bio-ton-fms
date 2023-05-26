import { TestBed } from '@angular/core/testing';

import { TokenKey, TokenService } from './token.service';

import { testCredentialsResponse } from './auth.service.spec';

describe('TokenService', () => {
  let service: TokenService;

  beforeEach(() => {
    service = TestBed.inject(TokenService);

    service.clear();
  });

  afterEach(() => {
    service.clear();
  });
  it('should be created', () => {
    expect(service)
      .toBeTruthy();
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

  it('should clear tokens', () => {
    service.saveToken(testCredentialsResponse.accessToken);
    service.saveToken(testCredentialsResponse.refreshToken);

    spyOn(localStorage, 'removeItem').and.callThrough();

    service.clear();

    expect(localStorage.removeItem)
      .toHaveBeenCalledWith(TokenKey.Token);

    expect(localStorage.removeItem)
      .toHaveBeenCalledWith(TokenKey.RefreshToken);

    expect(service.token)
      .withContext('clear token')
      .toBeNull();
  });
});
