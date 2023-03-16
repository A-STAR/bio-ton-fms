import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { formatDate, KeyValue, registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap, Params } from '@angular/router';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHarness } from '@angular/material/card/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';
import { MatSlideToggleHarness } from '@angular/material/slide-toggle/testing';
import { MatDialogRef } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { Observable, of } from 'rxjs';

import { TrackerParameterName, TrackerService, TrackerStandardParameter } from '../tracker.service';
import { NewSensor, Sensors, SensorService } from '../sensor.service';

import TrackerComponent, { SensorColumn, sensorColumns, trackerParameterColumns } from './tracker.component';
import { SensorDialogComponent } from '../sensor-dialog/sensor-dialog.component';

import { DATE_FORMAT } from '../trackers/trackers.component';
import { testStandardParameters } from '../tracker.service.spec';
import { testSensors, TEST_TRACKER_ID, testNewSensor } from '../sensor.service.spec';

describe('TrackerComponent', () => {
  let component: TrackerComponent;
  let fixture: ComponentFixture<TrackerComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;

  let standardParametersSpy: jasmine.Spy<() => Observable<TrackerStandardParameter[]>>;
  let sensorsSpy: jasmine.Spy<() => Observable<Sensors>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatSnackBarModule,
          TrackerComponent
        ],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: 'ru-RU'
          },
          {
            provide: ActivatedRoute,
            useValue: testActivatedRoute
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, 'ru-RU');

    fixture = TestBed.createComponent(TrackerComponent);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);
    overlayContainer = TestBed.inject(OverlayContainer);

    component = fixture.componentInstance;

    const trackerService = TestBed.inject(TrackerService);
    const sensorService = TestBed.inject(SensorService);

    const standardParameters$ = of(testStandardParameters);
    const sensors$ = of(testSensors);

    standardParametersSpy = spyOn(trackerService, 'getStandardParameters')
      .and.returnValue(standardParameters$);

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

  it('should render standard parameters card', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Стандартные параметры'
      })
    );

    expect(card)
      .withContext('render standard parameters card')
      .not.toBeNull();
  });

  it('should get standard parameters', () => {
    expect(standardParametersSpy)
      .toHaveBeenCalled();
  });

  it('should render tracker standard parameter table', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Стандартные параметры'
      })
    );

    const table = await card.getHarnessOrNull(
      MatTableHarness.with({
        ancestor: 'mat-card-content'
      })
    );

    expect(table)
      .withContext('render a tracker standard parameter table')
      .not.toBeNull();
  });

  it('should render tracker standard parameter table rows', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Стандартные параметры'
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
      .toBe(testStandardParameters.length);
  });

  it('should render tracker standard parameter table header cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Стандартные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(trackerParameterColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells.map(cell => cell.getText())
    );

    const columnLabels = trackerParameterColumns.map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  });

  it('should render tracker standard parameter table cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Стандартные параметры'
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
        .toBe(trackerParameterColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells =>
        parallel(
          () => rowCells.map(cell => cell.getText())
        )
    ));

    cellTexts.slice(0, 1).forEach((rowCellTexts, index) => {
      const {
        name,
        paramName: param,
        lastValueDateTime: date,
        lastValueDecimal: decimal
      } = testStandardParameters[index];

      let value: string;

      switch (param) {
        case TrackerParameterName.Time:
          value = formatDate(date!, DATE_FORMAT, 'ru-RU');

          break;
        case TrackerParameterName.Latitude:
        case TrackerParameterName.Longitude:
          value = `${decimal}&deg;`;

          break;

        case TrackerParameterName.Altitude:
          value = `${decimal} m`;

          break;

        case TrackerParameterName.Speed:
          value = `${decimal} km/h`;

          break;

        default:
          value = formatDate(date!, DATE_FORMAT, 'ru-RU') ?? decimal?.toString();
      }

      const standardParameterTexts = [name, param, value];

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(standardParameterTexts);
    });
  });

  it('should render parameters card', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Доступные данные'
      })
    );

    expect(card)
      .withContext('render parameters card')
      .not.toBeNull();
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

  it('should render tracker sensor table', async () => {
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
      .withContext('render a tracker sensor table')
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

  it('should render sensor table action cells', async () => {
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

  it('should render sensor table visibility cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
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

  it('should render create sensor button', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const createSensorButton = await card.getHarnessOrNull(
      MatButtonHarness.with({
        variant: 'flat',
        text: 'Добавить запись'
      })
    );

    expect(createSensorButton)
      .withContext('render a create sensor button')
      .not.toBeNull();
  });

  it('should create tracker sensor', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Дополнительные параметры'
      })
    );

    const createSensorButton = await card.getHarness(
      MatButtonHarness.with({
        variant: 'flat',
        text: 'Добавить запись'
      })
    );

    await createSensorButton.click();

    const sensorDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(sensorDialog)
      .withContext('render a sensor dialog')
      .not.toBeNull();

    await sensorDialog!.close();

    overlayContainer.ngOnDestroy();

    /* Coverage for updating sensors. */

    const dialogRef = {
      afterClosed: () => of(testNewSensor)
    } as MatDialogRef<SensorDialogComponent, NewSensor>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await createSensorButton.click();
  });
});

const testParams: Params = {
  id: TEST_TRACKER_ID
};

const testParamMap = convertToParamMap(testParams);

const testActivatedRoute = {
  params: testParams,
  snapshot: {
    get paramMap() {
      return testParamMap;
    }
  },
  get paramMap() {
    return of(testParamMap);
  }
} as Pick<ActivatedRoute, 'params' | 'paramMap' | 'snapshot'>;
