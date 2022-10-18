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

  it('should initialize authenticated state', async () => {
    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('initialize `autnenticated$`')
      .toBeResolvedTo(false);
  });

  it('should sign in', async () => {
    await firstValueFrom(service.signIn$);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `true`')
      .toBeResolvedTo(true);
  });

  it('should sign out', async () => {
    await firstValueFrom(service.signIn$);
    await firstValueFrom(service.signOut$);

    await expectAsync(firstValueFrom(service.authenticated$))
      .withContext('set `autnenticated$` to `false`')
      .toBeResolvedTo(false);
  });
});
