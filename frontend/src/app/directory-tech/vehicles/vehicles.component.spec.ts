import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { MatSortHarness } from '@angular/material/sort/testing';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { MatDialogRef } from '@angular/material/dialog';

import { Observable, of } from 'rxjs';

import { Vehicles, VehicleService } from '../vehicle.service';

import { columns, VehicleColumn, VehiclesComponent } from './vehicles.component';
import { VehicleDialogComponent } from '../vehicle-dialog/vehicle-dialog.component';

import { testVehicles } from '../vehicle.service.spec';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;
  let overlayContainer: OverlayContainer;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;

  let vehiclesSpy: jasmine.Spy<() => Observable<Vehicles>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          VehiclesComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(VehiclesComponent);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);
    overlayContainer = TestBed.inject(OverlayContainer);

    const vehicleService = TestBed.inject(VehicleService);

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

  it('should render add vehicle button', async () => {
    const buttons = await loader.getAllHarnesses(MatButtonHarness.with({
      selector: '[mat-stroked-button]',
      text: 'Добавить технику'
    }));

    expect(buttons.length)
      .withContext('render an add vehicle button')
      .toBe(1);
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
      .toBe(12);

    const headerCellTexts = await parallel(
      () => headerCells
        .slice(1)
        .map(cell => cell.getText())
    );

    const columnLabels = columns
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

    const columnLabels = [columns[1].value, columns[4].value, columns[5].value, columns[6].value, columns[8].value]
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
      row => row.getCells()
    ));

    const updateButtons = await parallel(() => cells.map(
      ({
        0: actionCell
      }) => actionCell.getHarnessOrNull(MatButtonHarness)
    ));

    updateButtons.forEach(updateButton => {
      expect(updateButton)
        .withContext('render update button')
        .toBeDefined();

      updateButton?.hasHarness(MatIconHarness.with({
        name: 'edit'
      }));
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
        .toBe(12);
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
        description,
        tracker: {
          value: tracker
        } = {
          value: undefined
        }
      } = testVehicles.vehicles[index];

      const registration = registrationNumber
        ?.split(' ')
        .join('');

      const vehicleTexts = [
        name,
        make,
        model,
        type,
        subtype,
        group,
        year,
        fuel,
        registration,
        tracker,
        description
      ].map(value => value?.toString() ?? '');

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
      selector: '[mat-stroked-button]',
      text: 'Добавить технику'
    }));

    await createVehicleButton.click();

    const vehicleDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(length)
      .withContext('render a vehicle dialog')
      .toBeDefined();

    await vehicleDialog?.close();

    overlayContainer.ngOnDestroy();

    const dialogRef = {
      afterClosed: () => of(true)
    } as MatDialogRef<VehicleDialogComponent, true | ''>;

    spyOn(component['dialog'], 'open')
      .and.returnValue(dialogRef);

    await createVehicleButton.click();
  });

  it('should update vehicle', async () => {
    const updateVehicleButtons = await loader.getAllHarnesses(MatButtonHarness.with({
      ancestor: '.mat-column-action',
      selector: '[mat-icon-button]',
      text: 'edit'
    }));

    await updateVehicleButtons[0].click();

    let vehicleDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    expect(vehicleDialog)
      .withContext('render a vehicle dialog')
      .toBeDefined();

    await vehicleDialog?.close();

    await updateVehicleButtons[1].click();

    vehicleDialog = await documentRootLoader.getHarnessOrNull(MatDialogHarness);

    await vehicleDialog?.close();

    overlayContainer.ngOnDestroy();
  });
});
