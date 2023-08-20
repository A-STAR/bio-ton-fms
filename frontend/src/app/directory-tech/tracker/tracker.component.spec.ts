import { ErrorHandler, LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DATE_PIPE_DEFAULT_OPTIONS, formatDate, formatNumber, KeyValue, registerLocaleData } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import localeRu from '@angular/common/locales/ru';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap, Params } from '@angular/router';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatCardHeader } from '@angular/material/card';
import { MatCardHarness } from '@angular/material/card/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';
import { MatSlideToggleHarness } from '@angular/material/slide-toggle/testing';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of, throwError } from 'rxjs';

import { TrackerParameter, TrackerParameterName, TrackerService, TrackerStandardParameter } from '../tracker.service';
import { Sensor, Sensors, SensorService } from '../sensor.service';

import TrackerComponent, { SENSOR_DELETED, SensorColumn, sensorColumns, trackerParameterColumns } from './tracker.component';
import { SensorDialogComponent } from '../sensor-dialog/sensor-dialog.component';

import { environment } from '../../../environments/environment';
import { localeID } from '../../tech/shared/relative-time.pipe';
import { dateFormat } from '../trackers/trackers.component.spec';
import { testParameters, testStandardParameters, TEST_TRACKER_ID } from '../tracker.service.spec';
import { testSensor, testSensors } from '../sensor.service.spec';

