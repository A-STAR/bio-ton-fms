import { TestBed } from '@angular/core/testing';

import { firstValueFrom } from 'rxjs';

import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service)
      .toBeTruthy();
  });

  it('should set authenticated state', async () => {
    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('initialize `autnenticated$`')
      .toBeResolvedTo(false);

    service.setAuthenticated(true);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `true`')
      .toBeResolvedTo(true);

    service.setAuthenticated(false);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `false`')
      .toBeResolvedTo(false);
  });
});
