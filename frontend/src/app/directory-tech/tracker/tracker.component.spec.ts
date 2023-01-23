import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ActivatedRoute, convertToParamMap, Params } from '@angular/router';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';

import { of } from 'rxjs';

import TrackerComponent from './tracker.component';

describe('TrackerComponent', () => {
  let component: TrackerComponent;
  let fixture: ComponentFixture<TrackerComponent>;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TrackerComponent],
        providers: [
          {
            provide: ActivatedRoute,
            useValue: testActivatedRoute
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerComponent);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render Tracker page heading', () => {
    const headingDe = fixture.debugElement.query(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render heading element')
      .not.toBeNull();

    expect(headingDe.nativeElement.textContent)
      .withContext('render heading text')
      .toBe('Конфигурация данных GPS-трекера');
  });

  it('should render sensors card', async () => {
    const card = await loader.getHarness(MatCardHarness.with({
      title: 'Дополнительные параметры'
    }));

    expect(card)
      .withContext('render sensors card')
      .not.toBeNull();
  });
});

const TRACKER_ID = 1;

const testParams: Params = {
  id: TRACKER_ID
};

const testParamMap = convertToParamMap(testParams);

const testActivatedRoute = {
  params: testParams,
  get paramMap() {
    return of(testParamMap);
  }
} as Pick<ActivatedRoute, 'params' | 'paramMap'>;
