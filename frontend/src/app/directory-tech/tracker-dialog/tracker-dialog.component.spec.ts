import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatDialogTitle } from '@angular/material/dialog';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';

import { Observable, of } from 'rxjs';

import { TrackerService, TrackerTypeEnum } from '../tracker.service';

import { NumberOnlyInputDirective } from 'src/app/shared/number-only-input/number-only-input.directive';
import { TrackerDialogComponent } from './tracker-dialog.component';

import { testTrackerTypeEnum } from '../tracker.service.spec';

describe('TrackerDialogComponent', () => {
  let component: TrackerDialogComponent;
  let fixture: ComponentFixture<TrackerDialogComponent>;
  let loader: HarnessLoader;
  let trackerService: TrackerService;

  let trackerTypeSpy: jasmine.Spy<(this: TrackerService) => Observable<KeyValue<TrackerTypeEnum, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          TrackerDialogComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);
    trackerService = TestBed.inject(TrackerService);

    component = fixture.componentInstance;

    const trackerType$ = of(testTrackerTypeEnum);

    trackerTypeSpy = spyOnProperty(trackerService, 'trackerType$')
      .and.returnValue(trackerType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get tracker type enum', () => {
    expect(trackerTypeSpy)
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
      .toBe('Добавление GPS-трекера');
  });

  it('should render tracker form', async () => {
    const trackerFormDe = fixture.debugElement.query(
      By.css('form#tracker-form')
    );

    expect(trackerFormDe)
      .withContext('render Tracker form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Наименование GPS-трекера'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#tracker-form',
        selector: '[placeholder="Тип устройства"]'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Время начала'
      })
    );

    const externalInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Внешний ID'
      })
    );

    await expectAsync(
      externalInput.getType()
    )
      .withContext('render external ID input type')
      .toBeResolvedTo('number');

    const externalInputDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    expect(externalInputDe.nativeElement.placeholder)
      .withContext('render external ID input with `NumberOnlyDirective`')
      .toBe('Внешний ID');

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        selector: '[type="tel"]',
        placeholder: 'Номер SIM-карты'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'IMEI номер'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#tracker-form',
        placeholder: 'Описание'
      })
    );
  });
});
