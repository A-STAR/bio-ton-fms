import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpHeaders } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';
import { MatDividerHarness } from '@angular/material/divider/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatLegacyButtonHarness as MatButtonHarness } from '@angular/material/legacy-button/testing';

import { AuthService } from '../auth.service';

import SignInComponent from './sign-in.component';

import { testCredentials, testCredentialsResponse } from '../auth.service.spec';

describe('SignInComponent', () => {
  let component: SignInComponent;
  let fixture: ComponentFixture<SignInComponent>;
  let httpTestingController: HttpTestingController;
  let router: Router;
  let loader: HarnessLoader;
  let authService: AuthService;

  const testVersion = '1.0.0';

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          RouterTestingModule,
          SignInComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SignInComponent);
    httpTestingController = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    loader = TestbedHarnessEnvironment.loader(fixture);
    authService = TestBed.inject(AuthService);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  beforeEach(() => {
    const systemVersionRequest = httpTestingController.expectOne('/system/get-version');

    systemVersionRequest.flush(testVersion);
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render card', async () => {
    const cards = await loader.getAllHarnesses(MatCardHarness);

    expect(cards.length)
      .withContext('render a single card')
      .toBe(1);
  });

  it('should render card title group', async () => {
    const card = await loader.getHarness(MatCardHarness.with({
      title: 'Вход в личный кабинет'
    }));

    const logoDivisionDe = fixture.debugElement.query(By.css('div.logo'));
    const logoAnchorDe = logoDivisionDe.query(By.css('a'));

    expect(logoAnchorDe)
      .withContext('render logo anchor element')
      .not.toBeNull();

    expect(logoAnchorDe.nativeElement.getAttribute('routerlink'))
      .withContext('render logo anchor router link')
      .toBe('/');

    expect(logoAnchorDe.nativeElement.getAttribute('title'))
      .withContext('render logo anchor title')
      .toBe('Bio-Ton Field Management System');

    const logoImageDe = logoAnchorDe.query(By.css('img'));

    expect(logoImageDe)
      .withContext('render logo image element')
      .not.toBeNull();

    expect(logoImageDe.nativeElement.src)
      .withContext('render logo image source')
      .toMatch('assets/images/bio-ton-field-management-system-logo-green.svg$');

    expect(logoImageDe.nativeElement.alt)
      .withContext('render logo image alternate text')
      .toBe('Bio-Ton Field Management System');

    expect(logoImageDe.nativeElement.getAttribute('width'))
      .withContext('render logo image width')
      .toBe('149');

    expect(logoImageDe.nativeElement.getAttribute('height'))
      .withContext('render logo image height')
      .toBe('44');

    const versionSpanDe = logoDivisionDe.query(By.css('.version'));

    expect(versionSpanDe.nativeElement.textContent)
      .withContext('render an app version')
      .toBe(`v. ${testVersion}`);

    const divider = await card.getHarness(MatDividerHarness);

    await expectAsync(divider.getOrientation())
      .withContext('render horizontal divider orientation')
      .toBeResolvedTo('horizontal');

    await expectAsync(divider.isInset())
      .withContext('render divider inset')
      .toBeResolvedTo(true);
  });

  it('should render Sign in form', async () => {
    const signInFormDe = fixture.debugElement.query(By.css('form#sign-in-form'));

    expect(signInFormDe)
      .withContext('render Sign in form element')
      .not.toBeNull();

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#sign-in-form',
      placeholder: 'Логин'
    }));

    const passwordInput = await loader.getHarness(MatInputHarness.with({
      ancestor: 'form#sign-in-form',
      placeholder: 'Пароль'
    }));

    await expectAsync(passwordInput.getType())
      .withContext('render password input type')
      .toBeResolvedTo('password');
  });

  it('should toggle password visibility', async () => {
    const passwordVisibilityButton = await loader.getHarness(MatButtonHarness.with({
      ancestor: 'form#sign-in-form label[for="password"] + mat-form-field',
      selector: '[matIconSuffix]'
    }));

    await passwordVisibilityButton.click();

    const passwordInput = await loader.getHarness(MatInputHarness.with({
      ancestor: 'form#sign-in-form',
      placeholder: 'Пароль'
    }));

    await expectAsync(passwordInput.getType())
      .withContext('render password input type as text')
      .toBeResolvedTo('text');

    await passwordVisibilityButton.click();

    await expectAsync(passwordInput.getType())
      .withContext('render password input type as password')
      .toBeResolvedTo('password');
  });

  it('should submit invalid Sign in form', async () => {
    const card = await loader.getHarness(MatCardHarness.with({
      title: 'Вход в личный кабинет'
    }));

    spyOn(authService, 'signIn')
      .and.callThrough();

    spyOn(router, 'navigate');

    const signInButton = await card.getHarness(MatButtonHarness.with({
      selector: '[form="sign-in-form"]'
    }));

    await signInButton.click();

    expect(authService.signIn)
      .not.toHaveBeenCalled();

    expect(router.navigate)
      .not.toHaveBeenCalled();
  });

  it('should submit Sign in form with error response', async () => {
    const card = await loader.getHarness(MatCardHarness.with({
      title: 'Вход в личный кабинет'
    }));

    const [usernameInput, passwordInput] = await card.getAllHarnesses(MatInputHarness.with({
      ancestor: 'form#sign-in-form'
    }));

    const testMalformedCredentials = {
      ...testCredentials,
      username: testCredentials.username.slice(0, -1)
    };

    await usernameInput.setValue(testMalformedCredentials.username);
    await passwordInput.setValue(testMalformedCredentials.password);

    spyOn(authService, 'signIn')
      .and.callThrough();

    spyOn(router, 'navigate');

    const signInButton = await card.getHarness(MatButtonHarness.with({
      selector: '[form="sign-in-form"]'
    }));

    await signInButton.click();

    expect(authService.signIn)
      .toHaveBeenCalledWith(testMalformedCredentials);

    let loginRequest = httpTestingController.expectOne({
      method: 'POST',
      url: '/api/auth/login'
    }, 'error login request');

    const testErrorResponse = 'Неверный логин или пароль';

    const testRequestErrorOptions: {
      headers?: HttpHeaders | {
        [name: string]: string | string[];
      };
      status?: number;
      statusText?: string;
    } = {
      status: 400,
      statusText: 'Bad Request'
    };

    loginRequest.flush(testErrorResponse, testRequestErrorOptions);
    fixture.detectChanges();

    expect(router.navigate)
      .not.toHaveBeenCalled();

    const errorDe = fixture.debugElement.query(By.css('form#sign-in-form > mat-error'));

    expect(errorDe.nativeElement.textContent.trim())
      .withContext('render form error message')
      .toBe(testErrorResponse);

    httpTestingController.verify();
  });

  it('should submit Sign in form', async () => {
    const card = await loader.getHarness(MatCardHarness.with({
      title: 'Вход в личный кабинет'
    }));

    const [usernameInput, passwordInput] = await card.getAllHarnesses(MatInputHarness.with({
      ancestor: 'form#sign-in-form'
    }));

    await usernameInput.setValue(testCredentials.username);
    await passwordInput.setValue(testCredentials.password);

    spyOn(authService, 'signIn')
      .and.callThrough();

    spyOn(router, 'navigate');

    const signInButton = await card.getHarness(MatButtonHarness.with({
      selector: '[form="sign-in-form"]'
    }));

    await signInButton.click();

    expect(authService.signIn)
      .toHaveBeenCalledWith(testCredentials);

    const loginRequest = httpTestingController.expectOne({
      method: 'POST',
      url: '/api/auth/login'
    }, 'login request');

    loginRequest.flush(testCredentialsResponse);

    expect(router.navigate)
      .toHaveBeenCalledWith(['/'], {
        replaceUrl: true
      });

    httpTestingController.verify();
  });
});
