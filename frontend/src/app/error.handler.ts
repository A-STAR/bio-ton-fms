import { ErrorHandler as ErrorHanlderInterface, Injectable, NgZone } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatLegacySnackBar as MatSnackBar, MatLegacySnackBarConfig as MatSnackBarConfig } from '@angular/material/legacy-snack-bar';

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
      let message: string = error?.error?.messages?.join(' ')
        ?? error?.error?.message
        ?? `${DEFAULT_ERROR_MESSAGE}${error?.message ? `: ${error.message}` : ''}`;

      if (error?.error?.errors) {
        message = this.#getErrorsMessage(error.error.errors);
      }

      this.snackBar.open(message, ERROR_ACTION, snackBarConfig);
    });

    console.error(error);
  };

  /**
   * Combine error messages.
   *
   * @param errors Errors from `HttpErrorResponse`.
   *
   * @returns Combined error message.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #getErrorsMessage(errors: {
    [key: string]: string[]
  }) {
    return Object
      .values(errors)
      .reduce((errorsMessage, errors) => {
        const errorMessages = errors.reduce((messages, error) => messages += messages ? ` ${error}` : error, '');

        errorsMessage += errorsMessage ? ` ${errorMessages}` : errorMessages;

        return errorsMessage;
      }, '');
  }

  constructor(private zone: NgZone, private snackBar: MatSnackBar) { }
}

export const DEFAULT_ERROR_MESSAGE = 'Произошла ошибка';
export const ERROR_ACTION = '✕';

export const snackBarConfig: MatSnackBarConfig = {
  duration: undefined,
  panelClass: 'error'
};
