import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

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

  /**
   * Sign in user.
   *
   * @returns An `Observable' of signing in stream.
   */
  signIn({ username, password }: Credentials) {
    const body = {
      userName: username,
      password
    };

    return this.httpClient
      .post<CredentialsResponse>('/api/auth/login', body)
      .pipe(
        tap(this.#setAuthenticated.bind(this, true))
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

  constructor(private httpClient: HttpClient) { }
}

export type Credentials = {
  username: string;
  password: string;
}

export type CredentialsResponse = {
  accessToken: string;
  refreshToken: string;
};
