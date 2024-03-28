import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

import { Observable, catchError, mergeMap, of, tap, throwError } from 'rxjs';

import { TokenService } from './token.service';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(httpRequest: HttpRequest<unknown>, httpHandler: HttpHandler): Observable<HttpEvent<unknown>> {
    let authRequest: HttpRequest<unknown> | undefined;

    if (this.tokenService.token) {
      const headers = httpRequest.headers.set('Authorization', `Bearer ${this.tokenService.token}`);

      authRequest = httpRequest.clone({ headers });
    }

    return httpHandler
      .handle(authRequest ?? httpRequest)
      .pipe(
        catchError(error => {
          let error$ = of(undefined);

          if (error instanceof HttpErrorResponse) {
            switch (error.status) {
              case 401:
              case 403:
                error$ = this.authService.signOut$.pipe(
                  tap(async () => {
                    this.dialog.closeAll();

                    await this.router.navigate(['/sign-in'], {
                      replaceUrl: true
                    });
                  })
                );
            }
          }

          return error$.pipe(
            mergeMap(() => throwError(() => error))
          );
        })
      );
  }

  constructor(private router: Router, private dialog: MatDialog, private tokenService: TokenService, private authService: AuthService) { }
}
