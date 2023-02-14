import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { formatDate, KeyValue, registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatSortHarness } from '@angular/material/sort/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';

import { Observable, of } from 'rxjs';

import { Trackers, TrackerService } from '../tracker.service';

import TrackersComponent, { DATE_FORMAT, TrackerColumn, trackerColumns } from './trackers.component';

import { testTrackers } from '../tracker.service.spec';

describe('TrackersComponent', () => {
  let component: TrackersComponent;
  let fixture: ComponentFixture<TrackersComponent>;
  let loader: HarnessLoader;

  let trackersSpy: jasmine.Spy<() => Observable<Trackers>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          TrackersComponent
        ],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: 'ru-Ru'
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, 'ru-RU');

    fixture = TestBed.createComponent(TrackersComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    const trackerService = TestBed.inject(TrackerService);

    component = fixture.componentInstance;

    const trackers$ = of(testTrackers);

    trackersSpy = spyOn(trackerService, 'getTrackers')
      .and.returnValue(trackers$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get trackers', () => {
    expect(trackersSpy)
      .toHaveBeenCalled();
  });

  it('should render create tracker button', async () => {
    const button = await loader.getHarnessOrNull(
      MatButtonHarness.with({
        variant: 'stroked',
        text: 'Добавить GPS-трекер'
      })
    );

    expect(button)
      .withContext('render an create tracker button')
      .not.toBeNull();
  });

  it('should render trackers table', async () => {
    const tables = await loader.getHarnessOrNull(MatTableHarness);

    expect(tables)
      .withContext('render a table')
      .not.toBeNull();
  });

  it('should render tracker table rows', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testTrackers.trackers.length);
  });

  it('should render tracker table header cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(trackerColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells
        .slice(1)
        .map(cell => cell.getText())
    );

    const columnLabels = trackerColumns
      .filter((column): column is KeyValue<TrackerColumn, string> => column.value !== undefined)
      .map(
        ({ value }) => value
          .replace('&#10;', '\n')
          .replace('&shy;', '­')
      );

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  });

  it('should render tracker table sort headers', async () => {
    const sort = await loader.getHarnessOrNull(MatSortHarness);

    expect(sort)
      .withContext('render a sort')
      .not.toBeNull();

    const sortHeaders = await sort!.getSortHeaders({
      sortDirection: ''
    });

    expect(sortHeaders!.length)
      .withContext('render sort headers')
      .toBe(6);

    const sortHeaderLabels = await parallel(() => sortHeaders!.map(
      header => header.getLabel()
    ));

    const columnLabels = [
      trackerColumns[1].value,
      trackerColumns[2].value,
      trackerColumns[3].value,
      trackerColumns[4].value,
      trackerColumns[6].value,
      trackerColumns[7].value
    ]
      .filter((value): value is string => value !== undefined)
      .map(
        value => value
          .replace('&#10;', '\n')
          .replace('&shy;', '­')
      );

    expect(sortHeaderLabels)
      .withContext('render sort header labels')
      .toEqual(columnLabels);
  });

  it('should render tracker table action cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells({
        columnName: TrackerColumn.Action
      })
    ));

    const actionButtons = await parallel(() => cells.map(
      ([actionCell]) => parallel(() => [
        actionCell.getHarnessOrNull(MatButtonHarness.with({
          variant: 'icon',
          text: 'more_horiz'
        }))
      ])
    ));

    actionButtons.forEach(([actionButton]) => {
      expect(actionButton)
        .withContext('render action button')
        .not.toBeNull();

      actionButton?.hasHarness(MatIconHarness.with({
        name: 'more_horiz'
      }));
    });
  });

  it('should render tracker table cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(trackerColumns.length);
    });

    const cellTexts = await parallel(() => cells.map(
      rowCells =>
        parallel(
          () => rowCells
            .slice(1)
            .map(cell => cell.getText())
        )
    ));

    cellTexts.forEach((rowCellTexts, index) => {
      const {
        name,
        externalId: external,
        simNumber,
        imei,
        trackerType: {
          value: type
        },
        startDate,
        vehicle: {
          value: vehicle
        } = {
          value: undefined
        }
      } = testTrackers.trackers[index];

      const sim = simNumber ? `+${simNumber[0]} (${simNumber.slice(1, 4)}) ${simNumber.slice(4)}` : '';
      const start = formatDate(startDate, DATE_FORMAT, 'ru-RU');

      const trackerTexts = [name, external, type, sim, imei, start, vehicle].map(
        value => value?.toString() ?? ''
      );

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(trackerTexts);
    });
  });

  it('should sort tracker table', async () => {
    const sort = await loader.getHarness(MatSortHarness);

    const [
      nameSortHeader,
      externalSortHeader,
      typeSortHeader,
      simSortHeader,
      startSortHeader,
      vehicleSortHeader
    ] = await sort.getSortHeaders();

    await nameSortHeader.click();

    let sortDirection = await nameSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render name sort header sorted')
      .toBe('asc');

    await externalSortHeader.click();

    sortDirection = await externalSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render external ID sort header sorted')
      .toBe('asc');

    await typeSortHeader.click();

    sortDirection = await typeSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render type sort header sorted')
      .toBe('asc');

    await simSortHeader.click();

    sortDirection = await simSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render sim number sort header sorted')
      .toBe('asc');

    await startSortHeader.click();

    sortDirection = await startSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render start date sort header sorted')
      .toBe('asc');

    await vehicleSortHeader.click();

    sortDirection = await vehicleSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render vehicle sort header sorted')
      .toBe('asc');

    await vehicleSortHeader.click();

    sortDirection = await vehicleSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render vehicle sort header sorted in descending direction')
      .toBe('desc');

    await vehicleSortHeader.click();

    sortDirection = await vehicleSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render vehicle sort header unsorted')
      .toBe('');
  });
});
