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
import { MatDialogModule } from '@angular/material/dialog';

import DirectoryTechComponent, { navigation } from './directory-tech.component';

import routes from '../directory-tech.routes';
import { TEST_TRACKER_ID } from '../tracker.service.spec';

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
          MatDialogModule,
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

    const navigationDe = fixture.debugElement.query(
      By.css('nav')
    );

    expect(navigationDe)
      .withContext('render navigation element')
      .not.toBeNull();

    const buttons = await loader.getAllHarnesses(
      MatButtonHarness.with({
        ancestor: 'nav',
        variant: 'flat',
        selector: 'a'
      })
    );

    expect(buttons.length)
      .withContext(`render ${navigation.length} buttons`)
      .toBe(navigation.length);

    const navigationButtonTexts = await parallel(() => buttons.map(
      button => button.getText()
    ));

    expect(navigationButtonTexts)
      .withContext('render navigation button texts')
      .toEqual(
        navigation.map(({ title }) => title)
      );

    const navigationLinkDes = navigationDe.queryAll(
      By.directive(RouterLink)
    );

    let navigationLinkAttributes = navigationLinkDes.map(
      linkDe => linkDe.nativeElement.getAttribute('ng-reflect-router-link')
    );

    let testNavigation = navigation.map(({ link }) => link);

    expect(navigationLinkAttributes)
      .withContext('render navigation link attributes')
      .toEqual(testNavigation);

    const ngZone = TestBed.inject(NgZone);
    const router = TestBed.inject(Router);

    // navigate to root
    await ngZone.run(async () => {
      await router.navigate([routes[0].children?.[0].path]);
    });

    fixture.detectChanges();

    let activeButton = await loader.getHarnessOrNull(
      MatButtonHarness.with({
        ancestor: 'nav',
        variant: 'flat',
        selector: 'a.active',
        text: navigation[0].title
      })
    );

    expect(activeButton)
      .withContext(`render active ${navigation[0].title} button`)
      .not.toBeNull();

    // navigate to tracker
    await ngZone.run(async () => {
      await router.navigate([
        routes[0].children?.[2].path?.split('/')[0],
        TEST_TRACKER_ID
      ]);
    });

    fixture.detectChanges();

    activeButton = await loader.getHarnessOrNull(
      MatButtonHarness.with({
        ancestor: 'nav',
        variant: 'flat',
        selector: 'a.active',
        text: navigation[1].title
      })
    );

    expect(activeButton)
      .withContext(`render active ${navigation[1].title} button`)
      .not.toBeNull();
  });
});
