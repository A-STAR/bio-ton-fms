import { By } from '@angular/platform-browser';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { SidebarComponent } from './sidebar.component';

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          RouterTestingModule,
          SidebarComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SidebarComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render logo', () => {
    const logoDivisionDe = fixture.debugElement.query(By.css('div.logo'));

    expect(logoDivisionDe)
      .withContext('render logo element')
      .not.toBeNull();

    const logoAnchorDe = logoDivisionDe.query(By.css('a'));

    expect(logoDivisionDe)
      .withContext('render logo anchor element')
      .not.toBeNull();

    expect(logoAnchorDe.nativeElement.getAttribute('title'))
      .withContext('render logo anchor title')
      .toBe('Bio-Ton Field Management System');

    const [sunflowerLogoImageDe, logoImageDe] = logoAnchorDe.queryAll(By.css('img'));

    expect(sunflowerLogoImageDe)
      .withContext('render sunflower logo image element')
      .toBeDefined();

    expect(sunflowerLogoImageDe.nativeElement.src)
      .withContext('render sunflower logo image source')
      .toMatch('assets/images/bio-ton-field-management-system-logo-sunflower.svg$');

    expect(sunflowerLogoImageDe.nativeElement.alt)
      .withContext('render sunflower logo image alternate text')
      .toBe('Bio-Ton Field Management System');

    expect(sunflowerLogoImageDe.nativeElement.getAttribute('width'))
      .withContext('render sunflower logo image width')
      .toBe('42');

    expect(sunflowerLogoImageDe.nativeElement.getAttribute('height'))
      .withContext('render sunflower logo image height')
      .toBe('44');

    expect(logoImageDe)
      .withContext('render logo element')
      .toBeDefined();

    expect(logoImageDe.nativeElement.src)
      .withContext('render logo image source')
      .toMatch('assets/images/bio-ton-field-management-system-logo-white.svg$');

    expect(logoImageDe.nativeElement.alt)
      .withContext('render logo image alternate text')
      .toBe('Bio-Ton Field Management System');

    expect(logoImageDe.nativeElement.getAttribute('width'))
      .withContext('render logo image width')
      .toBe('149');

    expect(logoImageDe.nativeElement.getAttribute('height'))
      .withContext('render logo image height')
      .toBe('44');
  });
});
