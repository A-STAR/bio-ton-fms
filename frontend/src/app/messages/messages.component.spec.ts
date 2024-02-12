import { ErrorHandler, LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed, discardPeriodicTasks, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DecimalPipe, KeyValue, formatDate, formatNumber, registerLocaleData } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import localeRu from '@angular/common/locales/ru';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatAutocompleteHarness } from '@angular/material/autocomplete/testing';
import { MatDatepickerInputHarness, MatDatepickerToggleHarness } from '@angular/material/datepicker/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatCheckboxHarness } from '@angular/material/checkbox/testing';
import { MatChipSetHarness } from '@angular/material/chips/testing';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';
import { LuxonDateAdapter, MAT_LUXON_DATE_FORMATS } from '@angular/material-luxon-adapter';

import { Observable, of, throwError } from 'rxjs';

import {
  DataMessage,
  MessageService,
  MessageStatistics,
  MessageStatisticsOptions,
  MessageTrackOptions,
  Messages,
  MessagesOptions
} from './message.service';

import MessagesComponent, {
  DataMessageParameter,
  MESSAGES_DELETED,
  MessageColumn,
  MessageType,
  commandMessageColumns,
  dataMessageColumns,
  parameterColors,
  parseTime,
  trackerMessageColumns
} from './messages.component';

import { MapComponent } from '../shared/map/map.component';
import { TablePaginationComponent } from '../shared/table-pagination/table-pagination.component';
import { ConfirmationDialogComponent } from '../shared/confirmation-dialog/confirmation-dialog.component';

