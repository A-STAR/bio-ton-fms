import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCheckboxHarness } from '@angular/material/checkbox/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatListOptionHarness, MatSelectionListHarness } from '@angular/material/list/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';

import { Observable, of } from 'rxjs';

import { MonitoringVehicle, MonitoringVehiclesOptions, TechService } from './tech.service';

import TechComponent, { SEARCH_DEBOUNCE_TIME, SEARCH_MIN_LENGTH } from './tech.component';
import { MapComponent } from '../shared/map/map.component';
import { TechMonitoringStateComponent } from './shared/tech-monitoring-state/tech-monitoring-state.component';

import { testFindCriterion, testFoundMonitoringVehicles, testMonitoringVehicles } from './tech.service.spec';

describe('TechComponent', () => {
  let component: TechComponent;
  let fixture: ComponentFixture<TechComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;

  let vehiclesSpy: jasmine.Spy<(options?: MonitoringVehiclesOptions) => Observable<MonitoringVehicle[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatDialogModule,
          TechComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TechComponent);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

    const techService = TestBed.inject(TechService);

    component = fixture.componentInstance;

    const vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy = spyOn(techService, 'getVehicles')
      .and.returnValue(vehicles$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get tech', () => {
    expect(vehiclesSpy)
      .toHaveBeenCalled();
  });

  it('should render all checkbox', fakeAsync(async () => {
    const checkbox = await loader.getHarnessOrNull(
      MatCheckboxHarness.with({
        ancestor: 'aside'
      })
    );

    expect(checkbox)
      .withContext('render all checkbox')
      .toBeDefined();
  }));

  it('should render search form', fakeAsync(async () => {
    const searchFormDe = fixture.debugElement.query(
      By.css('form#search-form')
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

  it('should render tech list', fakeAsync(async () => {
    const list = await loader.getHarness(
      MatSelectionListHarness.with({
        ancestor: 'aside'
      })
    );

    const options = await list.getItems();

    const titles = await parallel(() => options.map(
      item => item.getTitle()
    ));

    expect(titles)
      .withContext('render tech options title')
      .toEqual(
        testMonitoringVehicles.map(({ name }) => name)
      );

    const paragraphDe = fixture.debugElement.query(
      By.css('p')
    );

    expect(paragraphDe.attributes)
      .withContext('render empty tech list fallback paragraph `hidden` attribute')
      .toEqual(
        jasmine.objectContaining({
          hidden: ''
        })
      );

    expect(paragraphDe.nativeElement.textContent)
      .withContext('render empty tech list fallback paragraph text')
      .toBe('Техника не найдена');
  }));

  it('should render tech options monitoring state', async () => {
    const optionStateDes = fixture.debugElement.queryAll(
      By.css('mat-selection-list div bio-tech-monitoring-state')
    );

    expect(optionStateDes.length)
      .withContext('render tech options monitoring state')
      .toBe(testMonitoringVehicles.length);
  });

  it('should toggle all checkbox selecting options', fakeAsync(async () => {
    const checkbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: 'aside'
      })
    );

    let options = await loader.getAllHarnesses(
      MatListOptionHarness.with({
        ancestor: 'aside',
        selected: false
      })
    );

    expect(options.length)
      .withContext('render all options unselected')
      .toBe(testMonitoringVehicles.length);

    await checkbox.check();

    options = await loader.getAllHarnesses(
      MatListOptionHarness.with({
        ancestor: 'aside',
        selected: true
      })
    );

    expect(options.length)
      .withContext('render all options selected')
      .toBe(testMonitoringVehicles.length);

    await checkbox.uncheck();

    options = await loader.getAllHarnesses(
      MatListOptionHarness.with({
        ancestor: 'aside',
        selected: false
      })
    );

    expect(options.length)
      .withContext('render all options unselected')
      .toBe(testMonitoringVehicles.length);
  }));

  it('should select options toggling all checkbox', fakeAsync(async () => {
    const checkbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: 'aside'
      })
    );

    const list = await loader.getHarness(
      MatSelectionListHarness.with({
        ancestor: 'aside'
      })
    );

    await expectAsync(
      checkbox.isChecked()
    )
      .withContext('render all checkbox unchecked')
      .toBeResolvedTo(false);

    await list.selectItems({
      title: testMonitoringVehicles[0].name,
    });

    await expectAsync(
      checkbox.isIndeterminate()
    )
      .withContext('render all checkbox indeterminate')
      .toBeResolvedTo(true);

    await list.selectItems();

    await expectAsync(
      checkbox.isChecked()
    )
      .withContext('render all checkbox checked')
      .toBeResolvedTo(true);

    await list.deselectItems({
      title: testMonitoringVehicles[1].name,
    });

    await expectAsync(
      checkbox.isIndeterminate()
    )
      .withContext('render all checkbox indeterminate')
      .toBeResolvedTo(true);

    await list.deselectItems();

    await expectAsync(
      checkbox.isChecked()
    )
      .withContext('render all checkbox unchecked')
      .toBeResolvedTo(false);
  }));

  it('should render map', () => {
    const mapDe = fixture.debugElement.query(
      By.directive(MapComponent)
    );

    expect(mapDe)
      .withContext('render map component')
      .not.toBeNull();
  });

  it('should render GPS tracker command dialog', fakeAsync(async () => {
    const techMonitoringStateDe = fixture.debugElement.query(
      By.directive(TechMonitoringStateComponent)
    );

    techMonitoringStateDe.triggerEventHandler('sendTrackerCommand', testMonitoringVehicles[0].tracker!.id);

    const commandTrackerDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(commandTrackerDialog)
      .withContext('render a GPS tracker command dialog')
      .toBeDefined();

    await commandTrackerDialog!.close();

    overlayContainer = TestBed.inject(OverlayContainer);

    overlayContainer.ngOnDestroy();
  }));

  it('should search tech', fakeAsync(async () => {
    // skip initial call
    vehiclesSpy.calls.reset();

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    // enter empty value
    await searchInput.setValue('');

    tick(SEARCH_DEBOUNCE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();

    // enter insufficient search query
    const testInsufficientSearchQuery = testFindCriterion.substring(0, SEARCH_MIN_LENGTH - 1);

    await searchInput.setValue(testInsufficientSearchQuery);

    tick(SEARCH_DEBOUNCE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();

    // enter satisfying search query
    await searchInput.setValue(testFindCriterion);

    tick(SEARCH_DEBOUNCE_TIME);

    let vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith({
        findCriterion: testFindCriterion
      });

    // clean search field
    await searchInput.setValue('');

    tick(SEARCH_DEBOUNCE_TIME);

    vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith();

    // enter insufficient search query
    vehiclesSpy.calls.reset();

    await searchInput.setValue(testInsufficientSearchQuery);

    tick(SEARCH_DEBOUNCE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();
  }));
});
