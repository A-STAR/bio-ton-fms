import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed, discardPeriodicTasks, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { registerLocaleData } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import localeRu from '@angular/common/locales/ru';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatAutocompleteHarness } from '@angular/material/autocomplete/testing';
import { MatDatepickerInputHarness, MatDatepickerToggleHarness } from '@angular/material/datepicker/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { LuxonDateAdapter, MAT_LUXON_DATE_FORMATS } from '@angular/material-luxon-adapter';

import { Observable, of } from 'rxjs';

import { MessageService, MessageStatisticsOptions } from './message.service';

import MessagesComponent, { DataMessageParameter, MessageType, parseTime } from './messages.component';
import { MapComponent } from '../shared/map/map.component';

import { MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { localeID } from '../tech/shared/relative-time.pipe';
import { DEBOUNCE_DUE_TIME, SEARCH_MIN_LENGTH } from '../tech/tech.component';
import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from '../tech/tech.service.spec';
import { testMessageStatistics } from './message.service.spec';

describe('MessagesComponent', () => {
  let component: MessagesComponent;
  let fixture: ComponentFixture<MessagesComponent>;
  let loader: HarnessLoader;
  let messageService: MessageService;

  let vehiclesSpy: jasmine.Spy<(options?: MonitoringVehiclesOptions) => Observable<MonitoringVehicle[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MessagesComponent
        ],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: 'ru-RU'
          },
          {
            provide: DateAdapter,
            useClass: LuxonDateAdapter
          },
          {
            provide: MAT_DATE_FORMATS,
            useValue: MAT_LUXON_DATE_FORMATS
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(MessagesComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    messageService = TestBed.inject(MessageService);

    component = fixture.componentInstance;

    const vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy = spyOn(messageService, 'getVehicles')
      .and.returnValue(vehicles$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render selection form', fakeAsync(async () => {
    const selectionFormDe = fixture.debugElement.query(
      By.css('form#selection-form')
    );

    expect(selectionFormDe)
      .withContext('render selection form element')
      .not.toBeNull();

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#selection-form',
        placeholder: 'Поиск'
      })
    );

    await loader.getHarness(
      MatAutocompleteHarness.with({
        ancestor: 'form#selection-form'
      })
    );

    await loader.getHarness(
      MatDatepickerInputHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="start"]',
        selector: '#start',
        placeholder: 'Дата начала'
      })
    );

    await loader.getHarness(
      MatDatepickerToggleHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="start"]'
      })
    );

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="start"]',
        placeholder: 'Время начала'
      })
    );

    await loader.getAllHarnesses(
      MatDatepickerInputHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="end"]',
        selector: '#end',
        placeholder: 'Дата конца'
      })
    );

    await loader.getHarness(
      MatDatepickerToggleHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="end"]'
      })
    );

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="end"]',
        placeholder: 'Время конца'
      })
    );

    await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form',
        selector: '[placeholder="Тип сообщений"]'
      })
    );

    await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form',
        selector: '[placeholder="Параметры"]'
      })
    );

    await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="reset"]',
        text: 'Очистить',
        variant: 'stroked'
      })
    );

    await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="submit"]',
        text: 'Выполнить',
        variant: 'flat'
      })
    );
  }));

  it('should get vehicles', () => {
    expect(vehiclesSpy)
      .toHaveBeenCalled();
  });

  it('should render map', () => {
    const mapDe = fixture.debugElement.query(
      By.directive(MapComponent)
    );

    expect(mapDe)
      .withContext('render `bio-map` component')
      .not.toBeNull();
  });

  it('should validate required tech selection', fakeAsync(async () => {
    const techInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#selection-form',
        placeholder: 'Поиск'
      })
    );

    await techInput.setValue(testMonitoringVehicles[0].name);
    await techInput.blur();

    const techFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form',
        isValid: false,
        hasErrors: true
      })
    );

    const errors = await techFormField.getErrors();

    expect(errors.length)
      .withContext('render a single tech form field error')
      .toBe(1);

    await expectAsync(
      errors[0].getText()
    )
      .withContext('render selection required error')
      .toBeResolvedTo('Объект должен быть выбран из списка');

    discardPeriodicTasks();
  }));

  it('should validate range time', fakeAsync(async () => {
    const startDateInput = await loader.getHarness(
      MatDatepickerInputHarness.with({
        placeholder: 'Дата начала'
      })
    );

    const endDateInput = await loader.getHarness(
      MatDatepickerInputHarness.with({
        placeholder: 'Дата конца'
      })
    );

    const testDate = '17.11.2023';

    // set the same day
    await startDateInput.setValue(testDate);
    await endDateInput.setValue(testDate);

    const startTimeInput = await loader.getHarness(
      MatInputHarness.with({
        placeholder: 'Время начала'
      })
    );

    const endTimeInput = await loader.getHarness(
      MatInputHarness.with({
        placeholder: 'Время конца'
      })
    );

    const testStartTime = '00:02';
    const testEndTime = '00:03';

    // set the end time earlier than the start time
    await startTimeInput.setValue(testEndTime);
    await startTimeInput.blur();

    await endTimeInput.setValue(testStartTime);
    await endTimeInput.blur();

    let startTimeFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="start"]',
        isValid: false,
        hasErrors: true
      })
    );

    let errors = await startTimeFormField.getErrors();

    expect(errors.length)
      .withContext('render a single start time form field error')
      .toBe(1);

    await expectAsync(
      errors[0].getText()
    )
      .withContext('render range time max error')
      .toBeResolvedTo(`Время должно быть ранее ${testStartTime}`);

    let endTimeFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="end"]',
        isValid: false,
        hasErrors: true
      })
    );

    errors = await endTimeFormField.getErrors();

    expect(errors.length)
      .withContext('render a single end time form field error')
      .toBe(1);

    await expectAsync(
      errors[0].getText()
    )
      .withContext('render range time min error')
      .toBeResolvedTo(`Время должно быть позже ${testEndTime}`);

    // set correct start and end time
    await startTimeInput.setValue(testStartTime);
    await startTimeInput.blur();

    await endTimeInput.setValue(testEndTime);
    await endTimeInput.blur();

    startTimeFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="start"]',
        isValid: true,
        hasErrors: false
      })
    );

    endTimeFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="end"]',
        isValid: true,
        hasErrors: false
      })
    );

    // set the end time the same as the start time
    await endTimeInput.setValue(testStartTime);
    await endTimeInput.blur();

    startTimeFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="start"]',
        isValid: false,
        hasErrors: true
      })
    );

    errors = await startTimeFormField.getErrors();

    expect(errors.length)
      .withContext('render a single start time form field error')
      .toBe(1);

    await expectAsync(
      errors[0].getText()
    )
      .withContext('render range time max error')
      .toBeResolvedTo(`Время должно быть ранее ${testStartTime}`);

    endTimeFormField = await loader.getHarness(
      MatFormFieldHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"] [formGroupName="end"]',
        isValid: false,
        hasErrors: true
      })
    );

    errors = await endTimeFormField.getErrors();

    expect(errors.length)
      .withContext('render a single end time form field error')
      .toBe(1);

    await expectAsync(
      errors[0].getText()
    )
      .withContext('render range time min error')
      .toBeResolvedTo(`Время должно быть позже ${testStartTime}`);

    discardPeriodicTasks();
  }));

  it('should toggle parameters control visible/disabled state', fakeAsync(async () => {
    // initially render `parameters` control hidden and disabled
    let parametersSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form [formGroupName="message"] [hidden]',
        selector: '[placeholder="Параметры"]'
      })
    );

    await expectAsync(
      parametersSelect.isDisabled()
    )
      .withContext('render parameters control disabled')
      .toBeResolvedTo(true);

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form',
        selector: '[placeholder="Тип сообщений"]'
      })
    );

    // render `parameters` control visible and enabled for `message` data `type`
    typeSelect.clickOptions({
      text: 'Сообщения с данными'
    });

    parametersSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form [formGroupName="message"] :not([hidden])',
        selector: '[placeholder="Параметры"]'
      })
    );

    await expectAsync(
      parametersSelect.isDisabled()
    )
      .withContext('render parameters control enabled')
      .toBeResolvedTo(false);

    // render `parameters` control hidden and enabled for `message` command `type`
    typeSelect.clickOptions({
      text: 'Отправленные команды'
    });

    parametersSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form [formGroupName="message"] [hidden]',
        selector: '[placeholder="Параметры"]'
      })
    );

    await expectAsync(
      parametersSelect.isDisabled()
    )
      .withContext('render parameters control disabled')
      .toBeResolvedTo(true);

    discardPeriodicTasks();
  }));

  it('should search tech', fakeAsync(async () => {
    // skip initial vehicles call
    vehiclesSpy.calls.reset();

    const techInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#selection-form',
        placeholder: 'Поиск'
      })
    );

    // enter empty value
    await techInput.setValue('123');
    await techInput.setValue('');

    tick(DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();

    const spaceChar = ' ';

    // enter insufficient search query
    const testInsufficientSearchQuery = `${spaceChar}${testFindCriterion.substring(0, SEARCH_MIN_LENGTH - 1)}${spaceChar.repeat(20)}`;

    await techInput.setValue(testInsufficientSearchQuery);

    tick(DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalledTimes(2);

    // enter satisfying search query
    let testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles();
    let vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy.and.returnValue(vehicles$);

    await techInput.setValue(`${testFindCriterion}${spaceChar.repeat(2)}`);

    tick(DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith({
        findCriterion: testFindCriterion.toLocaleLowerCase()
      });

    // clean search field
    vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy.and.returnValue(vehicles$);

    await techInput.setValue('');

    tick(DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(2);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith();

    // enter insufficient search query
    vehiclesSpy.calls.reset();

    await techInput.setValue(testInsufficientSearchQuery);

    tick(DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();

    // enter invalid search query
    const testInvalidIDSearchQuery = '123';

    testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles(testInvalidIDSearchQuery);
    vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy.and.returnValue(vehicles$);

    await techInput.setValue(testInvalidIDSearchQuery);

    tick(DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith({
        findCriterion: testInvalidIDSearchQuery.toLocaleLowerCase()
      });
  }));

  it('should submit invalid selection form', fakeAsync(async () => {
    spyOn(messageService, 'getStatistics')
      .and.callThrough();

    const executeButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="submit"]',
        text: 'Выполнить',
        variant: 'flat'
      })
    );

    await executeButton.click();

    expect(messageService.getStatistics)
      .not.toHaveBeenCalled();
  }));

  it('should submit selection form', fakeAsync(async () => {
    /* Autocomplete doesn't properly set an actual control `object` value in tests
       rather than its display value. */
    component['selectionForm'].controls.tech.setValue(testMonitoringVehicles[0]);

    const [startDateInput, endDateInput] = await loader.getAllHarnesses(
      MatDatepickerInputHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"]'
      })
    );

    const testDate = '17.11.2023';

    await startDateInput.setValue(testDate);
    await endDateInput.setValue(testDate);

    const startTimeInput = await loader.getHarness(
      MatInputHarness.with({
        placeholder: 'Время начала'
      })
    );

    const endTimeInput = await loader.getHarness(
      MatInputHarness.with({
        placeholder: 'Время конца'
      })
    );

    const testStartTime = '00:00';
    const testEndTime = '23:59';

    await startTimeInput.setValue(testStartTime);
    await endTimeInput.setValue(testEndTime);

    const [typeSelect, parametersSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#selection-form'
      })
    );

    await typeSelect.clickOptions({
      text: 'Сообщения с данными'
    });

    await parametersSelect.clickOptions({
      text: 'Исходные данные'
    });

    const [day, month, year] = testDate
      .split('.')
      .map(Number);

    const monthIndex = month - 1;

    const [startHours, startMinutes] = parseTime(testStartTime);
    const [endHours, endMinutes] = parseTime(testEndTime);

    const startDate = new Date(year, monthIndex, day, startHours, startMinutes);
    const endDate = new Date(year, monthIndex, day, endHours, endMinutes);

    const testStatisticsOptions: MessageStatisticsOptions = {
      vehicleId: testMonitoringVehicles[0].id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString(),
      viewMessageType: MessageType.DataMessage,
      parameterType: DataMessageParameter.TrackerData
    };

    spyOn(messageService, 'getStatistics')
      .and.callFake(() => of(testMessageStatistics));

    const executeButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="submit"]',
        text: 'Выполнить',
        variant: 'flat'
      })
    );

    await executeButton.click();

    expect(messageService.getStatistics)
      .toHaveBeenCalledWith(testStatisticsOptions);
  }));
});
