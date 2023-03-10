import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ActivatedRouteSnapshot, Route, Router, RouterStateSnapshot, UrlSegment } from '@angular/router';

import { firstValueFrom, of } from 'rxjs';

import { AuthService } from './auth.service';

import { AuthGuard } from './auth.guard';

import { TokenKey } from './token.service';
import { testSignIn } from './auth.service.spec';

describe('AuthGuard', () => {
  let httpTestingController: HttpTestingController;
  let router: Router;
  let guard: AuthGuard;
  let authService: AuthService;

  const testRouteSnapshot = {} as ActivatedRouteSnapshot;

  const testState = {
    url: '/'
  } as RouterStateSnapshot;

  const testRoute = {
    path: '/'
  } as Route;

  const testSegments = [
    {
      path: '/'
    }
  ] as UrlSegment[];

  const testSignInState = {
    url: '/sign-in'
  } as RouterStateSnapshot;

  const testSignInRoute = {
    path: '/sign-in'
  } as Route;

  const testSignInSegments = [
    {
      path: '/sign-in'
    }
  ] as UrlSegment[];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpTestingController = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    guard = TestBed.inject(AuthGuard);
    authService = TestBed.inject(AuthService);

    spyOn(router, 'navigate');

    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });

  afterEach(() => {
    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });

  it('should be created', () => {
    expect(guard)
      .toBeTruthy();
  });

  it('should allow navigation to Sign in page', async () => {
    spyOn(authService, 'authenticate')
      .and.callFake(() => of(undefined));

    const canActivate = await firstValueFrom(
      guard.canActivate(testRouteSnapshot, testSignInState)
    );

    expect(router.navigate)
      .not.toHaveBeenCalled();

    expect(canActivate)
      .withContext('allow navigation to Sign in page')
      .toBe(true);

    const canLoad = await firstValueFrom(
      guard.canLoad(testSignInRoute, testSignInSegments)
    );

    expect(router.navigate)
      .not.toHaveBeenCalled();

    expect(canLoad)
      .withContext('allow navigation to Sign in page')
      .toBe(true);
  });

  it('should allow navigation to authenticated area', async () => {
    testSignIn(httpTestingController, authService);

    const canActivate = await firstValueFrom(
      guard.canActivate(testRouteSnapshot, testState)
    );

    expect(router.navigate)
      .not.toHaveBeenCalled();

    expect(canActivate)
      .withContext('allow navigation to authenticated area')
      .toBe(true);

    const canLoad = await firstValueFrom(
      guard.canLoad(testRoute, testSegments)
    );

    expect(router.navigate)
      .not.toHaveBeenCalled();

    expect(canLoad)
      .withContext('allow navigation to authenticated area')
      .toBe(true);
  });

  it('should redirect to Sign in page', async () => {
    spyOn(authService, 'authenticate')
      .and.callFake(() => of(undefined));

    const canActivate = await firstValueFrom(
      guard.canActivate(testRouteSnapshot, testState)
    );

    expect(router.navigate)
      .withContext('redirect to Sign in page')
      .toHaveBeenCalledWith(['/sign-in']);

    expect(canActivate)
      .withContext('cancel navigation to authenticated area')
      .toBe(false);

    const canLoad = await firstValueFrom(
      guard.canLoad(testRoute, testSegments)
    );

    expect(router.navigate)
      .withContext('redirect to Sign in page')
      .toHaveBeenCalledWith(['/sign-in']);

    expect(canLoad)
      .withContext('cancel navigation to authenticated area')
      .toBe(false);
  });

  it('should redirect to authenticated area', async () => {
    testSignIn(httpTestingController, authService);

    const canActivate = await firstValueFrom(
      guard.canActivate(testRouteSnapshot, testSignInState)
    );

    expect(router.navigate)
      .withContext('redirect to authenticated area')
      .toHaveBeenCalledWith(['/']);

    expect(canActivate)
      .withContext('cancel navigation to Sign in page')
      .toBe(false);

    const canLoad = await firstValueFrom(
      guard.canLoad(testSignInRoute, testSignInSegments)
    );

    expect(router.navigate)
      .withContext('redirect to Sign in page')
      .toHaveBeenCalledWith(['/']);

    expect(canLoad)
      .withContext('cancel navigation to authenticated area')
      .toBe(false);
  });
});
