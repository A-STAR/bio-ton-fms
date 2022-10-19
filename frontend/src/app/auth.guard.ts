import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanLoad, Route, Router, RouterStateSnapshot, UrlSegment, UrlTree } from '@angular/router';

import { map, Observable, tap } from 'rxjs';

import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanLoad {
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const { url } = state;

    return this.#canActivate(url);
  }
  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> {
    const { path } = route;

    return this.#canActivate(path);
  }

  #canActivate(url: string | undefined): Observable<boolean> {
    const signInPath = '/sign-in';
    const isSignInPage = url === signInPath;

    return this.authService.authenticated$.pipe(
      tap(authenticated => {
        if (authenticated && isSignInPage) {
          this.router.navigate(['/']);
        }

        if (!authenticated && !isSignInPage) {
          this.router.navigate([signInPath]);
        }
      }),
      map(authenticated => isSignInPage ? !authenticated : authenticated)
    );
  }

  constructor(private router: Router, private authService: AuthService) {}
}
