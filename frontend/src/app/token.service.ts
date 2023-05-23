import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  /**
   * Access token.
   *
   * @returns Token.
   */
  get token() {
    return localStorage.getItem(TokenKey.Token);
  }

  /**
   * Save access token.
   *
   * @param token Access token.
   */
  saveToken(token: string) {
    localStorage.setItem(TokenKey.Token, token);
  }

  /**
   * Save refresh access token.
   *
   * @param token Refresh access token.
   */
  saveRefreshToken(token: string) {
    localStorage.setItem(TokenKey.RefreshToken, token);
  }

  /**
   * Clear access and refresh.
   */
  clear() {
    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  }
}

export enum TokenKey {
  Token = 'token',
  RefreshToken = 'refreshToken'
}
