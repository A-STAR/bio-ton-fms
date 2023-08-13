import { ComponentFixture, TestBed, discardPeriodicTasks, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCheckboxHarness } from '@angular/material/checkbox/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatListOptionHarness, MatSelectionListHarness } from '@angular/material/list/testing';
import { MatAccordionHarness } from '@angular/material/expansion/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';

import { Observable, of } from 'rxjs';

import { MonitoringVehicle, MonitoringVehiclesOptions, TechService } from './tech.service';

import TechComponent, { POLL_INTERVAL_PERIOD, SEARCH_DEBOUNCE_DUE_TIME, SEARCH_MIN_LENGTH } from './tech.component';
import { TechMonitoringStateComponent } from './shared/tech-monitoring-state/tech-monitoring-state.component';
import { MapComponent } from '../shared/map/map.component';

import { mockTestFoundMonitoringVehicles, testFindCriterion, testMonitoringVehicles } from './tech.service.spec';

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

  it('should poll tech', fakeAsync(async () => {
    expect(vehiclesSpy)
      .toHaveBeenCalled();

    /* Workaround to retrigger async pipe. */
    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    // enter search query
    const testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles();
    let vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue(testFindCriterion);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    // clean search field to get back to normal tech requests
    vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue('');

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(3);

    tick(POLL_INTERVAL_PERIOD);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(4);

    tick(POLL_INTERVAL_PERIOD);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(5);

    discardPeriodicTasks();
  }));

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

  it('should converge tech selection', fakeAsync(async () => {
    expect(vehiclesSpy)
      .toHaveBeenCalled();

    const checkbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: 'aside'
      })
    );

    await checkbox.check();

    /* Workaround to retrigger async pipe. */
    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    // enter search query
    const testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles();
    let vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue(testFindCriterion);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    /* Coverage for selected tech become diverged. */
    const testUpdatedMonitoringVehicles = testMonitoringVehicles.slice(0, 2);

    vehicles$ = of(testUpdatedMonitoringVehicles);

    vehiclesSpy.and.returnValue(vehicles$);

    // clean search field to get back to normal tech requests
    await searchInput.setValue('');

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(3);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith();

    const options = await loader.getAllHarnesses(
      MatListOptionHarness.with({
        ancestor: 'aside',
        selected: true
      })
    );

    expect(options.length)
      .withContext('render all converged options selected')
      .toBe(testUpdatedMonitoringVehicles.length);

    discardPeriodicTasks();
  }));

  it('should render tech list', fakeAsync(async () => {
    const list = await loader.getHarness(
      MatSelectionListHarness.with({
        ancestor: 'aside'
      })
    );

    const options = await list.getItems();

    expect(options.length)
      .withContext('render tech options')
      .toBe(testMonitoringVehicles.length);

    const titles = await parallel(() => options.map(
      option => option.getTitle()
    ));

    expect(titles)
      .withContext('render tech options title')
      .toEqual(
        testMonitoringVehicles.map(({ name }) => name)
      );

    const checkboxPositions = await parallel(() => options.map(
      item => item.getCheckboxPosition()
    ));

    checkboxPositions.forEach(position => {
      expect(position)
        .withContext('render tech options checkbox position')
        .toBe('before');
    });

    const panelButtonDes = fixture.debugElement.queryAll(
      By.css('aside mat-selection-list mat-list-option button[bioStopClickPropagation]')
    );

    expect(panelButtonDes.length)
      .withContext('render panel buttons')
      .toBe(testMonitoringVehicles.length);

    const paragraphDe = fixture.debugElement.query(
      By.css('p')
    );

    expect(paragraphDe.attributes)
      .withContext('render empty tech list paragraph `hidden` attribute')
      .toEqual(
        jasmine.objectContaining({
          hidden: ''
        })
      );
  }));

  it('should render tech options monitoring state', async () => {
    const optionStateDes = fixture.debugElement.queryAll(
      By.css('mat-selection-list div bio-tech-monitoring-state')
    );

    expect(optionStateDes.length)
      .withContext('render tech options monitoring state')
      .toBe(testMonitoringVehicles.length);
  });

  it('should render tech panels', fakeAsync(async () => {
    const accordion = await loader.getHarness(
      MatAccordionHarness.with({
        ancestor: 'aside mat-selection-list',
        selector: '[displayMode="flat"]'
      })
    );

    const panels = await accordion.getExpansionPanels();

    expect(panels.length)
      .withContext('render tech panels')
      .toBe(testMonitoringVehicles.length);
  }));

  it('should render map', () => {
    const mapDe = fixture.debugElement.query(
      By.directive(MapComponent)
    );

    expect(mapDe)
      .withContext('render map component')
      .not.toBeNull();
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

  it('should toggle tech panels', fakeAsync(async () => {
    const accordion = await loader.getHarness(
      MatAccordionHarness.with({
        ancestor: 'aside mat-selection-list',
        selector: '[displayMode="flat"]'
      })
    );

    let panels = await accordion.getExpansionPanels({
      expanded: false
    });

    expect(panels.length)
      .withContext('render collapsed tech panels')
      .toBe(testMonitoringVehicles.length);

    const panelButtonDes = fixture.debugElement.queryAll(
      By.css('aside mat-selection-list mat-list-option button')
    );

    const testTogglePanel = async (index: number) => {
      const {
        [index]: panel
      } = await accordion.getExpansionPanels();

      let isExpanded = await panel.isExpanded();

      const event = new MouseEvent('click');

      panelButtonDes[index].triggerEventHandler('click', event);

      panels = await accordion.getExpansionPanels({
        expanded: false
      });

      if (isExpanded) {
        isExpanded = await panel.isExpanded();

        expect(isExpanded)
          .withContext(`render collapsed ${testMonitoringVehicles[index].name} panel`)
          .toBe(false);

        expect(panels.length)
          .withContext('render collapsed tech panels')
          .toBe(testMonitoringVehicles.length);
      } else {
        isExpanded = await panel.isExpanded();

        expect(isExpanded)
          .withContext(`render expanded ${testMonitoringVehicles[index].name} panel`)
          .toBe(true);

        expect(panels.length)
          .withContext('render collapsed tech panels')
          .toBe(testMonitoringVehicles.length - 1);
      }
    };

    await testTogglePanel(0);
    await testTogglePanel(0);
    await testTogglePanel(0);
    await testTogglePanel(1);
    await testTogglePanel(2);
    await testTogglePanel(2);
    await testTogglePanel(0);
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

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();

    const spaceChar = ' ';

    // enter insufficient search query
    const testInsufficientSearchQuery = `${spaceChar}${testFindCriterion.substring(0, SEARCH_MIN_LENGTH - 1)}${spaceChar.repeat(20)}`;

    await searchInput.setValue(testInsufficientSearchQuery);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalledTimes(2);

    // enter satisfying search query
    let testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles();
    let vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue(`${testFindCriterion}${spaceChar.repeat(2)}`);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith({
        findCriterion: testFindCriterion.toLocaleLowerCase()
      });

    // clean search field
    vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue('');

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(2);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith();

    // enter insufficient search query
    vehiclesSpy.calls.reset();

    await searchInput.setValue(testInsufficientSearchQuery);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .not.toHaveBeenCalled();

    // enter invalid search query
    const testInvalidIDSearchQuery = '123';

    testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles(testInvalidIDSearchQuery);
    vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue(testInvalidIDSearchQuery);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith({
        findCriterion: testInvalidIDSearchQuery.toLocaleLowerCase()
      });

    const paragraphDe = fixture.debugElement.query(
      By.css('p')
    );

    expect(paragraphDe.attributes)
      .withContext('render empty tech list paragraph without `hidden` attribute')
      .toEqual(
        jasmine.objectContaining({})
      );

    expect(paragraphDe.nativeElement.textContent)
      .withContext('render empty tech list paragraph text')
      .toBe('Техника не найдена');

    discardPeriodicTasks();
  }));

  it('should poll tech search', fakeAsync(async () => {
    expect(vehiclesSpy)
      .toHaveBeenCalled();

    const searchInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#search-form',
        placeholder: 'Поиск'
      })
    );

    // enter search query
    const testFoundMonitoringVehicles = mockTestFoundMonitoringVehicles();
    let vehicles$ = of(testFoundMonitoringVehicles);

    vehiclesSpy = vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue(testFindCriterion);

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(2);

    const options = {
      findCriterion: testFindCriterion.toLocaleLowerCase()
    };

    expect(vehiclesSpy)
      .toHaveBeenCalledWith(options);

    tick(POLL_INTERVAL_PERIOD);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(3);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith(options);

    tick(POLL_INTERVAL_PERIOD);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(4);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith(options);

    // clean search field
    vehicles$ = of(testMonitoringVehicles);

    vehiclesSpy.and.returnValue(vehicles$);

    await searchInput.setValue('');

    tick(SEARCH_DEBOUNCE_DUE_TIME);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(5);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith();

    tick(POLL_INTERVAL_PERIOD);

    expect(vehiclesSpy)
      .toHaveBeenCalledTimes(6);

    expect(vehiclesSpy)
      .toHaveBeenCalledWith();

    discardPeriodicTasks();
  }));
});