import { LocationAndTrackResponse, MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { environment } from '../../environments/environment';
import { localeID } from '../tech/shared/relative-time.pipe';
import { PAGE_NUM as INITIAL_PAGE, PAGE_SIZE } from '../directory-tech/shared/pagination';
import { DEBOUNCE_DUE_TIME, SEARCH_MIN_LENGTH } from '../tech/tech.component';
import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from '../tech/tech.service.spec';

import {
  testCommandMessages,
  testMessageLocationAndTrack,
  testMessageStatistics,
  testSensorMessages,
  testTrackerMessages
} from './message.service.spec';

describe('MessagesComponent', () => {
  let component: MessagesComponent;
  let fixture: ComponentFixture<MessagesComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let messageService: MessageService;

  let vehiclesSpy: jasmine.Spy<(options?: MonitoringVehiclesOptions) => Observable<MonitoringVehicle[]>>;
  let messagesSpy: jasmine.Spy<(options: MessagesOptions) => Observable<Messages>>;
  let trackSpy: jasmine.Spy<(options: MessageTrackOptions) => Observable<LocationAndTrackResponse>>;
  let statisticsSpy: jasmine.Spy<(options: MessageStatisticsOptions) => Observable<MessageStatistics>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatSnackBarModule,
          MatDialogModule,
          MessagesComponent
        ],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: 'ru-RU'
          },
          DecimalPipe,
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
    overlayContainer = TestBed.inject(OverlayContainer);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

    messageService = TestBed.inject(MessageService);

    component = fixture.componentInstance;

    const vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy = spyOn(messageService, 'getVehicles')
      .and.returnValue(vehicles$);

    messagesSpy = spyOn(messageService, 'getMessages');
    trackSpy = spyOn(messageService, 'getTrack');
    statisticsSpy = spyOn(messageService, 'getStatistics');

    fixture.detectChanges();
  });

  afterEach(() => {
    vehiclesSpy.calls.reset();
    messagesSpy.calls.reset();
    trackSpy.calls.reset();
    statisticsSpy.calls.reset();
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
        selector: '[bioTimeCharsInput]',
        placeholder: 'Время начала',
        value: '00:00'
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
        selector: '[bioTimeCharsInput]',
        placeholder: 'Время конца',
        value: '00:00'
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

  it('should render messages placeholder', () => {
    const messagesDe = fixture.debugElement.query(
      By.css('section#messages')
    );

    expect(messagesDe)
      .withContext('render messages section')
      .not.toBeNull();

    let paragraphDe = messagesDe.query(
      By.css('p')
    );

    expect(paragraphDe)
      .withContext('render messages placeholder paragraph element')
      .not.toBeNull();

    expect(paragraphDe.nativeElement.textContent)
      .withContext('render messages unselected placeholder paragraph text')
      .toBe('Сообщения не выбраны');

    component['messages$'] = of({
      trackerDataMessages: [],
      pagination: {
        pageIndex: INITIAL_PAGE,
        total: 1,
        records: 0
      }
    });

    fixture.detectChanges();

    paragraphDe = messagesDe.query(
      By.css('p')
    );

    expect(paragraphDe)
      .withContext('render messages placeholder paragraph element')
      .not.toBeNull();

    expect(paragraphDe.nativeElement.textContent)
      .withContext('render messages empty placeholder paragraph text')
      .toBe('Сообщения не найдены');
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
      .toBeResolvedTo(`Время позднее ${testStartTime}`);

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
      .toBeResolvedTo(`Время ранее ${testEndTime}`);

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
      .toBeResolvedTo(`Время позднее ${testStartTime}`);

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
      .toBeResolvedTo(`Время ранее ${testStartTime}`);

    discardPeriodicTasks();
  }));

  it('should toggle parameter control visible/disabled state', fakeAsync(async () => {
    // initially render `parameter` control hidden and disabled
    let parameterSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form [formGroupName="message"] [hidden]',
        selector: '[placeholder="Параметры"]'
      })
    );

    await expectAsync(
      parameterSelect.isDisabled()
    )
      .withContext('render parameter control disabled')
      .toBeResolvedTo(true);

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form',
        selector: '[placeholder="Тип сообщений"]'
      })
    );

    // render `parameter` control visible and enabled for `message` data `type`
    typeSelect.clickOptions({
      text: 'Сообщения с данными'
    });

    parameterSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form [formGroupName="message"] :not([hidden])',
        selector: '[placeholder="Параметры"]'
      })
    );

    await expectAsync(
      parameterSelect.isDisabled()
    )
      .withContext('render parameter control enabled')
      .toBeResolvedTo(false);

    // render `parameter` control hidden and enabled for `message` command `type`
    typeSelect.clickOptions({
      text: 'Отправленные команды'
    });

    parameterSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form [formGroupName="message"] [hidden]',
        selector: '[placeholder="Параметры"]'
      })
    );

    await expectAsync(
      parameterSelect.isDisabled()
    )
      .withContext('render parameter control disabled')
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

  it('should reset selection form range time', fakeAsync(async () => {
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

    startTimeInput.setValue('');
    endTimeInput.setValue('23:59');

    const resetButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="reset"]',
        text: 'Очистить',
        variant: 'stroked'
      })
    );

    await resetButton.click();

    tick(1);

    await expectAsync(
      startTimeInput.getValue()
    )
      .withContext('reset start time default value')
      .toBeResolvedTo('00:00');

    await expectAsync(
      endTimeInput.getValue()
    )
      .withContext('reset end time default value')
      .toBeResolvedTo('00:00');
  }));

  it('should submit invalid selection form', fakeAsync(async () => {
    statisticsSpy.and.callThrough();

    const executeButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="submit"]',
        text: 'Выполнить',
        variant: 'flat'
      })
    );

    await executeButton.click();

    expect(statisticsSpy)
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

    const [typeSelect, parameterSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#selection-form'
      })
    );

    await typeSelect.clickOptions({
      text: 'Сообщения с данными'
    });

    await parameterSelect.clickOptions({
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

    const testMessagesOptions: MessagesOptions = {
      vehicleId: testMonitoringVehicles[0].id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString(),
      viewMessageType: MessageType.DataMessage,
      parameterType: DataMessageParameter.TrackerData,
    };

    const testStatisticsOptions: MessageStatisticsOptions = {
      ...testMessagesOptions
    };

    testMessagesOptions.pageNum = INITIAL_PAGE;

    const testTrackOptions: MessageTrackOptions = {
      vehicleId: testMonitoringVehicles[0].id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString()
    };

    trackSpy.and.callFake(() => of(testMessageLocationAndTrack));
    statisticsSpy.and.callFake(() => of(testMessageStatistics));
    messagesSpy.and.callFake(() => of(testTrackerMessages));

    const executeButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="submit"]',
        text: 'Выполнить',
        variant: 'flat'
      })
    );

    await executeButton.click();

    expect(trackSpy)
      .toHaveBeenCalledWith(testTrackOptions);

    expect(statisticsSpy)
      .toHaveBeenCalledWith(testStatisticsOptions);

    expect(messagesSpy)
      .toHaveBeenCalledWith(testMessagesOptions);

    /* Coverage for updating messages data source */

    await executeButton.click();
  }));

  it('should render statistics', fakeAsync(async () => {
    let headingDe = fixture.debugElement.query(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render no heading element')
      .toBeNull();

    let descriptionListDe = fixture.debugElement.query(
      By.css('dl.statistics')
    );

    expect(descriptionListDe)
      .withContext('render no description list element')
      .toBeNull();

    // set data message statistics
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    fixture.detectChanges();

    headingDe = fixture.debugElement.query(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render heading element')
      .not.toBeNull();

    expect(headingDe.nativeElement.textContent)
      .withContext('render heading text')
      .toBe('Статистика');

    descriptionListDe = fixture.debugElement.query(
      By.css('dl.statistics')
    );

    const descriptionTermDes = descriptionListDe.queryAll(
      By.css('dt')
    );

    const descriptionDetailsDes = descriptionListDe.queryAll(
      By.css('dd')
    );

    const { numberOfMessages, totalTime, distance, mileage, averageSpeed, maxSpeed } = testMessageStatistics;

    const DESCRIPTION_TEXTS = [
      {
        term: 'Всего сообщений',
        details: numberOfMessages.toString()
      },
      {
        term: 'Общее время',
        details: `${totalTime} ч`
      },
      {
        term: 'Расстояние',
        details: `${formatNumber(distance!, localeID, '1.0-0')} км`
      },
      {
        term: 'Пробег',
        details: `${formatNumber(mileage, localeID, '1.0-0')} км`
      },
      {
        term: 'Средняя скорость',
        details: `${formatNumber(averageSpeed, 'en-US', '1.1-1')} км/ч`
      },
      {
        term: 'Максимальная скорость',
        details: `${formatNumber(maxSpeed!, 'en-US', '1.1-1')} км/ч`
      }
    ];

    descriptionTermDes.forEach((descriptionTermDe, index) => {
      expect(descriptionTermDe.nativeElement.textContent)
        .withContext('render description term text')
        .toBe(`${DESCRIPTION_TEXTS[index].term}:`);

      expect(
        descriptionDetailsDes[index].nativeElement.textContent.trim()
      )
        .withContext('render description details text')
        .toBe(DESCRIPTION_TEXTS[index].details);
    });
  }));

  it('should render legend', fakeAsync(async () => {
    let [, headingDe] = fixture.debugElement.queryAll(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render no heading element')
      .toBeUndefined();

    let descriptionListDe = fixture.debugElement.query(
      By.css('dl#legend')
    );

    expect(descriptionListDe)
      .withContext('render no description list element')
      .toBeNull();

    // set message statistics
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    fixture.detectChanges();

    [, headingDe] = fixture.debugElement.queryAll(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render heading element')
      .toBeDefined();

    expect(headingDe.nativeElement.textContent)
      .withContext('render heading text')
      .toBe('Легенда');

    descriptionListDe = fixture.debugElement.query(
      By.css('dl#legend')
    );

    const descriptionDetailsDes = descriptionListDe.queryAll(
      By.css('dd')
    );

    const DESCRIPTION_TEXTS = [
      {
        color: '#CAD8CE',
        details: 'Сообщения из «черного ящика» (от 2 до 10 минут)'
      },
      {
        color: '#8FAB93',
        details: 'Сообщения из «черного ящика» (от 10 до 30 минут)'
      },
      {
        color: '#699575',
        details: 'Сообщения из «черного ящика» (позже 30 минут)'
      }
    ];

    descriptionDetailsDes.forEach((descriptionDetailsDe, index) => {
      const styleAttribute = descriptionDetailsDe.nativeElement.getAttribute('style');

      expect(styleAttribute)
        .withContext('render description marker background color variable attribute')
        .toBe(`--marker-background-color: ${DESCRIPTION_TEXTS[index].color};`);

      expect(
        descriptionDetailsDe.nativeElement.textContent.trim()
      )
        .withContext('render description details text')
        .toBe(DESCRIPTION_TEXTS[index].details);
    });
  }));

  it('should render message table search form', fakeAsync(async () => {
    // render tracker messages table search form
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    let searchFormDe = fixture.debugElement.query(
      By.css('#messages header form#search-form')
    );

    expect(searchFormDe)
      .withContext('render search form element')
      .not.toBeNull();

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    // render no sensor message table search form
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    searchFormDe = fixture.debugElement.query(
      By.css('#messages header form#search-form')
    );

    expect(searchFormDe)
      .withContext('render no search form element')
      .toBeNull();

    // render command message table search form
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    searchFormDe = fixture.debugElement.query(
      By.css('#messages header form#search-form')
    );

    expect(searchFormDe)
      .withContext('render search form element')
      .not.toBeNull();

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );
  }));

  it('should render tracker message table', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  }));

  it('should render sensor message table', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  }));

  it('should render command message table', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  }));

  it('should render tracker message table rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    fixture.detectChanges();

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testTrackerMessages.trackerDataMessages!.length);
  }));

  it('should render sensor message table rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    fixture.detectChanges();

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testSensorMessages.sensorDataMessages!.length);
  }));

  it('should render command message table rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    fixture.detectChanges();

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testCommandMessages.commandMessages!.length);
  }));

  it('should render tracker message table header cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(trackerMessageColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells
        .slice(1)
        .map(cell => cell.getText())
    );

    const columnLabels = trackerMessageColumns
      .filter((column): column is KeyValue<MessageColumn, string> => column.value !== undefined)
      .map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  }));

  it('should render sensor message table header cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(
        dataMessageColumns.length + (testSensorMessages.sensorDataMessages?.['0']?.sensors?.length ?? 0)
      );

    const sensorNames = testSensorMessages.sensorDataMessages?.[0]?.sensors?.map(({ name }) => name) ?? [];

    const headerCellTexts = await parallel(
      () => headerCells.map(cell => cell.getText())
    );

    const columnLabels = dataMessageColumns
      .filter((column): column is KeyValue<MessageColumn, string> => column.value !== undefined)
      .map(({ value }) => value)
      .concat(sensorNames);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  }));

  it('should render command message table header cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(commandMessageColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells
        .slice(1)
        .map(cell => cell.getText())
    );

    const columnLabels = commandMessageColumns
      .filter((column): column is KeyValue<MessageColumn, string> => column.value !== undefined)
      .map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  }));

  it('should render tracker message table cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const rowDes = fixture.debugElement.queryAll(
      By.css('#messages mat-row')
    );

    rowDes.forEach((rowDe, index) => {
      const {
        trackerDateTime: time,
        serverDateTime: registration
      } = testTrackerMessages.trackerDataMessages![index];

      const cssClass = getBlackBoxClass(time, registration);

      if (cssClass) {
        expect(rowDe.classes)
          .withContext('has black box CSS class')
          .toEqual(
            jasmine.objectContaining({
              [cssClass]: true
            })
          );
      } else {
        expect(rowDe.classes)
          .withContext('has no black box CSS class')
          .not.toEqual(
            jasmine.objectContaining({
              ['black-box']: true,
              ['black-box-medium']: true,
              ['black-box-long']: true
            })
          );
      }
    });

    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(trackerMessageColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells => parallel(
        () => rowCells
          .slice(1, 7)
          .map(cell => cell.getText())
      )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        num: position,
        trackerDateTime: time,
        serverDateTime: registration,
        speed,
        latitude,
        longitude,
        satNumber: satellites,
        altitude
      } = testTrackerMessages.trackerDataMessages![index];

      let formattedTime: string | undefined;

      if (time) {
        formattedTime = formatDate(time, DATE_FORMAT, 'ru-RU');
      }

      const formattedRegistration = formatDate(registration, DATE_FORMAT, 'ru-RU');

      let formattedSpeed: string | undefined;
      let formattedLatitude: string | undefined;
      let formattedLongitude: string | undefined;
      let location: string | undefined;
      let formattedAltitude: string | undefined;

      if (speed) {
        formattedSpeed = formatNumber(speed, 'en-US', '1.1-1');
      }

      if (latitude) {
        formattedLatitude = formatNumber(latitude, 'en-US', '1.6-6');
      }

      if (longitude) {
        formattedLongitude = formatNumber(longitude, 'en-US', '1.6-6');
      }

      if (formattedLatitude && formattedLongitude && satellites) {
        location = `${formattedLatitude} ${formattedLongitude} (${satellites})`;
      }

      if (altitude) {
        formattedAltitude = formatNumber(altitude, 'en-US', '1.1-1');
      }

      const messageTexts = [position, formattedTime, formattedRegistration, formattedSpeed, location, formattedAltitude].map(
        value => value?.toString() ?? ''
      );

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(messageTexts);
    });
  }));

  it('should render tracker message table parameters cells chips', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells({
        columnName: MessageColumn.Parameters
      })
    ));

    const parametersChipSets = await parallel(() => cells.map(
      ([parametersCell]) => parametersCell.getHarness(MatChipSetHarness))
    );

    parametersChipSets.forEach(parametersChipSet => {
      expect(parametersChipSet)
        .withContext('render chip set')
        .not.toBeNull();
    });

    parametersChipSets.forEach(async (parametersChipSet, index) => {
      const chips = await parametersChipSet.getChips();

      const chipTexts = await parallel(() => chips.map(
        chip => chip.getText()
      ));

      const parameters = testTrackerMessages.trackerDataMessages![index].parameters!;

      chipTexts.forEach((chipText, index) => {
        const {
          paramName: name,
          lastValueDateTime,
          lastValueDecimal,
          lastValueString
        } = parameters[index];

        const value = lastValueString ?? lastValueDecimal ?? lastValueDateTime;

        expect(chipText)
          .withContext('render parameter text')
          .toBe(`${name}=${value}`);
      });

      const chipHosts = await parallel(() => chips.map(
        chip => chip.host()
      ));

      const chipDisableRippleAttributes = await parallel(() => chipHosts.map(
        host => host.getAttribute('ng-reflect-disable-ripple')
      ));

      chipDisableRippleAttributes.forEach(async value => {
        expect(value)
          .withContext('render disable ripple attribute')
          .not.toBeNull();
      });
    });
  }));

  it('should render sensor message table cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    const rowDes = fixture.debugElement.queryAll(
      By.css('#messages mat-row')
    );

    rowDes.forEach((rowDe, index) => {
      const {
        trackerDateTime: time,
        serverDateTime: registration
      } = testSensorMessages.sensorDataMessages![index];

      const cssClass = getBlackBoxClass(time, registration);

      if (cssClass) {
        expect(rowDe.classes)
          .withContext('has black box CSS class')
          .toEqual(
            jasmine.objectContaining({
              [cssClass]: true
            })
          );
      } else {
        expect(rowDe.classes)
          .withContext('has no black box CSS class')
          .not.toEqual(
            jasmine.objectContaining({
              ['black-box']: true,
              ['black-box-medium']: true,
              ['black-box-long']: true
            })
          );
      }
    });

    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(
          dataMessageColumns.length + (testSensorMessages.sensorDataMessages?.['0']?.sensors?.length ?? 0)
        );
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells => parallel(
        () => rowCells.map(cell => cell.getText())
      )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        num: position,
        trackerDateTime: time,
        serverDateTime: registration,
        speed,
        latitude,
        longitude,
        satNumber: satellites,
        altitude,
        sensors
      } = testSensorMessages.sensorDataMessages![index];

      let formattedTime: string | undefined;

      if (time) {
        formattedTime = formatDate(time, DATE_FORMAT, 'ru-RU');
      }

      const formattedRegistration = formatDate(registration, DATE_FORMAT, 'ru-RU');

      let formattedSpeed: string | undefined;
      let formattedLatitude: string | undefined;
      let formattedLongitude: string | undefined;
      let location: string | undefined;
      let formattedAltitude: string | undefined;

      if (speed) {
        formattedSpeed = formatNumber(speed, 'en-US', '1.1-1');
      }

      if (latitude) {
        formattedLatitude = formatNumber(latitude, 'en-US', '1.6-6');
      }

      if (longitude) {
        formattedLongitude = formatNumber(longitude, 'en-US', '1.6-6');
      }

      if (formattedLatitude && formattedLongitude && satellites) {
        location = `${formattedLatitude} ${formattedLongitude} (${satellites})`;
      }

      if (altitude) {
        formattedAltitude = formatNumber(altitude, 'en-US', '1.1-1');
      }

      const decimalPipe = TestBed.inject(DecimalPipe);

      const sensorValues: string[] = [];

      sensors?.forEach(({ value, unit }) => {
        if (value !== undefined && !isNaN(parseFloat(value))) {
          value = decimalPipe.transform(value, '1.1-6', 'en-US')!;
        }

        const sensorValue = value ? `${value} ${unit}` : '';

        sensorValues.push(sensorValue);
      });

      const messageTexts = [
        position,
        formattedTime,
        formattedRegistration,
        formattedSpeed,
        location,
        formattedAltitude,
        ...sensorValues
      ].map(value => value?.toString() ?? '');

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(messageTexts);
    });
  }));

  it('should render command message table cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(commandMessageColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells => parallel(
        () => rowCells
          .slice(1)
          .map(cell => cell.getText())
      )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        num: position,
        commandDateTime: time,
        commandText: command,
        executionTime: execution,
        channel,
        commandResponseText: response
      } = testCommandMessages.commandMessages![index];

      const formattedTime = formatDate(time, DATE_FORMAT, 'ru-RU');

      let formattedExecution: string | undefined;

      if (execution) {
        formattedExecution = formatDate(time, DATE_FORMAT, 'ru-RU');
      }

      const messageTexts = [position, formattedTime, command, formattedExecution, channel, response].map(
        value => value?.toString() ?? ''
      );

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(messageTexts);
    });
  }));

  it('should render table pagination', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const tablePaginationDe = fixture.debugElement.query(
      By.css('#messages bio-table-pagination')
    );

    expect(tablePaginationDe)
      .withContext('render `bio-table-pagination` component')
      .not.toBeNull();

    expect(
      tablePaginationDe.nativeElement.getAttribute('ng-reflect-pagination')
    )
      .withContext('set pagination')
      .toBeDefined();
  }));

  it('should search messages', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const searchInput = await loader.getHarnessOrNull(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    expect(searchInput)
      .withContext('render search form element')
      .not.toBeNull();

    await searchInput!.setValue('123');

    tick(DEBOUNCE_DUE_TIME);

    await searchInput!.setValue('');

    tick(DEBOUNCE_DUE_TIME);
  }));

  it('should filter out tracker, command message table rows', fakeAsync(async () => {
    // filter out tracker messages
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    const table = await loader.getHarness(MatTableHarness);

    await searchInput!.setValue('123');

    tick(DEBOUNCE_DUE_TIME);

    let rows = await table.getRows();

    expect(rows.length)
      .withContext('render tracker message table no data row')
      .toBe(1);

    let [cell] = await rows[0].getCells();

    await expectAsync(
      cell.getText()
    )
      .withContext('render tracker message table no data row text')
      .toBeResolvedTo('Нет данных');

    await searchInput!.setValue('');

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render tracker message table rows')
      .toBe(testTrackerMessages.trackerDataMessages!.length);

    // filter out command messages
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    await searchInput!.setValue('123');

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render command message table no data row')
      .toBe(1);

    [cell] = await rows[0].getCells();

    await expectAsync(
      cell.getText()
    )
      .withContext('render command message table no data row text')
      .toBeResolvedTo('Нет данных');

    await searchInput!.setValue('');

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render command message table rows')
      .toBe(testCommandMessages.commandMessages!.length);
  }));

  it('should filter tracker message table parameter string value rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    const table = await loader.getHarness(MatTableHarness);

    // test parameter string value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![1].paramName}=${
      testTrackerMessages.trackerDataMessages![0].parameters![1].lastValueString
    }`);

    tick(DEBOUNCE_DUE_TIME);

    let rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter string value rows')
      .toBe(2);

    await searchInput.setValue(`alarm_code?<>${
      testTrackerMessages.trackerDataMessages![0].parameters![1].lastValueString
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render other than parameter string value rows')
      .toBe(1);

    // test multiple parameter string value query
    await searchInput.setValue(`alarm*=${testTrackerMessages.trackerDataMessages![0].parameters![1].lastValueString}, ${
      testTrackerMessages.trackerDataMessages![1].parameters![1].paramName
    }=${testTrackerMessages.trackerDataMessages![1].parameters![1].lastValueString}`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render multiple parameter string value rows')
      .toBe(2);

    // test parameter date time value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![5].paramName}=2023-03-16T09:14:36.422?`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter date time value rows')
      .toBe(2);

    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![5].paramName}<>2023-03-16T09:14:36.42*`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render other than parameter date time value rows')
      .toBe(1);

    // test multiple parameter date time value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![5].paramName}=2023-03-16?09:*:36.422Z, ${
      testTrackerMessages.trackerDataMessages![1].parameters![5].paramName
    }=2023-03-16*`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render multiple parameter date time value rows')
      .toBe(2);

    // test parameter decimal value query
    await searchInput.setValue('*humidity?');

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter decimal value rows')
      .toBe(2);

    // test multiple parameter single decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal}, 35.1*`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render multiple parameter decimal value rows')
      .toBe(2);
  }));

  it('should filter tracker message table parameter decimal value rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    const table = await loader.getHarness(MatTableHarness);

    // test parameter lesser decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}<${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    let rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter lesser decimal value rows')
      .toBe(1);

    // test parameter lesser or equal decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}<=${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter lesser or equal decimal value rows')
      .toBe(2);

    // test parameter equal decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}=${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter equal decimal value rows')
      .toBe(2);

    // test parameter unequal decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}<>${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter unequal decimal value rows')
      .toBe(1);

    // test parameter greater or equal decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}>=${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter greater or equal decimal value rows')
      .toBe(2);

    // test parameter greater decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}>${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render parameter greater decimal value rows')
      .toBe(1);

    // test multiple parameter decimal value query
    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![0].paramName}>${
      testTrackerMessages.trackerDataMessages![0].parameters![0].lastValueDecimal
    }, ${testTrackerMessages.trackerDataMessages![0].parameters![2].paramName}<=${
      testTrackerMessages.trackerDataMessages![0].parameters![2].lastValueDecimal
    }, ${testTrackerMessages.trackerDataMessages![0].parameters![3].paramName}=${
      testTrackerMessages.trackerDataMessages![0].parameters![3].lastValueDecimal
    }`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render multiple parameter decimal value rows')
      .toBe(2);
  }));

  it('should filter command message table response rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.CommandMessage
    }, testCommandMessages);

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    const table = await loader.getHarness(MatTableHarness);

    // test a single response query
    await searchInput.setValue(testCommandMessages.commandMessages![0].commandResponseText!);

    tick(DEBOUNCE_DUE_TIME);

    let rows = await table.getRows();

    expect(rows.length)
      .withContext('render response rows')
      .toBe(1);

    // test shared response query
    await searchInput.setValue('respon?e *');

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render multiple response rows')
      .toBe(2);

    // test multiple response query
    await searchInput.setValue(`${testCommandMessages.commandMessages![0].commandResponseText!}, resp*}`);

    tick(DEBOUNCE_DUE_TIME);

    rows = await table.getRows();

    expect(rows.length)
      .withContext('render multiple response rows')
      .toBe(2);
  }));

  it('should render tracker message table parameters highlight', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    const table = await loader.getHarness(MatTableHarness);

    const parameterIndices = [1, 2, 4];

    await searchInput.setValue(`${testTrackerMessages.trackerDataMessages![0].parameters![parameterIndices[0]].paramName}, ${
      testTrackerMessages.trackerDataMessages![0].parameters![parameterIndices[1]].paramName
    }, ${testTrackerMessages.trackerDataMessages![0].parameters![parameterIndices[2]].paramName}`);

    tick(DEBOUNCE_DUE_TIME);

    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells({
        columnName: MessageColumn.Parameters
      })
    ));

    const parametersChipSets = await parallel(() => cells.map(
      ([parametersCell]) => parametersCell.getHarness(MatChipSetHarness))
    );

    parametersChipSets.forEach(async parametersChipSet => {
      const chips = await parametersChipSet.getChips();

      const chipHosts = await parallel(() => chips.map(
        chip => chip.host()
      ));

      const chipBackgroundColorValues = await parallel(() => chipHosts.map(
        host => host.getCssValue('background-color')
      ));

      parameterIndices.forEach(index => {
        expect(chipBackgroundColorValues[index])
          .withContext('render highlight `background-color` value')
          .toContain(parameterColors[index % parameterColors.length]);
      });
    });
  }));

  it('should reset search form on message type change', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    await searchInput.setValue('123');

    tick(DEBOUNCE_DUE_TIME);

    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    await expectAsync(
      searchInput?.getValue()
    )
      .withContext('reset search form input')
      .toBeResolvedTo('');
  }));

  it('should toggle all checkbox selecting messages', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const selectAllCheckbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-header-row',
        checked: false
      })
    );

    await loader.getAllHarnesses(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-row',
        checked: false
      })
    );

    await selectAllCheckbox.check();

    let selectCheckboxes = await loader.getAllHarnesses(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-row',
        selector: '[bioStopClickPropagation]',
        checked: true
      })
    );

    expect(selectCheckboxes.length)
      .withContext('render all messages selected')
      .toBe(testTrackerMessages.trackerDataMessages!.length);

    await selectAllCheckbox.uncheck();

    selectCheckboxes = await loader.getAllHarnesses(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-row',
        selector: '[bioStopClickPropagation]',
        checked: false
      })
    );

    expect(selectCheckboxes.length)
      .withContext('render all messages unselected')
      .toBe(testTrackerMessages.trackerDataMessages!.length);
  }));

  it('should select messages toggling all checkbox', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const selectAllCheckbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-header-row',
        checked: false
      })
    );

    const selectCheckboxes = await loader.getAllHarnesses(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-row',
        selector: '[bioStopClickPropagation]',
        checked: false
      })
    );

    selectCheckboxes[0].check();

    await expectAsync(
      selectAllCheckbox.isIndeterminate()
    )
      .withContext('render all checkbox indeterminate')
      .toBeResolvedTo(true);

    selectCheckboxes.forEach(checkbox => {
      checkbox.check();
    });

    await expectAsync(
      selectAllCheckbox.isChecked()
    )
      .withContext('render all checkbox checked')
      .toBeResolvedTo(true);

    selectCheckboxes[selectCheckboxes.length - 1].uncheck();

    await expectAsync(
      selectAllCheckbox.isIndeterminate()
    )
      .withContext('render all checkbox indeterminate')
      .toBeResolvedTo(true);

    selectCheckboxes.forEach(checkbox => {
      checkbox.uncheck();
    });

    await expectAsync(
      selectAllCheckbox.isChecked()
    )
      .withContext('render all checkbox unchecked')
      .toBeResolvedTo(false);
  }));

  it('should delete messages', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    let deleteButton: MatButtonHarness | undefined;

    try {
      deleteButton = await loader.getHarness(
        MatButtonHarness.with({
          ancestor: '#messages footer',
          selector: ':not([hidden])',
          text: 'delete',
          variant: 'icon'
        })
      );
    } catch { }

    expect(deleteButton)
      .withContext('render no delete button')
      .toBeUndefined();

    const selectAllCheckbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-header-row'
      })
    );

    selectAllCheckbox.check();

    deleteButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: '#messages footer',
        selector: ':not([hidden])',
        text: 'delete',
        variant: 'icon'
      })
    );

    await deleteButton.click();

    const confirmationDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(confirmationDialog)
      .withContext('render a confirmation dialog')
      .not.toBeNull();

    const deleteMessagesSpy = spyOn(messageService, 'deleteMessages')
      .and.callFake(() => of(null));

    /* Coverage for deleting, updating messages. */

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<ConfirmationDialogComponent, true | '' | undefined>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await deleteButton.click();

    const testMessageIDs = testTrackerMessages.trackerDataMessages!.map(({ id }) => id);

    expect(deleteMessagesSpy)
      .toHaveBeenCalledWith(testMessageIDs);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(MESSAGES_DELETED);

    expect(messagesSpy)
      .toHaveBeenCalled();

    confirmationDialog?.close();

    overlayContainer.ngOnDestroy();
  }));

  it('should delete messages with error response', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const selectAllCheckbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: '#messages mat-header-row'
      })
    );

    selectAllCheckbox.check();

    const deleteButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: '#messages footer',
        text: 'delete',
        variant: 'icon'
      })
    );

    await deleteButton.click();

    const confirmationDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(confirmationDialog)
      .withContext('render a confirmation dialog')
      .not.toBeNull();

    /* Handle an error although update messages anyway. */

    const testURL = `${environment.api}/api/telematica/messagesview/delete-messages'`;

    const testErrorResponse = new HttpErrorResponse({
      error: {
        message: `Http failure response for ${testURL}: 500 Internal Server Error`
      },
      status: 500,
      statusText: 'Internal Server Error',
      url: testURL
    });

    const errorHandler = TestBed.inject(ErrorHandler);

    spyOn(console, 'error');
    spyOn(errorHandler, 'handleError');

    const deleteMessagesSpy = spyOn(messageService, 'deleteMessages')
      .and.callFake(() => throwError(() => testErrorResponse));

    /* Coverage for updating messages. */

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<ConfirmationDialogComponent, true | '' | undefined>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await deleteButton.click();

    const testMessageIDs = testTrackerMessages.trackerDataMessages!.map(({ id }) => id);

    expect(deleteMessagesSpy)
      .toHaveBeenCalledWith(testMessageIDs);

    expect(errorHandler.handleError)
      .toHaveBeenCalledWith(testErrorResponse);

    expect(messagesSpy)
      .toHaveBeenCalled();

    confirmationDialog?.close();

    overlayContainer.ngOnDestroy();
  }));

  it('should keep on streaming messages after error', fakeAsync(async () => {
    /* Autocomplete doesn't properly set an actual control `object` value in tests
       rather than its display value. */
    component['selectionForm'].controls.tech.setValue(testMonitoringVehicles[0]);

    const [startDateInput, endDateInput] = await loader.getAllHarnesses(
      MatDatepickerInputHarness.with({
        ancestor: 'form#selection-form [formGroupName="range"]'
      })
    );

    const testStartDate = '17.11.2023';
    const testEndDate = '18.11.2023';

    await startDateInput.setValue(testStartDate);
    await endDateInput.setValue(testEndDate);

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#selection-form'
      })
    );

    await typeSelect.clickOptions({
      text: 'Отправленные команды'
    });

    trackSpy.and.callFake(() => of(testMessageLocationAndTrack));
    statisticsSpy.and.callFake(() => of(testMessageStatistics));

    const [startDay, startMonth, startYear] = testStartDate
      .split('.')
      .map(Number);

    const [endDay, endMonth, endYear] = testStartDate
      .split('.')
      .map(Number);

    const startMonthIndex = startMonth - 1;
    const endMonthIndex = endMonth - 1;

    const startDate = new Date(startYear, startMonthIndex, startDay);
    const endDate = new Date(endYear, endMonthIndex, endDay);

    const testURL = `${environment.api}/api/telematica/messagesview/list?pageNum=${INITIAL_PAGE}&pageSize=${PAGE_SIZE}&vehicleId=${
      testMonitoringVehicles[0].id
    }&periodStart=${startDate.toISOString()}&periodEnd=${endDate.toISOString()}&viewMessageType=${MessageType.CommandMessage}`;

    const testErrorResponse = new HttpErrorResponse({
      error: {
        message: `Http failure response for ${testURL}: 500 Internal Server Error`
      },
      status: 500,
      statusText: 'Internal Server Error',
      url: testURL
    });

    const errorHandler = TestBed.inject(ErrorHandler);

    messagesSpy.and.callFake(() => throwError(() => testErrorResponse));

    spyOn(console, 'error');

    const handleErrorSpy = spyOn(errorHandler, 'handleError');

    const executeButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#selection-form',
        selector: '[type="submit"]',
        text: 'Выполнить',
        variant: 'flat'
      })
    );

    await executeButton.click();

    expect(messagesSpy)
      .toHaveBeenCalled();

    expect(handleErrorSpy)
      .toHaveBeenCalledWith(testErrorResponse);

    handleErrorSpy.calls.reset();

    messagesSpy.and.callFake(() => of(testCommandMessages));
    handleErrorSpy.and.callThrough();

    await executeButton.click();

    expect(messagesSpy)
      .toHaveBeenCalledTimes(2);

    expect(handleErrorSpy)
      .not.toHaveBeenCalled();
  }));

  it('should handle pagination change', fakeAsync(async () => {
    await mockTestMessages(component, loader, messagesSpy, trackSpy, statisticsSpy);

    const tablePaginationDe = fixture.debugElement.query(
      By.directive(TablePaginationComponent)
    );

    expect(tablePaginationDe)
      .withContext('render `bio-table-pagination` component')
      .not.toBeNull();

    /* Coverage for handling pagination. */

    tablePaginationDe.triggerEventHandler('paginationChange', {
      page: 2,
      size: 100
    });
  }));
});

