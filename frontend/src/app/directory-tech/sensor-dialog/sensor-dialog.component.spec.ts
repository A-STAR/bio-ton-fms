import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MAT_DIALOG_DATA, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatTabGroupHarness } from '@angular/material/tabs/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatSlideToggleHarness } from '@angular/material/slide-toggle/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of } from 'rxjs';

import { NewSensor, Sensor, SensorGroup, SensorService, SensorType, Unit } from '../sensor.service';

import { NumberOnlyInputDirective } from '../../shared/number-only-input/number-only-input.directive';
import { SensorDialogComponent, SensorDialogData, SENSOR_CREATED, SENSOR_UPDATED } from './sensor-dialog.component';

import { Tracker } from '../tracker.service';

import { TEST_TRACKER_ID } from '../tracker.service.spec';
import {
  testNewSensor,
  testSensor,
  testSensorDataTypeEnum,
  testSensorGroups,
  testSensorTypes,
  testUnits,
  testValidationTypeEnum
} from '../sensor.service.spec';

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
  let sensorTypesSpy: jasmine.Spy<(this: SensorService) => Observable<SensorType[]>>;
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
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);
    sensorService = TestBed.inject(SensorService);

    component = fixture.componentInstance;

    const sensorGroups$ = of(testSensorGroups);
    const units$ = of(testUnits);
    const sensorDataType$ = of(testSensorDataTypeEnum);
    const sensorTypes$ = of(testSensorTypes);
    const validationType$ = of(testValidationTypeEnum);

    sensorGroupsSpy = spyOnProperty(sensorService, 'sensorGroups$')
      .and.returnValue(sensorGroups$);

    unitsSpy = spyOnProperty(sensorService, 'units$')
      .and.returnValue(units$);

    sensorDataTypeSpy = spyOnProperty(sensorService, 'sensorDataType$')
      .and.returnValue(sensorDataType$);

    sensorTypesSpy = spyOnProperty(sensorService, 'sensorTypes$')
      .and.returnValue(sensorTypes$);

    validationTypeSpy = spyOnProperty(sensorService, 'validationType$')
      .and.returnValue(validationType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get sensor groups, units, types', () => {
    expect(sensorGroupsSpy)
      .toHaveBeenCalled();

    expect(unitsSpy)
      .toHaveBeenCalled();

    expect(sensorTypesSpy)
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

    component['data'] = testSensor;

    component.ngOnInit();

    fixture.detectChanges();
    await fixture.whenStable();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog update title text')
      .toBe('Сводная информация о датчике');

    const { id, ...data } = testSensor;

    component['data'] = data;

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
        selectedTabLabel: 'Основные сведения',
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
      text: testSensorTypes[0].name
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
        placeholder: 'Расход л/ч'
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
      .toBe('Расход л/ч');

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Описание'
      })
    );

    /* Coverage for `onControlSelectionChange` control disabled state. */

    await type.clickOptions();
  });

  it('should render update sensor form', async () => {
    component['data'] = testSensor;

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
      .toBeResolvedTo(testSensorTypes[2].name);

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
      .withContext('render type select text')
      .toBeResolvedTo(testSensorTypes[0].name);

    const validationTypeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#sensor-form',
        selector: '[placeholder="Тип валидации"]'
      })
    );

    await expectAsync(
      validationTypeSelect.getValueText()
    )
      .withContext('render type select text')
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
        placeholder: 'Расход л/ч',
        value: testNewSensor.fuelUse?.toString()
      })
    );

    await expectAsync(
      fuelInput.isDisabled()
    )
      .withContext('render fuel use control enabled')
      .toBeResolvedTo(false);
  });

  it('should submit invalid sensor form', async () => {
    spyOn(sensorService, 'createSensor')
      .and.callThrough();

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="sensor-form"]',
        text: 'Сохранить'
      })
    );

    await saveButton.click();

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

    const [typeSelect, dataTypeSelect, unitSelect] = await loader.getAllHarnesses(
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
      description: undefined
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

    expect(dialogRef.close)
      .toHaveBeenCalledWith(testSensorResponse);

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
  });

  it('should submit update sensor form', async () => {
    component['data'] = testSensor;

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

  it('should duplicate create sensor form', async () => {
    const { id, ...data } = testSensor;

    component['data'] = data;

    component.ngOnInit();

    const nameInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form'
      })
    );

    const updatedName = 'Парктроник';

    await nameInput.setValue(updatedName);

    const newSensor: NewSensor = {
      name: updatedName,
      trackerId: testSensor.tracker.id,
      sensorTypeId: testSensor.sensorType.id,
      dataType: testSensor.dataType,
      formula: testSensor.formula,
      unitId: testSensor.unit.id,
      validatorId: testSensor.validator!.id,
      validationType: testSensor.validationType,
      useLastReceived: false,
      visibility: false,
      fuelUse: testSensor.fuelUse,
      description: testSensor.description
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

const testMatDialogData: SensorDialogData<Tracker['id']> = TEST_TRACKER_ID;
