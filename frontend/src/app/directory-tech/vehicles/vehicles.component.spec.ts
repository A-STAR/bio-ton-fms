import { ErrorHandler } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogRef } from '@angular/material/dialog';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatSortHarness } from '@angular/material/sort/testing';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of, throwError } from 'rxjs';

import { Vehicles, VehicleService } from '../vehicle.service';

import VehiclesComponent, { vehicleColumns, VehicleColumn, VEHICLE_DELETED } from './vehicles.component';
import { VehicleDialogComponent } from '../vehicle-dialog/vehicle-dialog.component';

import { environment } from '../../../../src/environments/environment';
import { testVehicles } from '../vehicle.service.spec';
import { testDataSource as testVehiclesDataSource } from '../shared/table/table.data-source.spec';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let vehicleService: VehicleService;

  let vehiclesSpy: jasmine.Spy<() => Observable<Vehicles>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          RouterTestingModule,
          MatSnackBarModule,
          VehiclesComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(VehiclesComponent);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);
    overlayContainer = TestBed.inject(OverlayContainer);
    vehicleService = TestBed.inject(VehicleService);

    component = fixture.componentInstance;

    const vehicles$ = of(testVehicles);

    vehiclesSpy = spyOn(vehicleService, 'getVehicles')
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

  it('should render create vehicle button', async () => {
    const createVehicleButton = await loader.getHarnessOrNull(MatButtonHarness.with({
      variant: 'stroked',
      text: 'Добавить технику'
    }));

    expect(createVehicleButton)
      .withContext('render a create vehicle button')
      .toBeDefined();
  });

  it('should render vehicle table', async () => {
    const tables = await loader.getAllHarnesses(MatTableHarness);

    expect(tables.length)
      .withContext('render a table')
      .toBe(1);
  });

  it('should render vehicle table rows', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();
    const rows = await table.getRows();

    expect(headerRows.length)
      .withContext('render a header row')
      .toBe(1);

    expect(rows.length)
      .withContext('render rows')
      .toBe(testVehicles.vehicles.length);
  });

  it('should render vehicle table header cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const headerRows = await table.getHeaderRows();

    const [headerCells] = await parallel(() => headerRows.map(
      row => row.getCells()
    ));

    expect(headerCells.length)
      .withContext('render header cells')
      .toBe(vehicleColumns.length);

    const headerCellTexts = await parallel(
      () => headerCells
        .slice(1)
        .map(cell => cell.getText())
    );

    const columnLabels = vehicleColumns
      .filter((column): column is KeyValue<VehicleColumn, string> => column.value !== undefined)
      .map(
        ({ value }) => value
          .replace('&#10;', '\n')
          .replace('&shy;', '­')
      );

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
  });

  it('should render vehicle table sort headers', async () => {
    const sorts = await loader.getAllHarnesses(MatSortHarness);

    expect(sorts.length)
      .withContext('render a sort')
      .toBe(1);

    const [sort] = sorts;

    let sortHeaders = await sort.getSortHeaders({
      sortDirection: ''
    });

    expect(sortHeaders.length)
      .withContext('render sort headers')
      .toBe(5);

    const sortHeaderLabels = await parallel(() => sortHeaders.map(
      header => header.getLabel()
    ));

    const columnLabels = [
      vehicleColumns[1].value,
      vehicleColumns[4].value,
      vehicleColumns[5].value,
      vehicleColumns[6].value,
      vehicleColumns[8].value
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

  it('should render vehicle table action cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells({
        columnName: VehicleColumn.Action
      })
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
            selector: 'a',
            variant: 'icon',
            text: 'location_on'
          })
        ),
        actionCell.getHarnessOrNull(
          MatButtonHarness.with({
            ancestor: '.actions',
            variant: 'icon',
            text: 'sms'
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

    actionButtons.forEach(async ([actionButton, updateButton, trackerAnchor, commandButton, deleteButton], index) => {
      expect(actionButton)
        .withContext('render action button')
        .not.toBeNull();

      expect(updateButton)
        .withContext('render update button')
        .not.toBeNull();

      if (testVehicles.vehicles[index].tracker) {
        expect(trackerAnchor)
          .withContext('render GPS tracker anchor')
          .not.toBeNull();

        const trackerAnchorEl = await trackerAnchor?.host();

        const [trackerRouterLink, trackerAnchorTitle] = await Promise.all([
          trackerAnchorEl?.getAttribute('ng-reflect-router-link'),
          trackerAnchorEl?.getAttribute('title')
        ]);

        expect(trackerRouterLink)
          .withContext('render GPS tracker anchor router link')
          .toBe(
            ['../trackers', testVehicles.vehicles[index].tracker?.id].join()
          );

        expect(trackerAnchorTitle)
          .withContext('render GPS tracker anchor title')
          .toBe('GPS-трекер');

        trackerAnchor!.hasHarness(
          MatIconHarness.with({
            name: 'location_on'
          })
        );

        expect(commandButton)
          .withContext('render command GPS tracker button')
          .not.toBeNull();

        commandButton!.hasHarness(
          MatIconHarness.with({
            name: 'sms'
          })
        );

        const commandButtonEl = await commandButton?.host();
        const commandButtonTitle = await commandButtonEl?.getAttribute('title');

        expect(commandButtonTitle)
          .withContext('render GPS tracker command button title')
          .toBe('Отправить команду');
      }

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

      deleteButton!.hasHarness(
        MatIconHarness.with({
          name: 'delete'
        })
      );
    });
  });

  it('should render vehicle table cells', async () => {
    const table = await loader.getHarness(MatTableHarness);
    const rows = await table.getRows();

    const cells = await parallel(() => rows.map(
      row => row.getCells()
    ));

    cells.forEach(({ length }) => {
      expect(length)
        .withContext('render cells')
        .toBe(vehicleColumns.length);
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
        type: {
          value: type
        },
        vehicleGroup: {
          value: group
        } = {
          value: undefined
        },
        make,
        model,
        subType: {
          value: subtype
        },
        fuelType: {
          value: fuel
        },
        manufacturingYear: year,
        registrationNumber,
        tracker: {
          value: tracker
        } = {
          value: undefined
        }
      } = testVehicles.vehicles[index];

      const registration = registrationNumber
        ?.split(' ')
        .join('');

      const vehicleTexts = [name, make, model, type, subtype, group, year, fuel, registration, tracker].map(
        value => value?.toString() ?? ''
      );

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(vehicleTexts);
    });
  });

  it('should sort vehicle table', async () => {
    const sort = await loader.getHarness(MatSortHarness);

    const [nameSortHeader, typeSortHeader, subtypeSortHeader, groupSortHeader, fuelSortHeader] = await sort.getSortHeaders();

    await nameSortHeader.click();

    let sortDirection = await nameSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render name sort header sorted')
      .toBe('asc');

    await typeSortHeader.click();

    sortDirection = await typeSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render type sort header sorted')
      .toBe('asc');

    await subtypeSortHeader.click();

    sortDirection = await subtypeSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render subtype sort header sorted')
      .toBe('asc');

    await groupSortHeader.click();

    sortDirection = await groupSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render group sort header sorted')
      .toBe('asc');

    await fuelSortHeader.click();

    sortDirection = await fuelSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render fuel sort header sorted')
      .toBe('asc');

    await fuelSortHeader.click();

    sortDirection = await fuelSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render fuel sort header sorted in descending direction')
      .toBe('desc');

    await fuelSortHeader.click();

    sortDirection = await fuelSortHeader.getSortDirection();

    expect(sortDirection)
      .withContext('render fuel sort header unsorted')
      .toBe('');
  });

  it('should create vehicle', async () => {
    const createVehicleButton = await loader.getHarness(MatButtonHarness.with({
      variant: 'stroked',
      text: 'Добавить технику'
    }));

    await createVehicleButton.click();

    const vehicleDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(vehicleDialog)
      .withContext('render a vehicle dialog')
      .toBeDefined();

    await vehicleDialog!.close();

    expect(vehiclesSpy)
      .toHaveBeenCalled();

    overlayContainer.ngOnDestroy();

    /* Coverage for updating vehicles. */

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<VehicleDialogComponent, true | '' | undefined>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await createVehicleButton.click();
  });

  it('should update vehicle', async () => {
    const updateVehicleButtons = await loader.getAllHarnesses(MatButtonHarness.with({
      ancestor: '.mat-column-action .actions',
      selector: '[mat-icon-button]',
      text: 'edit'
    }));

    await updateVehicleButtons[0].click();

    const vehicleDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(vehicleDialog)
      .withContext('render a vehicle dialog')
      .toBeDefined();

    await vehicleDialog!.close();

    expect(vehiclesSpy)
      .toHaveBeenCalled();

    overlayContainer.ngOnDestroy();

    /* Coverage for updating vehicles. */

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<VehicleDialogComponent, true | '' | undefined>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await updateVehicleButtons[1].click();
  });

  it('should delete vehicle', async () => {
    const deleteVehicleSpy = spyOn(vehicleService, 'deleteVehicle')
      .and.callFake(() => of({}));

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

    expect(vehicleService.deleteVehicle)
      .toHaveBeenCalledWith(testVehiclesDataSource[0].id);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(VEHICLE_DELETED);

    expect(vehiclesSpy)
      .toHaveBeenCalled();

    deleteVehicleSpy.calls.reset();

    /* Handle an error although update vehicles anyway. */

    const testURL = `${environment.api}/api/telematica/vehicle/${testVehiclesDataSource[1].id}`;

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
    deleteVehicleSpy.and.callFake(() => throwError(() => testErrorResponse));

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

    expect(vehicleService.deleteVehicle)
      .toHaveBeenCalledWith(testVehiclesDataSource[1].id);

    expect(errorHandler.handleError)
      .toHaveBeenCalledWith(testErrorResponse);

    expect(vehiclesSpy)
      .toHaveBeenCalled();

    overlayContainer.ngOnDestroy();
  });
});
