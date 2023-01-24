import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap, Params } from '@angular/router';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';

import { Observable, of } from 'rxjs';

import { Sensors, TrackerService } from '../tracker.service';

import TrackerComponent from './tracker.component';

import { testSensors, TEST_TRACKER_ID } from '../tracker.service.spec';

describe('TrackerComponent', () => {
  let component: TrackerComponent;
  let fixture: ComponentFixture<TrackerComponent>;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let trackerService: TrackerService;

  let sensorsSpy: jasmine.Spy<() => Observable<Sensors>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          TrackerComponent,
          HttpClientTestingModule
        ],
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
    trackerService = TestBed.inject(TrackerService);

    component = fixture.componentInstance;

    const sensors$ = of(testSensors);

    sensorsSpy = spyOn(trackerService, 'getSensors')
      .and.returnValue(sensors$);

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

  it('should get sensors', () => {
    expect(sensorsSpy)
      .toHaveBeenCalled();
  });
});

const testParams: Params = {
  id: TEST_TRACKER_ID
};

const testParamMap = convertToParamMap(testParams);

const testActivatedRoute = {
  params: testParams,
  get paramMap() {
    return of(testParamMap);
  }
} as Pick<ActivatedRoute, 'params' | 'paramMap'>;
