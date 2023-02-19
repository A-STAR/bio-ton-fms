import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of } from 'rxjs';

import { NewTracker, TrackerService, TrackerTypeEnum } from '../tracker.service';

import { NumberOnlyInputDirective } from 'src/app/shared/number-only-input/number-only-input.directive';
import { TrackerDialogComponent, TRACKER_CREATED } from './tracker-dialog.component';

import { testNewTracker, testTrackerTypeEnum } from '../tracker.service.spec';

describe('TrackerDialogComponent', () => {
  let component: TrackerDialogComponent;
  let fixture: ComponentFixture<TrackerDialogComponent>;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let trackerService: TrackerService;

  const dialogRef = jasmine.createSpyObj<MatDialogRef<TrackerDialogComponent, true | '' | undefined>>('MatDialogRef', ['close']);

  let trackerTypeSpy: jasmine.Spy<(this: TrackerService) => Observable<KeyValue<TrackerTypeEnum, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatSnackBarModule,
          TrackerDialogComponent
        ],
        providers: [
          {
            provide: MatDialogRef,
            useValue: dialogRef
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerDialogComponent);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);
    trackerService = TestBed.inject(TrackerService);

    component = fixture.componentInstance;

    const trackerType$ = of(testTrackerTypeEnum);

    trackerTypeSpy = spyOnProperty(trackerService, 'trackerType$')
      .and.returnValue(trackerType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get tracker type enum', () => {
    expect(trackerTypeSpy)
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
      .toBe('Добавление GPS-трекера');
  });

  it('should render tracker form', async () => {
    const trackerFormDe = fixture.debugElement.query(
      By.css('form#tracker-form')
    );

    expect(trackerFormDe)
      .withContext('render Tracker form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Наименование GPS-трекера'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#tracker-form',
        selector: '[placeholder="Тип устройства"]'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Время начала'
      })
    );

    const externalInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Внешний ID'
      })
    );

    await expectAsync(
      externalInput.getType()
    )
      .withContext('render external ID input type')
      .toBeResolvedTo('number');

    const externalInputDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    expect(externalInputDe.nativeElement.placeholder)
      .withContext('render external ID input with `NumberOnlyDirective`')
      .toBe('Внешний ID');

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        selector: '[type="tel"]',
        placeholder: 'Номер SIM-карты'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'IMEI номер'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Описание'
      })
    );
  });

  it('should submit invalid vehicle form', async () => {
    spyOn(trackerService, 'createTracker')
      .and.callThrough();

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="tracker-form"]',
        text: 'Сохранить'
      })
    );

    await saveButton.click();

    expect(trackerService.createTracker)
      .not.toHaveBeenCalled();
  });

  it('should submit create tracker form', async () => {
    const [nameInput, startInput, externalInput, simInput, imeiInput] = await loader.getAllHarnesses(
      MatInputHarness.with({
        ancestor: 'form#tracker-form'
      })
    );

    const { name, externalId, simNumber, imei } = testNewTracker;

    await nameInput.setValue(name);

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#tracker-form'
      })
    );

    await typeSelect.clickOptions({
      text: testTrackerTypeEnum[0].value
    });

    const testDate = new Date();
    const testLocaleTime = testDate.toLocaleTimeString('ru-RU');

    let testStart = `${testDate.toLocaleDateString('ru-RU')} ${testLocaleTime}`;

    await startInput.setValue(testStart);

    const external = externalId.toString();

    await externalInput.setValue(external);
    await simInput.setValue(simNumber);
    await imeiInput.setValue(imei);

    spyOn(trackerService, 'createTracker')
      .and.callFake(() => of({}));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="tracker-form"]'
      })
    );

    await saveButton.click();

    const [day, month, year, hours, minutes, seconds = 0] = testStart
      .split(/[\.\s:]/)
      .map(Number);

    const monthIndex = month - 1;

    const startDate = new Date(year, monthIndex, day, hours, minutes, seconds)
      .toISOString();

    const testTracker: NewTracker = {
      externalId: testNewTracker.externalId,
      name: testNewTracker.name,
      simNumber: testNewTracker.simNumber,
      imei: testNewTracker.imei,
      trackerType: testNewTracker.trackerType,
      startDate,
      description: undefined
    };

    expect(trackerService.createTracker)
      .toHaveBeenCalledWith(testTracker);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(TRACKER_CREATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    /* Coverage for `start` seconds default value. */

    testStart = `${testDate.toLocaleDateString('ru-RU')} ${testLocaleTime.slice(0, -3)}`;

    await startInput.setValue(testStart);

    await saveButton.click();
  });
});
