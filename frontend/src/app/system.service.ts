import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SystemService {
  constructor(private httpClient: HttpClient) { }

  /**
   * Get system version.
   *
   * @returns An `Observable' of system version stream.
   */
  get getVersion$() {
    return this.httpClient.get<string>('/system/get-version');
  }
}
