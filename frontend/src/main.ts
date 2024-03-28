import { ErrorHandler, LOCALE_ID, importProvidersFrom } from '@angular/core';
import { ApplicationConfig, bootstrapApplication } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DATE_PIPE_DEFAULT_OPTIONS, registerLocaleData } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import localeRu from '@angular/common/locales/ru';
import { provideRouter } from '@angular/router';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarConfig, MatSnackBarModule } from '@angular/material/snack-bar';
import { MAT_DIALOG_DEFAULT_OPTIONS, MatDialogConfig, MatDialogModule } from '@angular/material/dialog';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';

import {
  LuxonDateAdapter,
  MAT_LUXON_DATE_ADAPTER_OPTIONS,
  MAT_LUXON_DATE_FORMATS,
  MatLuxonDateAdapterOptions
} from '@angular/material-luxon-adapter';

import { ErrorHandler as ErrorHandlerClass } from './app/error.handler';
import { AuthInterceptor } from './app/auth.interceptor';
import { APIInterceptor } from './app/api.interceptor';

import { AppComponent } from './app/app.component';

import { routes } from './app/app.routes';

const luxonDateAdapterOptions: MatLuxonDateAdapterOptions = {
  useUtc: false,
  firstDayOfWeek: 1
};

const snackBarOptions: MatSnackBarConfig = {
  duration: 4000
};

const dialogOptions: MatDialogConfig = {
  width: '1080px',
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
    importProvidersFrom(BrowserAnimationsModule, HttpClientModule, MatSnackBarModule, MatDialogModule),
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
      provide: DATE_PIPE_DEFAULT_OPTIONS,
      useValue: {
        dateFormat: 'd MMMM y, H:mm'
      }
    },
    {
      provide: DateAdapter,
      useClass: LuxonDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS,
      useValue: MAT_LUXON_DATE_FORMATS
    },
    {
      provide: MAT_LUXON_DATE_ADAPTER_OPTIONS,
      useValue: luxonDateAdapterOptions
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
