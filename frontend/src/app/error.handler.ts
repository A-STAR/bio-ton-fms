import { ErrorHandler as ErrorHandlerClass, Injectable, NgZone } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable()
export class ErrorHandler implements ErrorHandlerClass {
  /**
   * Handle application and response errors.
   *
   * @param error An error or `HttpErrorResponse`.
   */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  handleError(error: any) {
    const isErrorResponse = error instanceof HttpErrorResponse;

    if (!isErrorResponse && error.rejection) {
      error = error.rejection;
    }

    this.zone.run(() => {
      let message: string = error.error?.messages?.join(' ') ?? error.error?.message ?? error.error
        ?? `${DEFAULT_ERROR_MESSAGE}${error.message ? `: ${error.message}` : ''}`;

      if (error.error?.errors) {
        message = this.#getErrorsMessage(error.error.errors);
      }

      this.snackBar.open(message, ERROR_ACTION, snackBarConfig);
    });

    // eslint-disable-next-line no-console
    console.error(error);
  }

  /**
   * Combine error messages.
   *
   * @param errors Errors from `HttpErrorResponse`.
   *
   * @returns Combined error message.
   */
  #getErrorsMessage(errors: {
    [key: string]: string[];
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
