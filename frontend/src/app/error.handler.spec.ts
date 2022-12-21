import { ErrorHandler } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { MatLegacySnackBar as MatSnackBar } from '@angular/material/legacy-snack-bar';

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

  it('should handle error response with multiple errors', () => {
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

  it('should handle error response', () => {
    const testErrorResponse = new HttpErrorResponse({
      error: {
        errors: {
          manufacturingYear: ['Год производства должен быть меньше текущего', 'Год производства введён некорректно'],
          trakerId: ['Трекер с ID 10 не найден']
        },
        message: 'Произошла ошибка валидации запроса'
      },
      status: 400,
      statusText: 'Bad Request',
      url: 'https://bioton-fms.ru/api/api/telematica/vehicle/1'
    });

    handler.handleError(testErrorResponse);

    const [
      [manufacturingYearMaxErrorMessage, manufacturingYearPatternErrorMessage],
      [trakerIDErrorMessage]
    ] = Object.values(testErrorResponse.error.errors as {
      [key: string]: string[]
    });

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(
        `${manufacturingYearMaxErrorMessage} ${manufacturingYearPatternErrorMessage} ${trakerIDErrorMessage}`,
        ERROR_ACTION,
        snackBarConfig
      );

    expect(console.error)
      .toHaveBeenCalledWith(testErrorResponse);
  });
});
