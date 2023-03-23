import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { MatDialogTitle, MAT_DIALOG_DATA } from '@angular/material/dialog';

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

  it('should render dialog title', async () => {
    const titleTextDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(titleTextDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleTextDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('История значений параметров');
  });
});
