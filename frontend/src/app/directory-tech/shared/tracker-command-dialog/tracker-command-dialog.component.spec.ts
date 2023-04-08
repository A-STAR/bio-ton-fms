import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { MAT_DIALOG_DATA, MatDialogTitle } from '@angular/material/dialog';

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

  it('should render dialog title', async () => {
    let titleDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(titleDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    let vehicleDe = titleDe.query(
      By.css('strong')
    );

    expect(vehicleDe)
      .withContext('render dialog vehicle element')
      .not.toBeNull();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog title text with vehicle')
      .toBe(`Отправить команду на ${testMatDialogData.vehicle}`);

    fixture = TestBed.createComponent(TrackerCommandDialogComponent);

    component = fixture.componentInstance;

    const { vehicle, ...data } = testMatDialogData;

    component['data'] = data;

    fixture.detectChanges();
    await fixture.whenStable();

    titleDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    vehicleDe = titleDe.query(
      By.css('strong')
    );

    expect(vehicleDe)
      .withContext('render no dialog vehicle element')
      .toBeNull();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog title text without vehicle')
      .toBe('Отправить команду');
  });
});

const testMatDialogData: TrackerCommandDialogData = {
  id: TEST_TRACKER_ID,
  vehicle: testNewVehicle.name
};
