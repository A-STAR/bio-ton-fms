import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCheckboxHarness } from '@angular/material/checkbox/testing';
import { MatListOptionHarness, MatSelectionListHarness } from '@angular/material/list/testing';

import { Observable, of } from 'rxjs';

import { MonitoringVehicle, TechService } from './tech.service';

import TechComponent from './tech.component';
import { MapComponent } from '../shared/map/map.component';

import { testMonitoringVehicles } from './tech.service.spec';

describe('TechComponent', () => {
  let component: TechComponent;
  let fixture: ComponentFixture<TechComponent>;
  let loader: HarnessLoader;

  let vehiclesSpy: jasmine.Spy<() => Observable<MonitoringVehicle[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          TechComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TechComponent);
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

  it('should get vehicles', () => {
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

  it('should render vehicle list', fakeAsync(async () => {
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
      .withContext('render vehicle options title')
      .toEqual(
        testMonitoringVehicles.map(({ name }) => name)
      );
  }));

  it('should render vehicle options monitoring state', async () => {
    const optionStateDes = fixture.debugElement.queryAll(
      By.css('mat-selection-list div bio-tech-monitoring-state')
    );

    expect(optionStateDes.length)
      .withContext('render vehicle options monitoring state')
      .toBe(testMonitoringVehicles.length);
  });

  it('should toggle all checkbox selecting options', fakeAsync(async () => {
    const checkbox = await loader.getHarness(
      MatCheckboxHarness.with({
        ancestor: 'aside'
      })
    );

    const options = await loader.getAllHarnesses(
      MatListOptionHarness.with({
        ancestor: 'aside'
      })
    );

    expect(options.length)
      .withContext('render all options unselected')
      .toBe(testMonitoringVehicles.length);

    await checkbox.check();

    expect(options.length)
      .withContext('render all options selected')
      .toBe(testMonitoringVehicles.length);

    await checkbox.uncheck();

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
});
