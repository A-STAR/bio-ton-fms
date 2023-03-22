import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable, of } from 'rxjs';

import { TrackerParametersHistory, TrackerService } from '../tracker.service';

import { TrackerParametersHistoryDialogComponent } from './tracker-parameters-history-dialog.component';

import { testParametersHistory, TEST_TRACKER_ID } from '../tracker.service.spec';

describe('TrackerParametersHistoryDialogComponent', () => {
  let component: TrackerParametersHistoryDialogComponent;
  let fixture: ComponentFixture<TrackerParametersHistoryDialogComponent>;

  let parametersHistorySpy: jasmine.Spy<(this: TrackerService) => Observable<TrackerParametersHistory>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientModule,
          TrackerParametersHistoryDialogComponent
        ],
        providers: [
          {
            provide: MAT_DIALOG_DATA,
            useValue: TEST_TRACKER_ID
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerParametersHistoryDialogComponent);

    component = fixture.componentInstance;

    const trackerService = TestBed.inject(TrackerService);

    const parametersHistory$ = of(testParametersHistory);

    parametersHistorySpy = spyOn(trackerService, 'getParametersHistory')
      .and.returnValue(parametersHistory$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get parameters history', () => {
    expect(parametersHistorySpy)
      .toHaveBeenCalled();
  });
});
