import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed, discardPeriodicTasks, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DATE_PIPE_DEFAULT_OPTIONS, formatDate, formatNumber, registerLocaleData } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import localeRu from '@angular/common/locales/ru';
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
import { MatChipSetHarness } from '@angular/material/chips/testing';
import { LuxonDateAdapter, MAT_LUXON_DATE_FORMATS } from '@angular/material-luxon-adapter';

import { Observable, of } from 'rxjs';

import { MessageService, MessageStatisticsOptions, MessageTrackOptions, Messages, MessagesOptions } from './message.service';

import MessagesComponent, {
  DataMessageParameter,
  MessageColumn,
  MessageType,
  dataMessageColumns,
  parseTime,
  trackerMessageColumns
} from './messages.component';

import { MapComponent } from '../shared/map/map.component';

import { MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { localeID } from '../tech/shared/relative-time.pipe';
import { PAGE_NUM } from '../directory-tech/shared/pagination';
import { DEBOUNCE_DUE_TIME, SEARCH_MIN_LENGTH } from '../tech/tech.component';
import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from '../tech/tech.service.spec';
import { testMessageLocationAndTrack, testMessageStatistics, testSensorMessages, testTrackerMessages } from './message.service.spec';
import { dateFormat } from '../directory-tech/trackers/trackers.component.spec';

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
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: {
              dateFormat: 'd MMMM y, H:mm'
            }
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
        pageIndex: PAGE_NUM,
        total: 10
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

    const testMessagesOptions: MessagesOptions = {
      vehicleId: testMonitoringVehicles[0].id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString(),
      viewMessageType: MessageType.DataMessage,
      parameterType: DataMessageParameter.TrackerData
    };

    const testStatisticsOptions: MessageStatisticsOptions = {
      ...testMessagesOptions
    };

    const testTrackOptions: MessageTrackOptions = {
      vehicleId: testMonitoringVehicles[0].id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString()
    };

    spyOn(messageService, 'getMessages')
      .and.callFake(() => of(testTrackerMessages));

    spyOn(messageService, 'getTrack')
      .and.callFake(() => of(testMessageLocationAndTrack));

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

    expect(messageService.getMessages)
      .toHaveBeenCalledWith(testMessagesOptions);

    expect(messageService.getTrack)
      .toHaveBeenCalledWith(testTrackOptions);

    expect(messageService.getStatistics)
      .toHaveBeenCalledWith(testStatisticsOptions);

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
    await mockTestMessages(component, loader, messageService);

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
      By.css('dl.legend')
    );

    expect(descriptionListDe)
      .withContext('render no description list element')
      .toBeNull();

    // set message statistics
    await mockTestMessages(component, loader, messageService);

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
      By.css('dl.legend')
    );

    const descriptionDetailsDes = descriptionListDe.queryAll(
      By.css('dd')
    );

    const DESCRIPTION_TEXTS = [
      {
        color: '#699575',
        details: 'Сообщения из «черного ящика» (от 2 до 10 минут)'
      },
      {
        color: '#8FAB93',
        details: 'Сообщения из «черного ящика» (от 10 до 30 минут)'
      },
      {
        color: '#CAD8CE',
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

  it('should render tracker message table', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService);

    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  }));

  it('should render sensor message table', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  }));

  it('should render tracker message table rows', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService);

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
    await mockTestMessages(component, loader, messageService, {
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

  it('should render tracker message table header cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService);

    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(dataMessageColumns.length + trackerMessageColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells.map(cell => cell.getText())
    );

    const columnLabels = dataMessageColumns
      .concat(trackerMessageColumns)
      .map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  }));

  it('should render sensor message table header cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService, {
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
      .toBe(dataMessageColumns.length + testSensorMessages.sensorDataMessages![0].sensors.length);

    const sensorNames = testSensorMessages.sensorDataMessages![0].sensors.map(({ name }) => name);

    const headerCellTexts = await parallel(
      () => headerCells.map(cell => cell.getText())
    );

    const columnLabels = dataMessageColumns
      .map(({ value }) => value)
      .concat(sensorNames);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  }));

  it('should render data message table cells', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService);

    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(dataMessageColumns.length + trackerMessageColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells => parallel(
        () => rowCells
          .slice(0, 6)
          .map(cell => cell.getText())
      )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        num: position,
        serverDateTime: time,
        trackerDateTime: registration,
        speed,
        latitude,
        longitude,
        satNumber: satellites,
        altitude
      } = testTrackerMessages.trackerDataMessages![index];

      let formattedTime: string | undefined;
      let formattedRegistration: string | undefined;
      let formattedSpeed: string | undefined;
      let formattedLatitude: string | undefined;
      let formattedLongitude: string | undefined;
      let location: string | undefined;
      let formattedAltitude: string | undefined;

      if (time) {
        formattedTime = formatDate(time, dateFormat, 'ru-RU');
      }

      if (registration) {
        formattedRegistration = formatDate(registration, dateFormat, 'ru-RU');
      }

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

  it('should render parameters cells chips', fakeAsync(async () => {
    await mockTestMessages(component, loader, messageService);

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

      const parameters = testTrackerMessages.trackerDataMessages![index].parameters;

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
    await mockTestMessages(component, loader, messageService, {
      type: MessageType.DataMessage,
      parameter: DataMessageParameter.SensorData
    }, testSensorMessages);

    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(dataMessageColumns.length + testSensorMessages.sensorDataMessages![0].sensors.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells => parallel(
        () => rowCells.map(cell => cell.getText())
      )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        num: position,
        serverDateTime: time,
        trackerDateTime: registration,
        speed,
        latitude,
        longitude,
        satNumber: satellites,
        altitude,
        sensors
      } = testSensorMessages.sensorDataMessages![index];

      let formattedTime: string | undefined;
      let formattedRegistration: string | undefined;
      let formattedSpeed: string | undefined;
      let formattedLatitude: string | undefined;
      let formattedLongitude: string | undefined;
      let location: string | undefined;
      let formattedAltitude: string | undefined;

      if (time) {
        formattedTime = formatDate(time, dateFormat, 'ru-RU');
      }

      if (registration) {
        formattedRegistration = formatDate(registration, dateFormat, 'ru-RU');
      }

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

      const sensorValues: string[] = [];

      sensors.forEach(({ value, unit }) => {
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
});

/**
 * Mock messages, track, statistics submitting selection form.
 *
 * @param component `MessagesComponent` test component.
 * @param loader `HarnessLoader` instance.
 * @param messageService `MessageService` instance.
 * @param options Message type options.
 * @param testMessages Test `Messages`.
 */
async function mockTestMessages(
  component: MessagesComponent,
  loader: HarnessLoader,
  messageService: MessageService,
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

  let typeText: string | undefined;
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
  }

  await typeSelect.clickOptions({
    text: typeText
  });

  if (parameter) {
    await parametersSelect.clickOptions({
      text: parameterText
    });
  }

  spyOn(messageService, 'getMessages')
    .and.callFake(() => of(testMessages));

  spyOn(messageService, 'getTrack')
    .and.callFake(() => of(testMessageLocationAndTrack));

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
}
