import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';

import { SignInComponent } from './sign-in.component';

describe('SignInComponent', () => {
  let component: SignInComponent;
  let fixture: ComponentFixture<SignInComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          RouterTestingModule,
          SignInComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SignInComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    fixture.detectChanges();
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
    loader.getHarness(MatCardHarness.with({
      title: 'Вход в личный кабинет'
    }));

    const logoAnchorDe = fixture.debugElement.query(By.css('a'));

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
  });
});
