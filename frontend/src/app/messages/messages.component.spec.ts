import { ComponentFixture, TestBed, discardPeriodicTasks, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatAutocompleteHarness } from '@angular/material/autocomplete/testing';

import { Observable, of } from 'rxjs';

import { MessageService } from './message.service';

import MessagesComponent from './messages.component';
import { MapComponent } from '../shared/map/map.component';

import { MonitoringVehicle, MonitoringVehiclesOptions } from '../tech/tech.service';

import { DEBOUNCE_DUE_TIME, SEARCH_MIN_LENGTH } from '../tech/tech.component';
import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from '../tech/tech.service.spec';
import { MatButtonHarness } from '@angular/material/button/testing';

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
      .withContext('render a single tech error')
      .toBe(1);

    await expectAsync(
      errors[0].getText()
    )
      .withContext('render selection required error')
      .toBeResolvedTo('Объект должен быть выбран из списка');

    discardPeriodicTasks();
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
