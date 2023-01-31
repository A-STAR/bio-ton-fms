import { NgZone } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router, RouterLink } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonHarness } from '@angular/material/button/testing';

import DirectoryTechComponent, { navigation } from './directory-tech.component';

import routes from '../directory-tech.routes';

describe('DirectoryTechComponent', () => {
  let component: DirectoryTechComponent;
  let fixture: ComponentFixture<DirectoryTechComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          RouterTestingModule.withRoutes(routes[0].children!),
          MatSnackBarModule,
          DirectoryTechComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(DirectoryTechComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render navigation', async () => {
    const loader = TestbedHarnessEnvironment.loader(fixture);
    const ngZone = TestBed.inject(NgZone);
    const router = TestBed.inject(Router);

    // navigate to root
    await ngZone.run(async () => {
      await router.navigate([routes[0].children?.[0].path]);
    });

    fixture.detectChanges();

    const navigationDe = fixture.debugElement.query(
      By.css('nav')
    );

    expect(navigationDe)
      .withContext('render navigation element')
      .not.toBeNull();

    let activeItemDe = fixture.debugElement.query(
      By.css('nav > span')
    );

    expect(activeItemDe)
      .withContext('render active item element')
      .not.toBeNull();

    let activeItemIndex = 0;

    expect(activeItemDe.nativeElement.textContent)
      .withContext('render vehicles active item text')
      .toBe(navigation[activeItemIndex].title);

    const anchors = await loader.getAllHarnesses(
      MatButtonHarness.with({
        ancestor: 'nav',
        variant: 'flat',
        selector: 'a'
      })
    );

    expect(anchors.length)
      .withContext(`render ${navigation.length} anchors`)
      .toBe(navigation.length);

    const navigationAnchorTitles = await parallel(() => anchors.map(
      anchor => anchor.getText()
    ));

    expect(navigationAnchorTitles)
      .withContext('render navigation anchor titles')
      .toEqual(
        navigation.map(({ title }) => title)
      );

    const navigationAnchorDes = navigationDe.queryAll(
      By.directive(RouterLink)
    );

    let navigationAnchorAttributes = navigationAnchorDes.map(
      anchorDe => [
        anchorDe.nativeElement.getAttribute('ng-reflect-router-link'),
        anchorDe.nativeElement.getAttribute('hidden')
      ]
    );

    expect(navigationAnchorAttributes[activeItemIndex])
      .withContext('render active navigation anchor link hidden')
      .toEqual([navigation[activeItemIndex].link, '']);

    let testNavigation = navigation
      .slice()
      .map(({ link }) => [link, null]);

    navigationAnchorAttributes.splice(activeItemIndex, 1);
    testNavigation.splice(activeItemIndex, 1);

    expect(navigationAnchorAttributes)
      .withContext('render navigation anchor links')
      .toEqual(testNavigation);

    const TRACKER_ID = 1;

    // navigate to tracker
    await ngZone.run(async () => {
      await router.navigate([
        routes[0].children?.[2].path?.split('/')[0],
        TRACKER_ID
      ]);
    });

    fixture.detectChanges();

    activeItemDe = fixture.debugElement.query(
      By.css('nav > span')
    );

    expect(activeItemDe)
      .withContext('render no active item element')
      .toBeNull();

    navigationAnchorAttributes = navigationAnchorDes.map(
      anchorDe => [
        anchorDe.nativeElement.getAttribute('ng-reflect-router-link'),
        anchorDe.nativeElement.getAttribute('hidden')
      ]
    );

    testNavigation = navigation.map(({ link }) => [link, null]);

    expect(navigationAnchorAttributes)
      .withContext('render navigation anchor links')
      .toEqual(testNavigation);
  });
});
