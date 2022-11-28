import { ErrorHandler as ErrorHanlderInterface, Injectable, NgZone } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable()
export class ErrorHandler implements ErrorHanlderInterface {
  /**
   * Handle application and response errors.
   *
   * @param error An error or `HttpErrorResposne`.
   */
  handleError(error: any) {
    const isErrorResponse = error instanceof HttpErrorResponse;

    if (!isErrorResponse && error.rejection) {
      error = error.rejection;
    }

    this.zone.run(() => {
      const message: string = error?.error?.messages?.join(' ')
        ?? error?.error?.message
        ?? `${DEFAULT_ERROR_MESSAGE}${error?.message ? `: ${error.message}` : ''}`;

      this.snackBar.open(message, ERROR_ACTION, snackBarConfig);
    });

    console.error(error);
  };

  constructor(private zone: NgZone, private snackBar: MatSnackBar) { }
}

export const DEFAULT_ERROR_MESSAGE = 'Произошла ошибка';
export const ERROR_ACTION = '✕';

export const snackBarConfig: MatSnackBarConfig = {
  duration: undefined,
  panelClass: 'error'
};
