import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { BehaviorSubject, Observable, of, tap } from 'rxjs';

import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  /**
   * Get authenticated state.
   *
   * @returns An `Observable` of authenticated state stream.
   */
  get authenticated$(): Observable<boolean> {
    return this.#authenticated$.asObservable();
  }

  /**
   * Sign out user.
   *
   * @returns An `Observable` of signing out stream.
   */
  get signOut$() {
    return of(undefined)
      .pipe(
        tap(() => {
          this.#setAuthenticated(false);

          this.tokenService.clear();
        })
      );
  }

  /**
   * Authenticate user in `AuthGuard` if there's a token.
   *
   * @returns An `Observable` of authenticate stream.
   */
  authenticate() {
    if (this.tokenService.token) {
      // TODO: refresh access token.
      this.#setAuthenticated(true);
    }

    return of(undefined);
  }

  /**
   * Sign in user.
   *
   * @param credentials Credentials.
   *
   * @returns An `Observable` of signing in stream.
   */
  signIn({ username, password }: Credentials) {
    const body = {
      userName: username,
      password
    };

    return this.httpClient
      .post<CredentialsResponse>('/api/auth/login', body)
      .pipe(
        tap(({ accessToken, refreshToken }) => {
          this.tokenService.saveToken(accessToken);
          this.tokenService.saveRefreshToken(refreshToken);

          this.#setAuthenticated(true);
        })
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

  constructor(private httpClient: HttpClient, private tokenService: TokenService) { }
}

export type Credentials = {
  username: string;
  password: string;
};

export type CredentialsResponse = {
  accessToken: string;
  refreshToken: string;
};
