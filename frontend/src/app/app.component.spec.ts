import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { firstValueFrom } from 'rxjs';

import { SidebarComponent } from './sidebar/sidebar.component';

import { AuthService } from './auth.service';

import { AppComponent } from './app.component';

import { TokenKey } from './token.service';
import { testSignIn } from './auth.service.spec';

describe('AppComponent', () => {
  let app: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpTestingController: HttpTestingController;
  let authService: AuthService;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        declarations: [AppComponent],
        imports: [
          HttpClientTestingModule,
          RouterTestingModule,
          SidebarComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    httpTestingController = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService);

    app = fixture.componentInstance;

    fixture.detectChanges();
  });

  afterEach(() => {
    localStorage.removeItem(TokenKey.Token);
    localStorage.removeItem(TokenKey.RefreshToken);
  });

  it('should create the app', () => {
    expect(app)
      .toBeTruthy();
  });

  it('should set authenticated state', async () => {
    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('initialize `autnenticated$`')
      .toBeResolvedTo(false);

    testSignIn(httpTestingController, authService);

    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('set `autnenticated$` to `true`')
      .toBeResolvedTo(true);

    await firstValueFrom(authService.signOut$);

    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('set `autnenticated$` to `false`')
      .toBeResolvedTo(false);
  });

  it('should render sidebar', async () => {
    let sidebar = fixture.debugElement.query(By.css('bio-sidebar'));

    expect(sidebar)
      .withContext('hide sidebar in unauthenticated state')
      .toBeNull();

    testSignIn(httpTestingController, authService);
    fixture.detectChanges();

    sidebar = fixture.debugElement.query(By.css('bio-sidebar'));

    expect(sidebar)
      .withContext('render sidebar in authenticated state')
      .not.toBeNull();
  });

  it('should hide sidebar', async () => {
    testSignIn(httpTestingController, authService);
    fixture.detectChanges();

    let sidebar = fixture.debugElement.query(By.css('bio-sidebar'));

    expect(sidebar)
      .withContext('render sidebar in authenticated state')
      .not.toBeNull();

    await firstValueFrom(authService.signOut$);
    fixture.detectChanges();

    sidebar = fixture.debugElement.query(By.css('bio-sidebar'));

    expect(sidebar)
      .withContext('hide sidebar in unauthenticated state')
      .toBeNull();
  });
});
