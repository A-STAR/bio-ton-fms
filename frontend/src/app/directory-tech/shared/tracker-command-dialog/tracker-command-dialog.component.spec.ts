import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MAT_DIALOG_DATA, MatDialogContent, MatDialogTitle } from '@angular/material/dialog';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatButtonToggleGroupHarness, MatButtonToggleHarness } from '@angular/material/button-toggle/testing';

import { TrackerCommandDialogComponent, TrackerCommandDialogData } from './tracker-command-dialog.component';

import { TEST_TRACKER_ID } from '../../tracker.service.spec';
import { testNewVehicle } from '../../vehicle.service.spec';

describe('TrackerCommandDialogComponent', () => {
  let component: TrackerCommandDialogComponent;
  let fixture: ComponentFixture<TrackerCommandDialogComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          TrackerCommandDialogComponent
        ],
        providers: [
          {
            provide: MAT_DIALOG_DATA,
            useValue: testMatDialogData
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerCommandDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

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

  it('should render command form', async () => {
    const dialogContentDe = fixture.debugElement.query(
      By.directive(MatDialogContent)
    );

    expect(dialogContentDe)
      .withContext('render dialog content element')
      .not.toBeNull();

    const commandFormDe = dialogContentDe.query(
      By.css('form#command-form')
    );

    expect(commandFormDe)
      .withContext('render command form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#command-form',
        placeholder: 'Сообщение'
      })
    );

    const transport = await loader.getHarness(
      MatButtonToggleGroupHarness.with({
        ancestor: 'form#command-form',
        selector: '[aria-label="Протокол передачи"]'
      })
    );

    await expectAsync(
      transport.isVertical()
    )
      .withContext('render transport toggle vertically')
      .toBeResolvedTo(true);

    loader.getHarness(
      MatButtonToggleHarness.with({
        ancestor: 'form#command-form',
        text: 'TCP',
        checked: true
      })
    );

    loader.getHarness(
      MatButtonToggleHarness.with({
        ancestor: 'form#command-form',
        text: 'SMS',
        checked: false
      })
    );
  });
});

const testMatDialogData: TrackerCommandDialogData = {
  id: TEST_TRACKER_ID,
  vehicle: testNewVehicle.name
};
