import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatAutocompleteHarness } from '@angular/material/autocomplete/testing';

import { Observable, of } from 'rxjs';

import { MessageService } from './message.service';

import MessagesComponent from './messages.component';
import { MapComponent } from '../shared/map/map.component';

import { MonitoringVehicle } from '../tech/tech.service';

import { testMonitoringVehicles } from '../tech/tech.service.spec';

describe('MessagesComponent', () => {
  let component: MessagesComponent;
  let fixture: ComponentFixture<MessagesComponent>;
  let loader: HarnessLoader;

  let vehiclesSpy: jasmine.Spy<() => Observable<MonitoringVehicle[]>>;

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
});
