import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MAT_DIALOG_DATA, MatDialogTitle } from '@angular/material/dialog';
import { MatTabGroupHarness } from '@angular/material/tabs/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatSlideToggleHarness } from '@angular/material/slide-toggle/testing';

import { Observable, of } from 'rxjs';

import { SensorGroup, SensorService, SensorType, Unit } from '../sensor.service';

import { NumberOnlyInputDirective } from '../../shared/number-only-input/number-only-input.directive';
import { SensorDialogComponent, SensorDialogData } from './sensor-dialog.component';

import {
  TEST_TRACKER_ID,
  testSensorDataTypeEnum,
  testSensorGroups,
  testSensorTypes,
  testUnits,
  testValidationTypeEnum
} from '../sensor.service.spec';

describe('SensorDialogComponent', () => {
  let component: SensorDialogComponent;
  let fixture: ComponentFixture<SensorDialogComponent>;
  let loader: HarnessLoader;

  let sensorGroupsSpy: jasmine.Spy<(this: SensorService) => Observable<SensorGroup[]>>;
  let sensorTypesSpy: jasmine.Spy<(this: SensorService) => Observable<SensorType[]>>;
  let unitsSpy: jasmine.Spy<(this: SensorService) => Observable<Unit[]>>;
  let sensorDataTypeSpy: jasmine.Spy<(this: SensorService) => Observable<KeyValue<string, string>[]>>;
  let validationTypeSpy: jasmine.Spy<(this: SensorService) => Observable<KeyValue<string, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          SensorDialogComponent
        ],
        providers: [
          {
            provide: MAT_DIALOG_DATA,
            useValue: testMatDialogData
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SensorDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    const sensorService = TestBed.inject(SensorService);

    component = fixture.componentInstance;

    const sensorGroups$ = of(testSensorGroups);
    const sensorTypes$ = of(testSensorTypes);
    const units$ = of(testUnits);
    const sensorDataType$ = of(testSensorDataTypeEnum);
    const validationType$ = of(testValidationTypeEnum);

    sensorGroupsSpy = spyOnProperty(sensorService, 'sensorGroups$')
      .and.returnValue(sensorGroups$);

    sensorTypesSpy = spyOnProperty(sensorService, 'sensorTypes$')
      .and.returnValue(sensorTypes$);

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

  it('should get sensor groups, sensor types, units', () => {
    expect(sensorGroupsSpy)
      .toHaveBeenCalled();

    expect(sensorTypesSpy)
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
    const titleTextDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(titleTextDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleTextDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('Новый датчик');
  });

  it('should render sensor form', async () => {
    const sensorFormDe = fixture.debugElement.query(
      By.css('form#sensor-form')
    );

    expect(sensorFormDe)
      .withContext('render sensor form element')
      .not.toBeNull();
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
    const sensorFormDe = fixture.debugElement.query(
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

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#sensor-form',
        placeholder: 'Описание'
      })
    );

    /* Coverage for `onControlSelectionChange` control disabled state. */

    await type.clickOptions();
  });
});

const testMatDialogData: SensorDialogData = {
  trackerId: TEST_TRACKER_ID
};
