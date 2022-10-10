import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';

@Injectable()
export class APIInterceptor implements HttpInterceptor {
  intercept(httpRequest: HttpRequest<unknown>, httpHandler: HttpHandler): Observable<HttpEvent<unknown>> {
    const apiRequest = httpRequest.clone({
      url: `${environment.api}${httpRequest.url}`
    });

    return httpHandler.handle(apiRequest);
  }
}
