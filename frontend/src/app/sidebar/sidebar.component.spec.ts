import { By } from '@angular/platform-browser';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatLegacyNavListHarness as MatNavListHarness } from '@angular/material/legacy-list/testing';

import { AuthService } from '../auth.service';

import { NAVIGATION, NavigationButtonType, SidebarComponent } from './sidebar.component';

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          RouterTestingModule,
          SidebarComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SidebarComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

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

  it('should render navigation', async () => {
    const navigationLists = await loader.getAllHarnesses(MatNavListHarness);

    const [appNavigationGroups, userNavigationGroups] = await parallel(() => navigationLists.map(
      list => list.getItemsGroupedByDividers()
    ));

    expect(appNavigationGroups.length)
      .withContext(`render ${NAVIGATION[0].length} app navigation groups`)
      .toBe(NAVIGATION[0].length);

    expect(userNavigationGroups.length)
      .withContext(`render ${NAVIGATION[1].length} user navigation groups`)
      .toBe(NAVIGATION[1].length);

    const navigationItems = await parallel(() => [appNavigationGroups, userNavigationGroups].map(
      list => parallel(() => list.map(
        group => parallel(() => group.map(
          item => parallel(() => [
            item.getText(),
            item.getHref(),
            item.hasIcon()
          ]))
        ))
      ))
    );

    expect(navigationItems)
      .withContext('render navigation items icon, title and link')
      .toEqual(
        NAVIGATION.map(
          list => list.map(
            group => group.map(({ title, link }) => [
              title,
              link ?? null,
              true
            ])
          )
        )
      );
  });

  it('should handle sign out button action', async () => {
    const [, userNavigationList] = await loader.getAllHarnesses(MatNavListHarness);

    const [signOutNavigationItem] = await userNavigationList.getItems({
      selector: 'button',
      text: 'Выход'
    });

    const router = TestBed.inject(Router);
    const authService = TestBed.inject(AuthService);

    spyOn(component, 'onNavigationButtonClick')
      .and.callThrough();

    const signOutSpy = spyOnProperty(authService, 'signOut$')
      .and.callThrough();

    spyOn(router, 'navigate');

    await signOutNavigationItem.click();

    expect(component.onNavigationButtonClick)
      .toHaveBeenCalledWith(NavigationButtonType.SignOut);

    expect(signOutSpy)
      .toHaveBeenCalled();

    expect(router.navigate)
      .toHaveBeenCalledWith(['/sign-in'], {
        replaceUrl: true
      });
  });
});
