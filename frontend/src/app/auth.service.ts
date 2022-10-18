import { Injectable } from '@angular/core';

import { BehaviorSubject, Observable, of, tap } from 'rxjs';

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
   * Sign in user.
   *
   * @returns An `Observable' of signing in stream.
   */
  get signIn$() {
    return of(undefined)
      .pipe(
        tap(this.#setAuthenticated.bind(this, true))
      );
  }

  /**
   * Sign out user.
   *
   * @returns An `Observable' of signing out stream.
   */
  get signOut$() {
    return of(undefined)
      .pipe(
        tap(this.#setAuthenticated.bind(this, false))
      );
  }

  #authenticated$ = new BehaviorSubject(false);

  /**
   * Emit authenticated state.
   *
   * @param authenticated An authenticated state.
   */
  #setAuthenticated(authenticated: boolean) {
    this.#authenticated$.next(authenticated);
  }
}
