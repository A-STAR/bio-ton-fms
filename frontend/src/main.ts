import { ErrorHandler, importProvidersFrom } from '@angular/core';
import { ApplicationConfig, bootstrapApplication } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import {
  MatLegacySnackBarConfig as MatSnackBarConfig,
  MatLegacySnackBarModule as MatSnackBarModule,
  MAT_LEGACY_SNACK_BAR_DEFAULT_OPTIONS as MAT_SNACK_BAR_DEFAULT_OPTIONS
} from '@angular/material/legacy-snack-bar';

import {
  MatLegacyDialogConfig as MatDialogConfig,
  MAT_LEGACY_DIALOG_DEFAULT_OPTIONS as MAT_DIALOG_DEFAULT_OPTIONS
} from '@angular/material/legacy-dialog';

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

bootstrapApplication(AppComponent, appConfig);