describe('TrackerComponent', () => {
  let component: TrackerComponent;
  let fixture: ComponentFixture<TrackerComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let sensorService: SensorService;

  let standardParametersSpy: jasmine.Spy<() => Observable<TrackerStandardParameter[]>>;
  let parametersSpy: jasmine.Spy<() => Observable<TrackerParameter[]>>;
  let sensorsSpy: jasmine.Spy<() => Observable<Sensors>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatSnackBarModule,
          MatDialogModule,
          TrackerComponent
        ],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: localeID
          },
          {
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: { dateFormat }
          },
          {
            provide: ActivatedRoute,
            useValue: testActivatedRoute
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(TrackerComponent);
    overlayContainer = TestBed.inject(OverlayContainer);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    sensorService = TestBed.inject(SensorService);
    const trackerService = TestBed.inject(TrackerService);

    const standardParameters$ = of(testStandardParameters);
    const parameters$ = of(testParameters);
    const sensors$ = of(testSensors);

    standardParametersSpy = spyOn(trackerService, 'getStandardParameters')
      .and.returnValue(standardParameters$);

    parametersSpy = spyOn(trackerService, 'getParameters')
      .and.returnValue(parameters$);

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

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        name,
        paramName: param,
        lastValueDateTime: date,
        lastValueDecimal: decimal
      } = testStandardParameters[index];

      let value: string;

      switch (param) {
        case TrackerParameterName.Time:
          value = formatDate(date!, dateFormat, localeID);

          break;

        case TrackerParameterName.Latitude:
        case TrackerParameterName.Longitude: {
          const formattedDecimal = formatNumber(decimal!, 'en-US', '1.6-6');

          value = `${formattedDecimal}°`;

          break;
        }

        case TrackerParameterName.Altitude: {
          const formattedDecimal = formatNumber(decimal!, 'en-US', '1.1-1');

          value = `${formattedDecimal} м`;

          break;
        }

        case TrackerParameterName.Speed: {
          const formattedDecimal = formatNumber(decimal!, 'en-US', '1.1-1');

          value = `${formattedDecimal} км/ч`;
        }
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
        title: 'Доступные параметры'
      })
    );

    expect(card)
      .withContext('render card')
      .not.toBeNull();

    const cardHeaderDe = fixture.debugElement
      .query(
        By.css('mat-card:nth-of-type(2)')
      )
      .query(
        By.directive(MatCardHeader)
      );

    expect(cardHeaderDe)
      .withContext('render card header')
      .not.toBeNull();

    card.getHarness(
      MatButtonHarness.with({
        ancestor: 'mat-card-header',
        text: 'История значений',
        variant: 'stroked'
      })
    );
  });

  it('should get parameters', () => {
    expect(parametersSpy)
      .toHaveBeenCalled();
  });

  it('should render tracker parameter table', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Доступные параметры'
      })
    );

    const table = await card.getHarnessOrNull(
      MatTableHarness.with({
        ancestor: 'mat-card-content'
      })
    );

    expect(table)
      .withContext('render a tracker parameter table')
      .not.toBeNull();
  });

  it('should render tracker parameter table rows', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Доступные параметры'
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
      .toBe(testParameters.length);
  });

  it('should render tracker parameter table header cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Доступные параметры'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(trackerParameterColumns.length - 1);

    const headerCellTexts = await parallel(
      () => headerCells.map(cell => cell.getText())
    );

    const columnLabels = trackerParameterColumns
      .slice(1)
      .map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  });

  it('should render tracker parameter table cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Доступные параметры'
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
        .toBe(trackerParameterColumns.length - 1);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells =>
        parallel(
          () => rowCells.map(cell => cell.getText())
        )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        paramName: param,
        lastValueDateTime: date,
        lastValueDecimal: decimal,
        lastValueString: string
      } = testParameters[index];

      const value = param === TrackerParameterName.Time
        ? formatDate(date!, dateFormat, localeID)
        : decimal?.toString() ?? string!;

      const parameterTexts = [param, value];

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(parameterTexts);
    });
  });

  it('should render parameters history dialog', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Доступные параметры'
      })
    );

    const parametersHistoryButton = await card.getHarness(
      MatButtonHarness.with({
        text: 'История значений',
        variant: 'stroked'
      })
    );

    await parametersHistoryButton.click();

    const parametersHistoryDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(parametersHistoryDialog)
      .withContext('render a parameters history dialog')
      .not.toBeNull();

    await parametersHistoryDialog!.close();

    overlayContainer.ngOnDestroy();
  });

  it('should render sensors card', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Датчики'
      })
    );

    expect(card)
      .withContext('render card')
      .not.toBeNull();

    const cardHeaderDe = fixture.debugElement
      .query(
        By.css('mat-card:nth-of-type(3)')
      )
      .query(
        By.directive(MatCardHeader)
      );

    expect(cardHeaderDe)
      .withContext('render card header')
      .not.toBeNull();

    await card.getHarness(
      MatButtonHarness.with({
        ancestor: 'mat-card-header',
        text: 'Добавить датчик',
        variant: 'flat'
      })
    );
  });

  it('should get sensors', () => {
    expect(sensorsSpy)
      .toHaveBeenCalled();
  });

  it('should render tracker sensor table', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Датчики'
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
        title: 'Датчики'
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
        title: 'Датчики'
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
        title: 'Датчики'
      })
    );

    const table = await card.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    const actionButtons = await parallel(() => cells.map(
      ([actionCell]) => parallel(() => [
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            selector: '[bioTableActionsTrigger]',
            variant: 'icon',
            text: 'more_horiz'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '.actions',
            variant: 'icon',
            text: 'edit'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '.actions',
            variant: 'icon',
            text: 'content_copy'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '.actions',
            variant: 'icon',
            text: 'delete'
          })
        )
      ])
    ));

    actionButtons.forEach(([actionButton, updateButton, duplicateButton, deleteButton]) => {
      expect(actionButton)
        .withContext('render action button')
        .not.toBeNull();

      expect(updateButton)
        .withContext('render update button')
        .not.toBeNull();

      expect(duplicateButton)
        .withContext('render duplicate button')
        .not.toBeNull();

      expect(deleteButton)
        .withContext('render delete button')
        .not.toBeNull();

      actionButton!.hasHarness(
        MatIconHarness.with({
          name: 'more_horiz'
        })
      );

      updateButton!.hasHarness(
        MatIconHarness.with({
          name: 'edit'
        })
      );

      duplicateButton!.hasHarness(
        MatIconHarness.with({
          name: 'content_copy'
        })
      );

      deleteButton!.hasHarness(
        MatIconHarness.with({
          name: 'delete'
        })
      );
    });
  });

  it('should render tracker sensor table cells', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Датчики'
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
        title: 'Датчики'
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

  it('should create tracker sensor', async () => {
    const card = await loader.getHarness(
      MatCardHarness.with({
        title: 'Датчики'
      })
    );

    const createSensorButton = await card.getHarness(
      MatButtonHarness.with({
        variant: 'flat',
        text: 'Добавить датчик'
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
      afterClosed: () => of(testSensor)
    } as MatDialogRef<SensorDialogComponent, Sensor>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await createSensorButton.click();
  });

  it('should update tracker sensor', async () => {
    const updateSensorButtons = await loader.getAllHarnesses(
      MatButtonHarness.with({
        ancestor: '.mat-column-action .actions',
        selector: '[mat-icon-button]',
        text: 'edit'
      })
    );

    await updateSensorButtons[0].click();

    const sensorDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(sensorDialog)
      .withContext('render a tracker sensor dialog')
      .toBeDefined();

    await sensorDialog!.close();

    overlayContainer.ngOnDestroy();

    /* Coverage for updating sensors. */

    const dialogRef = {
      afterClosed: () => of(testSensor)
    } as MatDialogRef<SensorDialogComponent, Sensor>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await updateSensorButtons[0].click();
  });

  it('should duplicate tracker sensor', async () => {
    const duplicateSensorButtons = await loader.getAllHarnesses(
      MatButtonHarness.with({
        ancestor: '.mat-column-action .actions',
        selector: '[mat-icon-button]',
        text: 'content_copy'
      })
    );

    await duplicateSensorButtons[0].click();

    const sensorDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(sensorDialog)
      .withContext('render a tracker sensor dialog')
      .toBeDefined();

    await sensorDialog!.close();

    overlayContainer.ngOnDestroy();

    /* Coverage for updating sensors. */

    const dialogRef = {
      afterClosed: () => of(testSensor)
    } as MatDialogRef<SensorDialogComponent, Sensor>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await duplicateSensorButtons[0].click();
  });

  it('should delete sensor', async () => {
    const deleteSensorSpy = spyOn(sensorService, 'deleteSensor')
      .and.callFake(() => of(null));

    let deleteButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: '.mat-column-action .actions',
        variant: 'icon',
        text: 'delete'
      })
    );

    await deleteButton.click();

    let confirmationDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(confirmationDialog)
      .withContext('render a confirmation dialog')
      .not.toBeNull();

    let confirmButton = await confirmationDialog!.getHarness(
      MatButtonHarness.with({
        ancestor: 'mat-dialog-actions',
        variant: 'flat',
        text: 'Удалить'
      })
    );

    await confirmButton.click();

    expect(sensorService.deleteSensor)
      .toHaveBeenCalledWith(testSensors.sensors[0].id);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(SENSOR_DELETED);

    expect(sensorsSpy)
      .toHaveBeenCalled();

    deleteSensorSpy.calls.reset();

    /* Handle an error although update sensors anyway. */

    const testURL = `${environment.api}/api/telematica/sensor/${testSensors.sensors[1].id}`;

    const testErrorResponse = new HttpErrorResponse({
      error: {
        message: `Http failure response for ${testURL}: 500 Internal Server Error`
      },
      status: 500,
      statusText: 'Internal Server Error',
      url: testURL
    });

    const errorHandler = TestBed.inject(ErrorHandler);

    spyOn(console, 'error');
    spyOn(errorHandler, 'handleError');

    deleteSensorSpy.and.callFake(() => throwError(() => testErrorResponse));

    deleteButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: '.mat-column-action .actions',
        variant: 'icon',
        text: 'delete'
      })
    );

    await deleteButton.click();

    confirmationDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    confirmButton = await confirmationDialog!.getHarness(
      MatButtonHarness.with({
        ancestor: 'mat-dialog-actions',
        variant: 'flat',
        text: 'Удалить'
      })
    );

    await confirmButton.click();

    expect(sensorService.deleteSensor)
      .toHaveBeenCalledWith(testSensors.sensors[1].id);

    expect(errorHandler.handleError)
      .toHaveBeenCalledWith(testErrorResponse);

    expect(sensorsSpy)
      .toHaveBeenCalled();
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
