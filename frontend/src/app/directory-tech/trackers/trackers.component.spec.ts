import { ErrorHandler, LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DATE_PIPE_DEFAULT_OPTIONS, KeyValue, formatDate, registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { OverlayContainer } from '@angular/cdk/overlay';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatSortHarness } from '@angular/material/sort/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of, throwError } from 'rxjs';

import { TrackerService, Trackers } from '../tracker.service';

import TrackersComponent, { TRACKER_DELETED, TrackerColumn, trackerColumns } from './trackers.component';
import { TrackerDialogComponent } from '../tracker-dialog/tracker-dialog.component';

import { environment } from '../../../environments/environment';
import { localeID } from '../../tech/shared/relative-time.pipe';
import { testTrackers } from '../tracker.service.spec';

describe('TrackersComponent', () => {
  let component: TrackersComponent;
  let fixture: ComponentFixture<TrackersComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let trackerService: TrackerService;

  let trackersSpy: jasmine.Spy<() => Observable<Trackers>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          RouterTestingModule,
          MatSnackBarModule,
          MatDialogModule,
          TrackersComponent
        ],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: localeID
          },
          {
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: { dateFormat }
          }
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(TrackersComponent);
    overlayContainer = TestBed.inject(OverlayContainer);
    trackerService = TestBed.inject(TrackerService);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;

    const trackers$ = of(testTrackers);

    trackersSpy = spyOn(trackerService, 'getTrackers')
      .and.returnValue(trackers$);

    fixture.detectChanges();
  });

  afterEach(async () => {
    const dialogs = await documentRootLoader.getAllHarnesses(MatDialogHarness);

    await parallel(() => dialogs.map(
      dialog => dialog.close()
    ));

    overlayContainer.ngOnDestroy();
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

  it('should render tracker table', async () => {
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

  it('should render tracker table row links', async () => {
    const table = await loader.getHarness(MatTableHarness);

    const rows = await table.getRows({
      selector: '[role="link"]'
    });

    rows.forEach(async (row, index) => {
      const rowEl = await row.host();
      const routerLink = await rowEl.getAttribute('ng-reflect-router-link');

      expect(routerLink)
        .withContext('render GPS tracker row router link')
        .toBe(
          testTrackers.trackers[index].id.toString()
        );
    });
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
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            selector: '[bioTableActionsTrigger][bioStopClickPropagation]',
            variant: 'icon',
            text: 'more_horiz'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '[bioStopClickPropagation].actions',
            selector: '[bioStopClickPropagation]',
            variant: 'icon',
            text: 'edit'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '[bioStopClickPropagation].actions',
            selector: '[bioStopClickPropagation]',
            variant: 'icon',
            text: 'sms'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '[bioStopClickPropagation].actions',
            selector: '[bioStopClickPropagation]',
            variant: 'icon',
            text: 'delete'
          })
        )
      ])
    ));

    actionButtons.forEach(async ([actionButton, updateButton, commandButton, deleteButton]) => {
      expect(actionButton)
        .withContext('render action button')
        .not.toBeNull();

      expect(updateButton)
        .withContext('render update button')
        .not.toBeNull();

      expect(commandButton)
        .withContext('render command button')
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

      commandButton!.hasHarness(
        MatIconHarness.with({
          name: 'sms'
        })
      );

      deleteButton!.hasHarness(
        MatIconHarness.with({
          name: 'delete'
        })
      );
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
      rowCells => parallel(
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

      const sim = simNumber ? `${simNumber.slice(0, 2)} (${simNumber.slice(2, 5)}) ${simNumber.slice(4)}` : '';
      const start = formatDate(startDate, dateFormat, localeID);

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

  it('should create tracker', async () => {
    const createButton = await loader.getHarness(
      MatButtonHarness.with({
        variant: 'stroked',
        text: 'Добавить GPS-трекер'
      })
    );

    await createButton.click();

    const trackerDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(trackerDialog)
      .withContext('render a tracker dialog')
      .not.toBeNull();

    expect(trackersSpy)
      .toHaveBeenCalled();

    /* Coverage for updating trackers. */

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<TrackerDialogComponent, true | '' | undefined>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await createButton.click();
  });

  it('should update tracker', async () => {
    const updateButtons = await loader.getAllHarnesses(
      MatButtonHarness.with({
        ancestor: '.mat-column-action .actions',
        selector: '[mat-icon-button]',
        text: 'edit'
      })
    );

    await updateButtons[0].click();

    const trackerDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(trackerDialog)
      .withContext('render a tracker dialog')
      .toBeDefined();

    /* Coverage for updating trackers. */

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<TrackerDialogComponent, true | ''>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await updateButtons[0].click();
  });

  it('should render tracker command dialog', async () => {
    const commandButtons = await loader.getAllHarnesses(
      MatButtonHarness.with({
        ancestor: '.mat-column-action .actions',
        selector: '[mat-icon-button]',
        text: 'sms'
      })
    );

    await commandButtons[0].click();

    const commandTrackerDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(commandTrackerDialog)
      .withContext('render a tracker command dialog')
      .toBeDefined();
  });

  it('should delete tracker', async () => {
    const deleteTrackerSpy = spyOn(trackerService, 'deleteTracker')
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

    expect(trackerService.deleteTracker)
      .toHaveBeenCalledWith(testTrackers.trackers[0].id);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(TRACKER_DELETED);

    expect(trackersSpy)
      .toHaveBeenCalled();

    deleteTrackerSpy.calls.reset();

    /* Handle an error although update trackers anyway. */

    const testURL = `${environment.api}/api/telematica/tracker/${testTrackers.trackers[1].id}`;

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

    deleteTrackerSpy.and.callFake(() => throwError(() => testErrorResponse));

    [, deleteButton] = await loader.getAllHarnesses(
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

    expect(trackerService.deleteTracker)
      .toHaveBeenCalledWith(testTrackers.trackers[1].id);

    expect(errorHandler.handleError)
      .toHaveBeenCalledWith(testErrorResponse);

    expect(trackersSpy)
      .toHaveBeenCalled();
  });
});

export const dateFormat = 'd MMMM y, H:mm';
