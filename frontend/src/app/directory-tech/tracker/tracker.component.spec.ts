import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap, Params } from '@angular/router';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';
import { MatSlideToggleHarness } from '@angular/material/slide-toggle/testing';

import { Observable, of } from 'rxjs';

import { Sensors, SensorService } from '../sensor.service';

import TrackerComponent, { SensorColumn, sensorColumns } from './tracker.component';

import { testSensors, TEST_TRACKER_ID } from '../sensor.service.spec';

describe('TrackerComponent', () => {
  let component: TrackerComponent;
  let fixture: ComponentFixture<TrackerComponent>;
  let loader: HarnessLoader;
  let sensorService: SensorService;

  let sensorsSpy: jasmine.Spy<() => Observable<Sensors>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          TrackerComponent,
          HttpClientTestingModule
        ],
        providers: [
          {
            provide: ActivatedRoute,
            useValue: testActivatedRoute
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);
    sensorService = TestBed.inject(SensorService);

    component = fixture.componentInstance;

    const sensors$ = of(testSensors);

    sensorsSpy = spyOn(sensorService, 'getSensors')
      .and.returnValue(sensors$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render Tracker page heading', () => {
    const headingDe = fixture.debugElement.query(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render heading element')
      .not.toBeNull();

    expect(headingDe.nativeElement.textContent)
      .withContext('render heading text')
      .toBe('Конфигурация данных GPS-трекера');
  });

  it('should render sensors card', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    expect(card)
      .withContext('render sensors card')
      .not.toBeNull();
  });

  it('should get sensors', () => {
    expect(sensorsSpy)
      .toHaveBeenCalled();
  });

  it('should render tracker sensors table', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const table = await card.getHarnessOrNull(
      MatTableHarness.with({
        ancestor: 'mat-card-content'
      })
    );

    expect(table)
      .withContext('render a tracker sensors table')
      .not.toBeNull();
  });

  it('should render tracker sensor table rows', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testSensors.sensors.length);
  });

  it('should render tracker sensor table header cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(sensorColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells
        .slice(1)
        .map(cell => cell.getText())
    );

    const columnLabels = sensorColumns
      .filter((column): column is KeyValue<SensorColumn, string> => column.value !== undefined)
      .map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  });

  it('should render sensors table action cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    const actionButtons = await parallel(() => cells.map(
      ({
        0: actionCell
      }) => parallel(() => [
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            selector: '[bioTableActionsTrigger]',
            variant: 'icon',
            text: 'more_horiz'
          })
        )
      ])
    ));

    actionButtons.forEach(([actionButton]) => {
      expect(actionButton)
        .withContext('render action button')
        .not.toBeNull();

      actionButton!.hasHarness(
        MatIconHarness.with({
          name: 'more_horiz'
        })
      );
    });
  });

  it('should render tracker sensor table cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(sensorColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells =>
        parallel(
          () => rowCells
            .slice(1, -1)
            .map(cell => cell.getText())
        )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        name,
        sensorType: {
          value: type
        },
        formula,
        unit: {
          value: unit
        }
      } = testSensors.sensors[index];

      const sensorTexts = [name, type, unit, formula].map(value => value ?? '');

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(sensorTexts);
    });
  });

  it('should render sensors table visibility cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells({
        columnName: SensorColumn.Visibility
      })
    ));

    const slideToggles = await parallel(() => cells.map(
      ([visibilityCell], index) => visibilityCell.getHarnessOrNull(
        MatSlideToggleHarness.with({
          ancestor: '.mat-column-visibility',
          checked: testSensors.sensors[index].visibility,
          disabled: true
        })
      )
    ));

    slideToggles.forEach(slideToggle => {
      expect(slideToggle)
        .withContext('render toggle')
        .not.toBeNull();
    });
  });
});

const testParams: Params = {
  id: TEST_TRACKER_ID
};

const testParamMap = convertToParamMap(testParams);

const testActivatedRoute = {
  params: testParams,
  get paramMap() {
    return of(testParamMap);
  }
} as Pick<ActivatedRoute, 'params' | 'paramMap'>;
