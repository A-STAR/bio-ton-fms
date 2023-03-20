import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackerParametersHistoryDialogComponent } from './tracker-parameters-history-dialog.component';

describe('TrackerParametersHistoryDialogComponent', () => {
  let component: TrackerParametersHistoryDialogComponent;
  let fixture: ComponentFixture<TrackerParametersHistoryDialogComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TrackerParametersHistoryDialogComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerParametersHistoryDialogComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });
});
