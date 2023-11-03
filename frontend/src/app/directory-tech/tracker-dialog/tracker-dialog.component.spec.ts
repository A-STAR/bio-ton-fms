import { ErrorHandler, LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { formatDate, KeyValue, registerLocaleData } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import localeRu from '@angular/common/locales/ru';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatDialogRef, MatDialogTitle, MAT_DIALOG_DATA, MatDialogContent, MatDialogClose } from '@angular/material/dialog';
import { MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of, throwError } from 'rxjs';

import { NewTracker, Tracker, TrackerService, TrackerTypeEnum } from '../tracker.service';

import { NumberOnlyInputDirective } from '../../shared/number-only-input/number-only-input.directive';
import { inputDateFormat, TrackerDialogComponent, TRACKER_CREATED, TRACKER_UPDATED } from './tracker-dialog.component';

import { environment } from '../../../environments/environment';
import { localeID } from '../../tech/shared/relative-time.pipe';
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
            provide: LOCALE_ID,
            useValue: localeID
          },
          {
            provide: MAT_DIALOG_DATA,
            useValue: undefined
          },
          {
            provide: MatDialogRef,
            useValue: dialogRef
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(TrackerDialogComponent);
    trackerService = TestBed.inject(TrackerService);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

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
    const titleDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(titleDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('Добавление GPS-трекера');

    component['data'] = testNewTracker;

    fixture.detectChanges();
    await fixture.whenStable();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog update title text')
      .toBe('Сводная информация о GPS-трекере');
  });

  it('should render tracker form', async () => {
    const dialogContentDe = fixture.debugElement.query(
      By.directive(MatDialogContent)
    );

    expect(dialogContentDe)
      .withContext('render dialog content element')
      .not.toBeNull();

    const trackerFormDe = dialogContentDe.query(
      By.css('form#tracker-form')
    );

    expect(trackerFormDe)
      .withContext('render tracker form element')
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

  it('should render dialog actions', async () => {
    const cancelButton = await loader.getHarnessOrNull(
      MatButtonHarness.with({
        text: 'Отмена',
        variant: 'stroked'
      })
    );

    expect(cancelButton)
      .withContext('render close button')
      .not.toBeNull();

    const dialogCloseDe = fixture.debugElement.query(
      By.directive(MatDialogClose)
    );

    expect(dialogCloseDe)
      .withContext('render dialog close element')
      .not.toBeNull();

    loader.getHarnessOrNull(
      MatButtonHarness.with({
        text: 'Отправить',
        variant: 'flat'
      })
    );
  });

  it('should render update tracker form', async () => {
    component['data'] = testNewTracker;

    component.ngOnInit();

    fixture.detectChanges();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Наименование GPS-трекера',
        value: testNewTracker.name
      })
    );

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#tracker-form',
        selector: '[placeholder="Тип устройства"]'
      })
    );

    await expectAsync(
      typeSelect.getValueText()
    )
      .withContext('render type select text')
      .toBeResolvedTo(testTrackerTypeEnum[0].value);

    const start = formatDate(testNewTracker.startDate!, inputDateFormat, localeID);

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Время начала',
        value: start
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Внешний ID',
        value: testNewTracker.externalId.toString()
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        selector: '[type="tel"]',
        placeholder: 'Номер SIM-карты',
        value: testNewTracker.simNumber
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'IMEI номер',
        value: testNewTracker.imei
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Описание',
        value: testNewTracker.description
      })
    );
  });

  it('should submit invalid tracker form', async () => {
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
    const testLocaleTime = testDate.toLocaleTimeString(localeID);

    const testStart = `${testDate.toLocaleDateString(localeID)} ${testLocaleTime.slice(0, -3)}`;

    await startInput.setValue(testStart);

    const external = externalId.toString();

    await externalInput.setValue(external);
    await simInput.setValue(simNumber);
    await imeiInput.setValue(imei);

    const [day, month, year, hours, minutes] = testStart
      .split(/[.\s:]/)
      .map(Number);

    const monthIndex = month - 1;

    const startDate = new Date(year, monthIndex, day, hours, minutes)
      .toISOString();

    const newTracker: NewTracker = {
      id: undefined,
      externalId: testNewTracker.externalId,
      name: testNewTracker.name,
      simNumber: testNewTracker.simNumber,
      imei: testNewTracker.imei,
      trackerType: testNewTracker.trackerType,
      startDate,
      description: undefined
    };

    const testTrackerResponse: Tracker = {
      id: testNewTracker.id!,
      externalId: testNewTracker.externalId,
      name: testNewTracker.name,
      simNumber: testNewTracker.simNumber,
      imei: testNewTracker.imei,
      trackerType: testTrackerTypeEnum[0],
      startDate: testNewTracker.startDate!
    };

    const createTrackerSpy = spyOn(trackerService, 'createTracker')
      .and.callFake(() => of(testTrackerResponse));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="tracker-form"]'
      })
    );

    await saveButton.click();

    expect(trackerService.createTracker)
      .toHaveBeenCalledWith(newTracker);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(TRACKER_CREATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(true);

    const url = `${environment.api}/api/telematica/tracker`;

    const nameErrorText = `GPS-трекер с именем ${testTrackerResponse.name} уже существует`;
    const externalIdErrorText = 'Внешний ID обязательное поле';

    let testErrorResponse = new HttpErrorResponse({
      error: {
        errors: {
          Name: [nameErrorText],
          ExternalId: [externalIdErrorText]
        }
      },
      status: 400,
      statusText: 'Bad Request',
      url
    });

    const errorHandler = TestBed.inject(ErrorHandler);

    spyOn(console, 'error');
    const handleErrorSpy = spyOn(errorHandler, 'handleError');

    createTrackerSpy.and.callFake(() => throwError(() => testErrorResponse));

    // test for the tracker request `400 Bad Request` error response
    await saveButton.click();

    const [nameFormField] = await loader.getAllHarnesses(
      MatFormFieldHarness.with({
        ancestor: 'form#tracker-form'
      })
    );

    const [nameError] = await nameFormField.getErrors();

    await expectAsync(
      nameError.getText()
    )
      .withContext('render name field error text')
      .toBeResolvedTo(nameErrorText);

    expect(handleErrorSpy)
      .toHaveBeenCalledWith(testErrorResponse);

    handleErrorSpy.calls.reset();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    testErrorResponse = new HttpErrorResponse({
      error: {
        messages: [
          `Имя GPS-трекера ${testTrackerResponse.name} зарезервировано`,
          `GPS-трекер уже имеет назначенные датчики`
        ]
      },
      status: 409,
      statusText: 'Conflict',
      url
    });

    createTrackerSpy.and.callFake(() => throwError(() => testErrorResponse));

    // test for the tracker request error response
    await saveButton.click();

    let formErrorDes = fixture.debugElement.queryAll(
      By.css('form > mat-error')
    );

    expect(formErrorDes.length)
      .withContext('render form error elements')
      .toBe(testErrorResponse.error.messages.length);

    formErrorDes.forEach((formErrorDe, index) => {
      expect(formErrorDe.nativeElement.textContent)
        .withContext('render form error element text')
        .toBe(testErrorResponse.error.messages[index]);
    });

    expect(handleErrorSpy)
      .not.toHaveBeenCalled();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    /* Coverage for a common server error fallback. */

    testErrorResponse = new HttpErrorResponse({
      error: `GPS-трекер с зарезервированным именем ${testTrackerResponse.name}`,
      status: 409,
      statusText: 'Conflict',
      url
    });

    createTrackerSpy.and.callFake(() => throwError(() => testErrorResponse));

    await saveButton.click();

    formErrorDes = fixture.debugElement.queryAll(
      By.css('form > mat-error')
    );

    expect(formErrorDes.length)
      .withContext('render form error element')
      .toBe(1);

    expect(formErrorDes[0].nativeElement.textContent)
      .withContext('render form error element text')
      .toBe(testErrorResponse.error);

    expect(handleErrorSpy)
      .not.toHaveBeenCalled();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    handleErrorSpy.calls.reset();

    /* Coverage for authorization error response */

    testErrorResponse = new HttpErrorResponse({
      status: 403,
      statusText: 'Forbidden',
      url
    });

    createTrackerSpy.and.callFake(() => throwError(() => testErrorResponse));

    await saveButton.click();

    formErrorDes = fixture.debugElement.queryAll(
      By.css('form > mat-error')
    );

    expect(formErrorDes.length)
      .withContext('render no form error element')
      .toBe(0);

    expect(handleErrorSpy)
      .toHaveBeenCalledWith(testErrorResponse);
  });

  it('should submit update tracker form', async () => {
    component['data'] = testNewTracker;

    component.ngOnInit();

    const [nameInput, startInput, externalInput, simInput, imeiInput, descriptionInput] = await loader.getAllHarnesses(
      MatInputHarness.with({
        ancestor: 'form#tracker-form'
      })
    );

    const updatedName = 'Galileo Sky';

    await nameInput.setValue(updatedName);

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#tracker-form'
      })
    );

    await typeSelect.clickOptions({
      text: testTrackerTypeEnum[0].value
    });

    const updatedStart = '28.02.2023 12:00';

    await startInput.setValue(updatedStart);

    const updatedExternal = '11';

    await externalInput.setValue(updatedExternal);

    const updatedSIM = '+78462777728';

    await simInput.setValue(updatedSIM);

    const updatedIMEI = '501248140768770';

    await imeiInput.setValue(updatedIMEI);

    const updatedDescription = 'Патруль';

    await descriptionInput.setValue(updatedDescription);

    spyOn(trackerService, 'updateTracker')
      .and.callFake(() => of(null));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="tracker-form"]'
      })
    );

    await saveButton.click();

    const [day, month, year, hours, minutes] = updatedStart
      .split(/[.\s:]/)
      .map(Number);

    const monthIndex = month - 1;

    const startDate = new Date(year, monthIndex, day, hours, minutes)
      .toISOString();

    expect(trackerService.updateTracker)
      .toHaveBeenCalledWith({
        ...testNewTracker,
        externalId: Number(updatedExternal),
        name: updatedName,
        simNumber: updatedSIM,
        imei: updatedIMEI,
        trackerType: testTrackerTypeEnum[0].key,
        startDate,
        description: updatedDescription
      });

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(TRACKER_UPDATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(true);
  });
});
