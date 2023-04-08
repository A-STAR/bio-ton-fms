import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DATE_PIPE_DEFAULT_OPTIONS, formatDate, formatNumber, registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { HttpClientModule } from '@angular/common/http';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatDialogTitle, MAT_DIALOG_DATA, MatDialogClose } from '@angular/material/dialog';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatChipSetHarness } from '@angular/material/chips/testing';
import { MatButtonHarness } from '@angular/material/button/testing';

import { Observable, of } from 'rxjs';

import { TrackerParametersHistory, TrackerService } from '../tracker.service';

import {
  ParameterHistoryColumn,
  trackerParameterHistoryColumns,
  TrackerParametersHistoryDialogComponent
} from './tracker-parameters-history-dialog.component';

import { localeID } from '../tracker-dialog/tracker-dialog.component';
import { dateFormat } from '../trackers/trackers.component.spec';
import { testParametersHistory, TEST_TRACKER_ID } from '../tracker.service.spec';

describe('TrackerParametersHistoryDialogComponent', () => {
  let component: TrackerParametersHistoryDialogComponent;
  let fixture: ComponentFixture<TrackerParametersHistoryDialogComponent>;
  let loader: HarnessLoader;

  let parametersHistorySpy: jasmine.Spy<(this: TrackerService) => Observable<TrackerParametersHistory>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientModule,
          TrackerParametersHistoryDialogComponent
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
            provide: MAT_DIALOG_DATA,
            useValue: TEST_TRACKER_ID
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(TrackerParametersHistoryDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    const trackerService = TestBed.inject(TrackerService);

    const parametersHistory$ = of(testParametersHistory);

    parametersHistorySpy = spyOn(trackerService, 'getParametersHistory')
      .and.returnValue(parametersHistory$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get parameters history', () => {
    expect(parametersHistorySpy)
      .toHaveBeenCalled();
  });

  it('should render dialog title', async () => {
    const titleDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(titleDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('История значений параметров');
  });

  it('should render parameters history table', async () => {
    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  });

  it('should render parameters history table rows', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testParametersHistory.parameters.length);
  });

  it('should render parameters history table header cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(trackerParameterHistoryColumns.length);

    const headerCellTexts = await parallel(() => headerCells.map(
      cell => cell.getText()
    ));

    const columnLabels = trackerParameterHistoryColumns.map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  });

  it('should render parameters history table cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(trackerParameterHistoryColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells => parallel(
        () => rowCells
          .slice(0, -1)
          .map(cell => cell.getText())
      )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const { time, latitude, longitude, altitude, speed } = testParametersHistory.parameters[index];

      const hash = index + 1;

      const formattedTime = formatDate(time, dateFormat, localeID);

      let formattedLatitude: string | undefined;
      let formattedLongitude: string | undefined;
      let formattedAltitude: string | undefined;
      let formattedSpeed: string | undefined;
      let coordinates: string | undefined;

      if (latitude) {
        formattedLatitude = formatNumber(latitude, 'en-US', '1.6-6');
      }

      if (longitude) {
        formattedLongitude = formatNumber(longitude, 'en-US', '1.6-6');
      }

      if (latitude && longitude) {
        coordinates = `${formattedLatitude} ${formattedLongitude}`;
      }

      if (altitude) {
        formattedAltitude = formatNumber(altitude, 'en-US', '1.1-1');
      }

      if (speed) {
        formattedSpeed = formatNumber(speed, 'en-US', '1.1-1');
      }

      const parametersHistoryTexts = [hash, formattedTime, formattedSpeed, coordinates, formattedAltitude].map(
        value => value?.toString() ?? ''
      );

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(parametersHistoryTexts);
    });
  });

  it('should render parameters cells chips', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells({
        columnName: ParameterHistoryColumn.Parameters
      })
    ));

    const parametersChipSets = await parallel(() => cells.map(
      ([parametersCell]) => parametersCell.getHarness(MatChipSetHarness))
    );

    parametersChipSets.forEach(async (parametersChipSet, index) => {
      const chips = await parametersChipSet.getChips();

      const chipTexts = await parallel(() => chips.map(
        chip => chip.getText()
      ));

      const parameters = testParametersHistory.parameters[index].parameters
        .split(',')
        .slice(0, -1);

      expect(chipTexts)
        .withContext('render chip texts')
        .toEqual(parameters);
    });

    parametersChipSets.forEach(parametersChipSet => {
      expect(parametersChipSet)
        .withContext('render chip set')
        .not.toBeNull();
    });
  });

  it('should render dialog actions', async () => {
    const closeButton = await loader.getHarnessOrNull(
      MatButtonHarness.with({
        text: 'Закрыть',
        variant: 'stroked'
      })
    );

    expect(closeButton)
      .withContext('render close button')
      .not.toBeNull();

    const dialogCloseDe = fixture.debugElement.query(
      By.directive(MatDialogClose)
    );

    expect(dialogCloseDe)
      .withContext('render dialog close element')
      .not.toBeNull();
  });
});
