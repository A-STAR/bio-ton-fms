import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';

import { Observable} from 'rxjs';

import { TokenService } from './token.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(httpRequest: HttpRequest<unknown>, httpHandler: HttpHandler): Observable<HttpEvent<unknown>> {
    let authRequest: HttpRequest<unknown> | undefined;

    if (this.tokenService.token) {
      const headers = httpRequest.headers.set('Authorization', `Bearer ${this.tokenService.token}`);

      authRequest = httpRequest.clone({ headers });
    }

    return httpHandler.handle(authRequest ?? httpRequest);
  }

  constructor(private tokenService: TokenService) { }
}
