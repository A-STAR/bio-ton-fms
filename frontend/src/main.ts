import { ErrorHandler, importProvidersFrom, LOCALE_ID } from '@angular/core';
import { ApplicationConfig, bootstrapApplication } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { registerLocaleData } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import localeRu from '@angular/common/locales/ru';
import { provideRouter } from '@angular/router';
import { MatSnackBarConfig, MatSnackBarModule, MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { MatDialogConfig, MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';
import { MatFormFieldDefaultOptions, MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';

import { ErrorHandler as ErrorHandlerClass } from './app/error.handler';
import { AuthInterceptor } from './app/auth.interceptor';
import { APIInterceptor } from './app/api.interceptor';

import { AppComponent } from './app/app.component';

import { routes } from './app/app.routes';

const snackBarOptions: MatSnackBarConfig = {
  duration: 4000
};

const dialogOptions: MatDialogConfig = {
  width: '70vw',
  maxHeight: '85vh',
  autoFocus: 'dialog',
  restoreFocus: false
};

const formFieldOptions: MatFormFieldDefaultOptions = {
  appearance: 'outline',
  floatLabel: 'always'
};

const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(BrowserAnimationsModule, HttpClientModule, MatSnackBarModule),
    {
      provide: ErrorHandler,
      useClass: ErrorHandlerClass
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: APIInterceptor,
      multi: true
    },
    provideRouter(routes),
    {
      provide: LOCALE_ID,
      useValue: 'ru-RU'
    },
    {
      provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
      useValue: snackBarOptions
    },
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: dialogOptions
    },
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: formFieldOptions
    }
  ]
};

registerLocaleData(localeRu, 'ru-RU');

bootstrapApplication(AppComponent, appConfig);
