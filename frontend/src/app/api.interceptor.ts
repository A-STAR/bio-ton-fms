import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../environments/environment';

@Injectable()
export class APIInterceptor implements HttpInterceptor {
  intercept(httpRequest: HttpRequest<unknown>, httpHandler: HttpHandler): Observable<HttpEvent<unknown>> {
    const url = `${environment.api}${httpRequest.url}`;

    const apiRequest = httpRequest.clone({ url });

    return httpHandler.handle(apiRequest);
  }
}
