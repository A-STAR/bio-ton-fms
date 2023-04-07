import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { TrackerCommandDialogComponent, TrackerCommandDialogData } from './tracker-command-dialog.component';

import { TEST_TRACKER_ID } from '../../tracker.service.spec';
import { testNewVehicle } from '../../vehicle.service.spec';

describe('TrackerCommandDialogComponent', () => {
  let component: TrackerCommandDialogComponent;
  let fixture: ComponentFixture<TrackerCommandDialogComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TrackerCommandDialogComponent],
        providers: [
          {
            provide: MAT_DIALOG_DATA,
            useValue: testMatDialogData
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerCommandDialogComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });
});

const testMatDialogData: TrackerCommandDialogData = {
  id: TEST_TRACKER_ID,
  vehicle: testNewVehicle.name
};
