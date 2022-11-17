import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MatSnackBarConfig, MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { MatDialogConfig, MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';
import { MatFormFieldDefaultOptions, MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';

import { AuthInterceptor } from './auth.interceptor';
import { APIInterceptor } from './api.interceptor';

import { AppRoutingModule } from './app-routing.module';

import { SidebarComponent } from './sidebar/sidebar.component';

import { AppComponent } from './app.component';

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

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    SidebarComponent
  ],
  providers: [
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
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
