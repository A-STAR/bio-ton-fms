import { Injectable } from '@angular/core';

import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  /**
   * Get authenticated state.
   *
   * @returns An `Observable' of authenticated state stream.
   */
  get authenticated$(): Observable<boolean> {
    return this.#authenticated$.asObservable();
  }

  /**
   * Emit authenticated state.
   *
   * @param authenticated An authenticated state.
   */
  setAuthenticated(authenticated: boolean) {
    this.#authenticated$.next(authenticated);
  }

  #authenticated$ = new BehaviorSubject(false);
}
