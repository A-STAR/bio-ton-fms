import { ErrorHandler } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { DEFAULT_ERROR_MESSAGE, ErrorHandler as ErrorHandlerClass, ERROR_ACTION, snackBarConfig } from './error.handler';

describe('ErrorHandler', () => {
  let handler: ErrorHandler;
  let snackBarSpy = jasmine.createSpyObj<MatSnackBar>('MatSnackBar', ['open']);

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: ErrorHandler,
          useClass: ErrorHandlerClass
        },
        {
          provide: MatSnackBar,
          useValue: snackBarSpy
        }
      ]
    });

    handler = TestBed.inject(ErrorHandler);

    spyOn(console, 'error');
  });

  it('should be created', () => {
    expect(handler)
      .toBeTruthy();
  });

  it('should handle error', () => {
    const testError = new Error();

    handler.handleError(testError);

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(DEFAULT_ERROR_MESSAGE, ERROR_ACTION, snackBarConfig);

    expect(console.error)
      .toHaveBeenCalledWith(testError);
  });

  it('should handle error rejection', () => {
    const testRejectionError = {
      rejection: {
        message: `stopped(aborted)`
      }
    };

    handler.handleError(testRejectionError);

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(`${DEFAULT_ERROR_MESSAGE}: ${testRejectionError.rejection.message}`, ERROR_ACTION, snackBarConfig);

    expect(console.error)
      .toHaveBeenCalledWith(testRejectionError.rejection);
  });

  it('should handle error response', () => {
    const testErrorResponse = new HttpErrorResponse({
      error: {
        message: 'Http failure response for https://bioton-fms.ru: 504 Gateway Timeout'
      },
      status: 504,
      statusText: 'Gateway Timeout',
      url: 'https://bioton-fms.ru'
    });

    handler.handleError(testErrorResponse);

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(testErrorResponse.error.message, ERROR_ACTION, snackBarConfig);

    expect(console.error)
      .toHaveBeenCalledWith(testErrorResponse);

    delete testErrorResponse.error.message;

    testErrorResponse.error.messages = ['Машина не существует', 'Машина с именем Марьевка уже существует'];

    handler.handleError(testErrorResponse);

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(
        testErrorResponse.error.messages.join(' '),
        ERROR_ACTION,
        snackBarConfig
      );

    expect(console.error)
      .toHaveBeenCalledWith(testErrorResponse);
  });
});
