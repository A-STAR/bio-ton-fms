import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MAT_DIALOG_DATA, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatTabGroupHarness } from '@angular/material/tabs/testing';
import { MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatSlideToggleHarness } from '@angular/material/slide-toggle/testing';
import { MatAccordionHarness, MatExpansionPanelHarness } from '@angular/material/expansion/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of, throwError } from 'rxjs';

import { NewSensor, Sensor, SensorGroup, SensorService, Unit } from '../sensor.service';

import { NumberOnlyInputDirective } from '../../shared/number-only-input/number-only-input.directive';
import { SENSOR_CREATED, SENSOR_UPDATED, SensorDialogComponent, SensorDialogData } from './sensor-dialog.component';

import { TEST_TRACKER_ID } from '../tracker.service.spec';
import {
  testNewSensor,
  testSensor,
  testSensorDataTypeEnum,
  testSensorGroups,
  testSensors,
  testUnits,
  testValidationTypeEnum
} from '../sensor.service.spec';
import { environment } from '../../../environments/environment';
import { ErrorHandler } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

describe('SensorDialogComponent', () => {
  let component: SensorDialogComponent;
  let fixture: ComponentFixture<SensorDialogComponent>;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let sensorService: SensorService;

  const dialogRef = jasmine.createSpyObj<MatDialogRef<SensorDialogComponent, Sensor | '' | undefined>>('MatDialogRef', ['close']);

  let sensorGroupsSpy: jasmine.Spy<(this: SensorService) => Observable<SensorGroup[]>>;
  let unitsSpy: jasmine.Spy<(this: SensorService) => Observable<Unit[]>>;
  let sensorDataTypeSpy: jasmine.Spy<(this: SensorService) => Observable<KeyValue<string, string>[]>>;
  let validationTypeSpy: jasmine.Spy<(this: SensorService) => Observable<KeyValue<string, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatSnackBarModule,
          SensorDialogComponent
        ],
        providers: [
          {
            provide: MAT_DIALOG_DATA,
            useValue: testMatDialogData
          },
          {
            provide: MatDialogRef,
            useValue: dialogRef
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SensorDialogComponent);
    sensorService = TestBed.inject(SensorService);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    const sensorGroups$ = of(testSensorGroups);
    const units$ = of(testUnits);
    const sensorDataType$ = of(testSensorDataTypeEnum);
    const validationType$ = of(testValidationTypeEnum);

    sensorGroupsSpy = spyOnProperty(sensorService, 'sensorGroups$')
      .and.returnValue(sensorGroups$);

    unitsSpy = spyOnProperty(sensorService, 'units$')
      .and.returnValue(units$);

    sensorDataTypeSpy = spyOnProperty(sensorService, 'sensorDataType$')
      .and.returnValue(sensorDataType$);

    validationTypeSpy = spyOnProperty(sensorService, 'validationType$')
      .and.returnValue(validationType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get sensor groups, units', () => {
    expect(sensorGroupsSpy)
      .toHaveBeenCalled();

    expect(unitsSpy)
      .toHaveBeenCalled();
  });

  it('should get sensor data type, validation enums', () => {
    expect(sensorDataTypeSpy)
      .toHaveBeenCalled();

    expect(validationTypeSpy)
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
      .toBe('Новый датчик');

    component['data'] = testUpdateMatDialogData;

    component.ngOnInit();

    fixture.detectChanges();
    await fixture.whenStable();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog update title text')
      .toBe('Сводная информация о датчике');

    component['data'] = testDuplicateMatDialogData;

    component.ngOnInit();

    fixture.detectChanges();
    await fixture.whenStable();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog duplicate title text')
      .toBe('Новый датчик');
  });

  it('should render tabs', async () => {
    const tabGroup = await loader.getHarnessOrNull(
      MatTabGroupHarness.with({
        ancestor: 'form#sensor-form',
        selectedTabLabel: 'Основные сведения'
      })
    );

    expect(tabGroup)
      .withContext('render tab group with basic tab selected')
      .not.toBeNull();

    const tabs = await tabGroup!.getTabs();

    expect(tabs.length)
      .withContext('should render tabs')
      .toBe(1);
  });

  it('should render sensor form', async () => {
    const dialogContentDe = fixture.debugElement.query(
      By.directive(MatDialogContent)
    );

    expect(dialogContentDe)
      .withContext('render dialog content element')
      .not.toBeNull();

    const sensorFormDe = dialogContentDe.query(
      By.css('form#sensor-form')
    );

    expect(sensorFormDe)
      .withContext('render sensor form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Наименование датчика'
      })
    );

    const type = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип датчика"]'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип данных"]'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Формула'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Единица измерения"]'
      })
    );

    const validator = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Валидатор"]'
      })
    );

    const validatorType = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип валидации"]'
      })
    );

    await expectAsync(
      validatorType.isDisabled()
    )
      .withContext('render validation type control disabled')
      .toBeResolvedTo(true);

    await validator.clickOptions({
      text: testSensors.sensors[1].name
    });

    await expectAsync(
      validatorType.isDisabled()
    )
      .withContext('render validation type control enabled')
      .toBeResolvedTo(false);

    loader.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Последнее сообщение',
        checked: false
      })
    );

    loader.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Видимость',
        checked: false
      })
    );

    const fuelUseInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Расход, л/ч'
      })
    );

    await expectAsync(
      fuelUseInput.isDisabled()
    )
      .withContext('render fuel use control disabled')
      .toBeResolvedTo(true);

    await type.clickOptions({
      text: testSensorGroups[1].sensorTypes![0].name
    });

    await expectAsync(
      fuelUseInput.isDisabled()
    )
      .withContext('render fuel use control enabled')
      .toBeResolvedTo(false);

    await expectAsync(
      fuelUseInput.getType()
    )
      .withContext('render fuel use input type')
      .toBeResolvedTo('number');

    const fuelUseInputDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    expect(fuelUseInputDe.nativeElement.placeholder)
      .withContext('render fuel use input with `NumberOnlyDirective`')
      .toBe('Расход, л/ч');

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Описание'
      })
    );

    const accordion = await loader.getHarnessOrNull(
      MatAccordionHarness.with({
        selector: '[displayMode="flat"]'
      })
    );

    expect(accordion)
      .withContext('render settings accordion')
      .not.toBeNull();

    await expectAsync(
      accordion!.isMulti()
    )
      .withContext('render settings accordion `multi` attribute')
      .toBeResolvedTo(true);

    const generalSettingsPanel = await loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Общие настройки',
        expanded: false
      })
    );

    generalSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Таймаут начала движения, с'
      })
    );

    generalSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Расчёт расхода топлива по датчику',
        checked: false
      })
    );

    generalSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Замена ошибочных значений',
        checked: false
      })
    );

    generalSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Расчёт расхода топлива по времени',
        checked: false
      })
    );

    const refuelingSettingsPanel = await loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Настройки заправки',
        expanded: false
      })
    );

    refuelingSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Минимальный объём заправки, л'
      })
    );

    refuelingSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Таймаут разделения заправок, с'
      })
    );

    refuelingSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Таймаут полного объема заправки, с'
      })
    );

    refuelingSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Поиск заправок при остановке',
        checked: false
      })
    );

    refuelingSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Расчёт заправок по времени',
        checked: false
      })
    );

    refuelingSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Расчёт заправки по сырым данным',
        checked: false
      })
    );

    const drainSettingsPanel = await loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Настройки слива',
        expanded: false
      })
    );

    drainSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Минимальный объем слива, л'
      })
    );

    drainSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Таймаут слива при остановки, с'
      })
    );

    drainSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Таймаут разделения сливов, с'
      })
    );

    drainSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Поиск сливов в движении',
        checked: false
      })
    );

    drainSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Расчёт сливов по времени',
        checked: false
      })
    );

    drainSettingsPanel.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Расчёт слива по сырым данным',
        checked: false
      })
    );

    /* Coverage for `onControlSelectionChange` control disabled state. */

    await type.clickOptions();
  });

  it('should render update, duplicate sensor form', async () => {
    component['data'] = testUpdateMatDialogData;

    component.ngOnInit();

    fixture.detectChanges();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Наименование датчика',
        value: testNewSensor.name
      })
    );

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип датчика"]'
      })
    );

    await expectAsync(
      typeSelect.getValueText()
    )
      .withContext('render type select text')
      .toBeResolvedTo(testSensorGroups[1].sensorTypes![0].name);

    const dataTypeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип данных"]'
      })
    );

    await expectAsync(
      dataTypeSelect.getValueText()
    )
      .withContext('render data type select text')
      .toBeResolvedTo(testSensorDataTypeEnum[0].value);

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Формула',
        value: testSensor.formula
      })
    );

    const unitSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Единица измерения"]'
      })
    );

    await expectAsync(
      unitSelect.getValueText()
    )
      .withContext('render type select text')
      .toBeResolvedTo(testUnits[1].name);

    const validatorSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Валидатор"]'
      })
    );

    await expectAsync(
      validatorSelect.getValueText()
    )
      .withContext('render validator select text')
      .toBeResolvedTo(testSensors.sensors[1].name);

    const validationTypeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип валидации"]'
      })
    );

    await expectAsync(
      validationTypeSelect.getValueText()
    )
      .withContext('render validation type select text')
      .toBeResolvedTo(testValidationTypeEnum[0].value);

    await expectAsync(
      validationTypeSelect.isDisabled()
    )
      .withContext('render validation type control enabled')
      .toBeResolvedTo(false);

    loader.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Последнее сообщение',
        checked: testSensor.useLastReceived
      })
    );

    loader.getHarness(
      MatSlideToggleHarness.with({
        ancestor: 'form#sensor-form',
        label: 'Видимость',
        checked: testSensor.visibility
      })
    );

    const fuelInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Расход, л/ч',
        value: testNewSensor.fuelUse?.toString()
      })
    );

    await expectAsync(
      fuelInput.isDisabled()
    )
      .withContext('render fuel use control enabled')
      .toBeResolvedTo(false);

    loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Общие настройки',
        expanded: true
      })
    );

    loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Настройки заправки',
        expanded: true
      })
    );

    loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Настройки слива',
        expanded: true
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

  it('should submit invalid sensor form', async () => {
    spyOn(sensorService, 'createSensor')
      .and.callThrough();

    const refuelingSettingsPanel = await loader.getHarness(
      MatExpansionPanelHarness.with({
        title: 'Настройки заправки',
        expanded: false
      })
    );

    const minRefuellingInput = await refuelingSettingsPanel.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Минимальный объём заправки, л'
      })
    );

    await minRefuellingInput.setValue('0.875');

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="sensor-form"]',
        text: 'Сохранить'
      })
    );

    await saveButton.click();

    await expectAsync(
      refuelingSettingsPanel.isExpanded()
    )
      .withContext('render expanded refuelling panel')
      .toBeResolvedTo(true);

    expect(sensorService.createSensor)
      .not.toHaveBeenCalled();
  });

  it('should submit create sensor form', async () => {
    const [nameInput, formulaInput, fuelInput] = await loader.getAllHarnesses(
      MatInputHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    const { name, formula } = testNewSensor;

    await nameInput.setValue(name);

    const [typeSelect, dataTypeSelect, unitSelect, validatorSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    await typeSelect.clickOptions({
      text: testSensorGroups[1].sensorTypes![0].name
    });

    await dataTypeSelect.clickOptions({
      text: testSensorDataTypeEnum[0].value
    });

    await formulaInput.setValue(formula!);

    await unitSelect.clickOptions({
      text: testUnits[1].name
    });

    const newSensor: NewSensor = {
      name: testNewSensor.name,
      trackerId: testNewSensor.trackerId,
      sensorTypeId: testNewSensor.sensorTypeId,
      dataType: testNewSensor.dataType,
      formula: testNewSensor.formula,
      unitId: testNewSensor.unitId,
      validatorId: undefined,
      validationType: undefined,
      useLastReceived: false,
      visibility: false,
      fuelUse: undefined,
      description: undefined,
      startTimeout: undefined,
      fixErrors: undefined,
      fuelUseCalculation: undefined,
      fuelUseTimeCalculation: undefined,
      minRefueling: undefined,
      refuelingTimeout: undefined,
      fullRefuelingTimeout: undefined,
      refuelingLookup: undefined,
      refuelingCalculation: undefined,
      refuelingRawCalculation: undefined,
      minDrain: undefined,
      drainTimeout: undefined,
      drainStopTimeout: undefined,
      drainLookup: undefined,
      drainCalculation: undefined,
      drainRawCalculation: undefined
    };

    const testSensorResponse: Sensor = {
      id: newSensor.trackerId,
      tracker: {
        id: newSensor.trackerId,
        value: 'Galileo Sky'
      },
      name: newSensor.name,
      sensorType: {
        id: newSensor.sensorTypeId,
        value: testSensorGroups[1].sensorTypes![0].name
      },
      dataType: newSensor.dataType,
      formula: newSensor.formula,
      unit: {
        id: newSensor.unitId,
        value: testUnits[1].name
      },
      useLastReceived: newSensor.useLastReceived,
      visibility: newSensor.visibility
    };

    const createSensorSpy = spyOn(sensorService, 'createSensor')
      .and.callFake(() => of(testSensorResponse));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="sensor-form"]'
      })
    );

    await saveButton.click();

    expect(sensorService.createSensor)
      .toHaveBeenCalledWith(newSensor);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(SENSOR_CREATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    // TODO: remove local settings
    const testSensor = {
      ...testSensorResponse,
      startTimeout: undefined,
      fixErrors: undefined,
      fuelUseCalculation: undefined,
      fuelUseTimeCalculation: undefined,
      minRefueling: undefined,
      refuelingTimeout: undefined,
      fullRefuelingTimeout: undefined,
      refuelingLookup: undefined,
      refuelingCalculation: undefined,
      refuelingRawCalculation: undefined,
      minDrain: undefined,
      drainTimeout: undefined,
      drainStopTimeout: undefined,
      drainLookup: undefined,
      drainCalculation: undefined,
      drainRawCalculation: undefined
    };

    expect(dialogRef.close)
      .toHaveBeenCalledWith(testSensor);

    /* Test fuel control validation. */

    createSensorSpy.calls.reset();

    await fuelInput.setValue(
      testNewSensor.fuelUse!.toString()
    );

    testSensorResponse.fuelUse = testNewSensor.fuelUse;

    createSensorSpy.and.callFake(() => of(testSensorResponse));

    await saveButton.click();

    newSensor.fuelUse = testNewSensor.fuelUse;

    expect(sensorService.createSensor)
      .toHaveBeenCalledWith(newSensor);

    const url = `${environment.api}/api/telematica/sensor`;

    const nameErrorText = `Датчик с именем ${testSensorResponse.name} уже существует`;
    const formulaErrorText = 'Выражение содержит ссылку на несуществующий параметр трекера или датчик';
    const validatorErrorText = 'Валидатор может ссылаться только на датчики своего трекера';

    let testErrorResponse = new HttpErrorResponse({
      error: {
        errors: {
          Name: [nameErrorText],
          Formula: [formulaErrorText],
          ValidatorId: [validatorErrorText]
        }
      },
      status: 400,
      statusText: 'Bad Request',
      url
    });

    const errorHandler = TestBed.inject(ErrorHandler);

    spyOn(console, 'error');
    const handleErrorSpy = spyOn(errorHandler, 'handleError');

    createSensorSpy.and.callFake(() => throwError(() => testErrorResponse));

    // test for the sensor request `400 Bad Request` error response
    await saveButton.click();

    const [nameFormField, , , formulaFormField] = await loader.getAllHarnesses(
      MatFormFieldHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    const [nameError] = await nameFormField.getErrors();

    await expectAsync(
      nameError.getText()
    )
      .withContext('render name field error text')
      .toBeResolvedTo(nameErrorText);

    const [formulaError] = await formulaFormField.getErrors();

    await expectAsync(
      formulaError.getText()
    )
      .withContext('render formula field error text')
      .toBeResolvedTo(formulaErrorText);

    handleErrorSpy.calls.reset();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    await formulaInput.setValue(formula!);

    testErrorResponse = new HttpErrorResponse({
      error: {
        messages: [
          `Имя датчика ${testSensorResponse.name} зарезервировано`,
          `Датчик уже назначен на GPS-трекер`
        ]
      },
      status: 409,
      statusText: 'Conflict',
      url
    });

    createSensorSpy.and.callFake(() => throwError(() => testErrorResponse));

    // test for the sensor request error response
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
      error: `Имя датчика ${testSensorResponse.name} зарезервировано`,
      status: 409,
      statusText: 'Conflict',
      url
    });

    createSensorSpy.and.callFake(() => throwError(() => testErrorResponse));

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

    createSensorSpy.and.callFake(() => throwError(() => testErrorResponse));

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

  it('should submit update sensor form', async () => {
    component['data'] = testUpdateMatDialogData;

    component.ngOnInit();

    const [nameInput, formulaInput] = await loader.getAllHarnesses(
      MatInputHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    const updatedName = 'Парктроник';

    await nameInput.setValue(updatedName);

    const [typeSelect, dataTypeSelect, unitSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    const updatedGroupIndex = 1;
    const updatedTypeIndex = 1;

    await typeSelect.clickOptions({
      text: testSensorGroups[updatedGroupIndex].sensorTypes![updatedTypeIndex].name
    });

    const updatedDataTypeIndex = 2;

    await dataTypeSelect.clickOptions({
      text: testSensorDataTypeEnum[updatedDataTypeIndex].value
    });

    const updatedFormula = 'park_ext';

    await formulaInput.setValue(updatedFormula);

    const updatedUnitIndex = 2;

    await unitSelect.clickOptions({
      text: testUnits[updatedUnitIndex].name
    });

    spyOn(sensorService, 'updateSensor')
      .and.callFake(() => of(null));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="sensor-form"]'
      })
    );

    await saveButton.click();

    const newSensor: NewSensor = {
      ...testNewSensor,
      name: updatedName,
      sensorTypeId: testSensorGroups[updatedGroupIndex].sensorTypes![updatedTypeIndex].id,
      dataType: testSensorDataTypeEnum[updatedDataTypeIndex].key,
      formula: updatedFormula,
      unitId: testUnits[updatedUnitIndex].id,
      visibility: false,
      fuelUse: undefined
    };

    expect(sensorService.updateSensor)
      .toHaveBeenCalledWith(newSensor);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(SENSOR_UPDATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    const sensor: Sensor = {
      ...testSensor,
      name: updatedName,
      sensorType: {
        id: newSensor.sensorTypeId,
        value: testSensorGroups[updatedGroupIndex].sensorTypes![updatedTypeIndex].name
      },
      dataType: newSensor.dataType,
      formula: updatedFormula,
      unit: {
        id: newSensor.unitId,
        value: testUnits[updatedUnitIndex].name
      },
      visibility: newSensor.visibility,
      fuelUse: newSensor.fuelUse
    };

    expect(dialogRef.close)
      .toHaveBeenCalledWith(sensor);
  });

  it('should submit duplicate sensor form', async () => {
    component['data'] = testDuplicateMatDialogData;

    component.ngOnInit();

    const nameInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    const updatedName = 'Парктроник';

    await nameInput.setValue(updatedName);

    const { id, ...sensor } = testNewSensor;

    const newSensor: NewSensor = {
      ...sensor,
      name: updatedName
    };

    const testSensorResponse: Sensor = {
      ...testSensor,
      name: newSensor.name
    };

    spyOn(sensorService, 'createSensor')
      .and.callFake(() => of(testSensorResponse));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="sensor-form"]'
      })
    );

    await saveButton.click();

    expect(sensorService.createSensor)
      .toHaveBeenCalledWith(newSensor);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(SENSOR_CREATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(testSensorResponse);
  });
});

const testMatDialogData: SensorDialogData = {
  trackerID: TEST_TRACKER_ID,
  sensors: testSensors.sensors
};

const testUpdateMatDialogData: SensorDialogData = {
  sensor: testSensor,
  sensors: testSensors.sensors.filter(sensor => sensor.id !== testSensor.id)
};

const { id, ...sensor } = testSensor;

const testDuplicateMatDialogData: SensorDialogData = {
  sensor,
  sensors: testSensors.sensors
};
