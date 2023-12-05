import { ErrorHandler } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { DEFAULT_ERROR_MESSAGE, ErrorHandler as ErrorHandlerClass, ERROR_ACTION, snackBarConfig } from './error.handler';

describe('ErrorHandler', () => {
  let handler: ErrorHandler;
  const snackBarSpy = jasmine.createSpyObj<MatSnackBar>('MatSnackBar', ['open']);

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

    // eslint-disable-next-line no-console
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

    // eslint-disable-next-line no-console
    expect(console.error)
      .toHaveBeenCalledWith(testRejectionError.rejection);
  });

  it('should handle error response with multiple errors', () => {
    let testErrorResponse = new HttpErrorResponse({
      error: 'Трекер машины с таким id не существует',
      status: 404,
      statusText: 'Not Found',
      // eslint-disable-next-line max-len
      url: 'https://bioton-fms.ru/api/telematica/messagesview/statistics?vehicleId=-3&periodStart=2023-11-14T21:00:00.000Z&periodEnd=2023-11-15T20:59:00.000Z&viewMessageType=dataMessage&parameterType=trackerData'
    });

    handler.handleError(testErrorResponse);

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(testErrorResponse.error, ERROR_ACTION, snackBarConfig);

    // eslint-disable-next-line no-console
    expect(console.error)
      .toHaveBeenCalledWith(testErrorResponse);

    testErrorResponse = new HttpErrorResponse({
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

    // eslint-disable-next-line no-console
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

    // eslint-disable-next-line no-console
    expect(console.error)
      .toHaveBeenCalledWith(testErrorResponse);
  });

  it('should handle error response', () => {
    const testErrorResponse = new HttpErrorResponse({
      error: {
        errors: {
          manufacturingYear: ['Год производства должен быть не более текущего', 'Год производства введён некорректно'],
          trackerId: ['Трекер с ID 10 не найден']
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
      [trackerIDErrorMessage]
    ] = Object.values(testErrorResponse.error.errors as {
      [key: string]: string[];
    });

    expect(snackBarSpy.open)
      .toHaveBeenCalledWith(
        `${manufacturingYearMaxErrorMessage} ${manufacturingYearPatternErrorMessage} ${trackerIDErrorMessage}`,
        ERROR_ACTION,
        snackBarConfig
      );

    // eslint-disable-next-line no-console
    expect(console.error)
      .toHaveBeenCalledWith(testErrorResponse);
  });
});
