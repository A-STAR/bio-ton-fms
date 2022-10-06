import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';
import { MatDividerHarness } from '@angular/material/divider/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatButtonHarness } from '@angular/material/button/testing';

import { SignInComponent } from './sign-in.component';

describe('SignInComponent', () => {
  let component: SignInComponent;
  let fixture: ComponentFixture<SignInComponent>;
  let httpTestingController: HttpTestingController;
  let loader: HarnessLoader;

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
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  beforeEach(() => {
    const systemVersionRequest = httpTestingController.expectOne('/system/get-version');

    systemVersionRequest.flush(testVersion);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
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

    const logoDivDe = fixture.debugElement.query(By.css('.logo'));
    const logoAnchorDe = logoDivDe.query(By.css('a'));

    expect(logoAnchorDe)
      .withContext('render logo anchor element')
      .toBeDefined();

    expect(logoAnchorDe.nativeElement.getAttribute('routerlink'))
      .withContext('render logo anchor router link')
      .toBe('/');

    const logoImageDe = logoAnchorDe.query(By.css('img'));

    expect(logoImageDe)
      .withContext('render logo image element')
      .toBeDefined();

    expect(logoImageDe.nativeElement.src)
      .withContext('render logo image source')
      .toMatch('assets/images/bio-ton-field-management-system-logo-green.svg$');

    expect(logoImageDe.nativeElement.alt)
      .withContext('render logo image alternate text')
      .toBe('Bio-Ton Field Management System');

    expect(logoImageDe.nativeElement.width)
      .withContext('render logo image width')
      .toBe(147);

    expect(logoImageDe.nativeElement.height)
      .withContext('render logo image height')
      .toBe(44);

    const versionSpanDe = logoDivDe.query(By.css('.version'));

    expect(versionSpanDe.nativeElement.textContent)
      .withContext('render an app version')
      .toBe(`v. ${testVersion}`);

    card.getHarness(MatDividerHarness);
  });

  it('should render Sign in form', async () => {
    const formDe = fixture.debugElement.query(By.css('form#sign-in-form'));

    expect(formDe)
      .withContext('render Sign in form element')
      .toBeDefined();

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
      selector: '[matSuffix]'
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
});
