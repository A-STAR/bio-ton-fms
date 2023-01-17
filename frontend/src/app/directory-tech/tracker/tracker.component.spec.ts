import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, Params } from '@angular/router';

import { of } from 'rxjs';

import TrackerComponent from './tracker.component';

describe('TrackerComponent', () => {
  let component: TrackerComponent;
  let fixture: ComponentFixture<TrackerComponent>;

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
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
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