/**
 * Mock messages, track, statistics submitting selection form.
 *
 * @param component `MessagesComponent` test component.
 * @param loader `HarnessLoader` instance.
 * @param messagesSpy Messages spy.
 * @param trackSpy Track spy.
 * @param statisticsSpy Statistics spy.
 * @param options Message type options.
 * @param testMessages Test `Messages`.
 */
async function mockTestMessages(
  component: MessagesComponent,
  loader: HarnessLoader,
  messagesSpy: jasmine.Spy<(options: MessagesOptions) => Observable<Messages>>,
  trackSpy: jasmine.Spy<(options: MessageTrackOptions) => Observable<LocationAndTrackResponse>>,
  statisticsSpy: jasmine.Spy<(options: MessageStatisticsOptions) => Observable<MessageStatistics>>,
  { type, parameter }: {
    type: MessageType;
    parameter?: DataMessageParameter;
  } = {
    type: MessageType.DataMessage,
    parameter: DataMessageParameter.TrackerData
  },
  testMessages: Messages = testTrackerMessages
) {
  /* Autocomplete doesn't properly set an actual control `object` value in tests
       rather than its display value. */
  component['selectionForm'].controls.tech.setValue(testMonitoringVehicles[0]);

  const [startDateInput, endDateInput] = await loader.getAllHarnesses(
    MatDatepickerInputHarness.with({
      ancestor: 'form#selection-form [formGroupName="range"]'
    })
  );

  await startDateInput.setValue('17.11.2023');
  await endDateInput.setValue('18.11.2023');

  const [typeSelect, parametersSelect] = await loader.getAllHarnesses(
    MatSelectHarness.with({
      ancestor: 'form#selection-form'
    })
  );

  let typeText: string;
  let parameterText: string | undefined;

  switch (type) {
    case MessageType.DataMessage:
      typeText = 'Сообщения с данными';

      switch (parameter) {
        case DataMessageParameter.TrackerData:
          parameterText = 'Исходные данные';

          break;

        case DataMessageParameter.SensorData:
          parameterText = 'Значения датчиков';
      }

      break;

    case MessageType.CommandMessage:
      typeText = 'Отправленные команды';
  }

  await typeSelect.clickOptions({
    text: typeText
  });

  if (parameter) {
    await parametersSelect.clickOptions({
      text: parameterText
    });
  }

  trackSpy.and.callFake(() => of(testMessageLocationAndTrack));
  statisticsSpy.and.callFake(() => of(testMessageStatistics));
  messagesSpy.and.callFake(() => of(testMessages));

  const executeButton = await loader.getHarness(
    MatButtonHarness.with({
      ancestor: 'form#selection-form',
      selector: '[type="submit"]',
      text: 'Выполнить',
      variant: 'flat'
    })
  );

  await executeButton.click();
}

/**
 * Compute message row black box CSS class.
 *
 * @param timeDate Message time date.
 * @param registrationDate Message server registration date.
 *
 * @returns Message CSS class.
 */
function getBlackBoxClass(timeDate: DataMessage['trackerDateTime'], registrationDate: DataMessage['serverDateTime']) {
  let cssClass: string | undefined;

  if (timeDate) {
    const time = new Date(timeDate)
      .getTime();

    const registration = new Date(registrationDate)
      .getTime();

    const period = registration - time;
    const MINUTE = 60 * 1000;

    switch (true) {
      case period <= 2 * MINUTE:

        break;

      case period <= 10 * MINUTE:
        cssClass = 'black-box';

        break;

      case period <= 30 * MINUTE:
        cssClass = 'black-box-medium';

        break;

      default:
        cssClass = 'black-box-long';
    }
  }

  return cssClass;
}

const DATE_FORMAT = 'd MMMM y, H:mm:ss';
