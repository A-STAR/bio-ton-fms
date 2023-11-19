import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed, discardPeriodicTasks, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
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

import { MessageService } from './message.service';

import MessagesComponent from './messages.component';
import { MapComponent } from '../shared/map/map.component';

import { MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { DEBOUNCE_DUE_TIME, SEARCH_MIN_LENGTH } from '../tech/tech.component';
import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from '../tech/tech.service.spec';

describe('MessagesComponent', () => {
  let component: MessagesComponent;
  let fixture: ComponentFixture<MessagesComponent>;
  let loader: HarnessLoader;

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
          },
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(MessagesComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    const messageService = TestBed.inject(MessageService);

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

    await loader.getAllHarnesses(
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

    const date = '17.11.2023';

    // set the same day
    await startDateInput.setValue(date);
    await endDateInput.setValue(date);

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

    const START_TIME = '00:02';
    const END_TIME = '00:03';

    // set the end time earlier than the start time
    await startTimeInput.setValue(END_TIME);
    await startTimeInput.blur();

    await endTimeInput.setValue(START_TIME);
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
      .toBeResolvedTo(`Время должно быть ранее ${START_TIME}`);

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
      .toBeResolvedTo(`Время должно быть позже ${END_TIME}`);

    // set correct start and end time
    await startTimeInput.setValue(START_TIME);
    await startTimeInput.blur();

    await endTimeInput.setValue(END_TIME);
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
    await endTimeInput.setValue(START_TIME);
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
      .toBeResolvedTo(`Время должно быть ранее ${START_TIME}`);

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
      .toBeResolvedTo(`Время должно быть позже ${START_TIME}`);

    discardPeriodicTasks();
  }));

  it('should render map', () => {
    const mapDe = fixture.debugElement.query(
      By.directive(MapComponent)
    );

    expect(mapDe)
      .withContext('render `bio-map` component')
      .not.toBeNull();
  });

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
});
