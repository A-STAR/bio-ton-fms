import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { firstValueFrom } from 'rxjs';

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
        imports: [
          HttpClientTestingModule,
          RouterTestingModule,
          AppComponent
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
      .withContext('initialize `authenticated$`')
      .toBeResolvedTo(false);

    testSignIn(httpTestingController, authService);

    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('set `authenticated$` to `true`')
      .toBeResolvedTo(true);

    await firstValueFrom(authService.signOut$);

    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('set `authenticated$` to `false`')
      .toBeResolvedTo(false);
  });

  it('should render sidebar', async () => {
    let sidebarDe = fixture.debugElement.query(
      By.css('bio-sidebar')
    );

    expect(sidebarDe)
      .withContext('hide sidebar component in unauthenticated state')
      .toBeNull();

    testSignIn(httpTestingController, authService);
    fixture.detectChanges();

    sidebarDe = fixture.debugElement.query(
      By.css('bio-sidebar')
    );

    expect(sidebarDe)
      .withContext('render sidebar component in authenticated state')
      .not.toBeNull();
  });

  it('should hide sidebar', async () => {
    testSignIn(httpTestingController, authService);
    fixture.detectChanges();

    let sidebarDe = fixture.debugElement.query(
      By.css('bio-sidebar')
    );

    expect(sidebarDe)
      .withContext('render sidebar component in authenticated state')
      .not.toBeNull();

    await firstValueFrom(authService.signOut$);
    fixture.detectChanges();

    sidebarDe = fixture.debugElement.query(
      By.css('bio-sidebar')
    );

    expect(sidebarDe)
      .withContext('hide sidebar component in unauthenticated state')
      .toBeNull();
  });
});
