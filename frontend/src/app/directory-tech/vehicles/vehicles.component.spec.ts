import { ComponentFixture, TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatTableHarness } from '@angular/material/table/testing';

import { Observable, of } from 'rxjs';

import { Fuel, VehicleGroup, Vehicles, VehicleService } from '../vehicle.service';

import { columns, VehicleColumn, VehiclesComponent } from './vehicles.component';

import { testFuels, testVehicleGroups, testVehicles } from '../vehicle.service.spec';

import { testVehicleSubTypeEnum, testVehicleTypeEnum } from '../vehicle.service.spec';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;
  let loader: HarnessLoader;

  let vehiclesSpy: jasmine.Spy<() => Observable<Vehicles>>;
  let vehicleGroupsSpy: jasmine.Spy<(this: VehicleService) => Observable<VehicleGroup[]>>;
  let fuelsSpy: jasmine.Spy<(this: VehicleService) => Observable<Fuel[]>>;
  let vehicleTypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<string, string>[]>>;
  let vehicleSubTypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<string, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          VehiclesComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(VehiclesComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    const vehicleService = TestBed.inject(VehicleService);

    component = fixture.componentInstance;

    const vehicles$ = of(testVehicles);
    const vehicleGroups$ = of(testVehicleGroups);
    const fuels$ = of(testFuels);
    const vehicleType$ = of(testVehicleTypeEnum);
    const vehicleSubType$ = of(testVehicleSubTypeEnum);

    vehiclesSpy = spyOn(vehicleService, 'getVehicles')
      .and.returnValue(vehicles$);

    vehicleGroupsSpy = spyOnProperty(vehicleService, 'vehicleGroups$')
      .and.returnValue(vehicleGroups$);

    fuelsSpy = spyOnProperty(vehicleService, 'fuels$')
      .and.returnValue(fuels$);

    vehicleTypeSpy = spyOnProperty(vehicleService, 'vehicleType$')
      .and.returnValue(vehicleType$);

    vehicleSubTypeSpy = spyOnProperty(vehicleService, 'vehicleSubType$')
      .and.returnValue(vehicleSubType$);

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

  it('should get vehicle groups, fuels', () => {
    expect(vehicleGroupsSpy)
      .toHaveBeenCalled();

    expect(fuelsSpy)
      .toHaveBeenCalled();
  });

  it('should get vehicle type, subtype enums', () => {
    expect(vehicleTypeSpy)
      .toHaveBeenCalled();

    expect(vehicleSubTypeSpy)
      .toHaveBeenCalled();
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
      .map(({ value }) => value);

    expect(headerCellTexts)
      .withContext('render column labels')
      .toEqual(columnLabels);
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
        type: typeKey,
        vehicleGroupId,
        make,
        model,
        subType: subtypeKey,
        fuelTypeId,
        manufacturingYear: year,
        registrationNumber: registration,
        description,
        tracker
      } = testVehicles.vehicles[index];

      const type = testVehicleTypeEnum.find(({ key }) => key === typeKey);
      const subtype = testVehicleSubTypeEnum.find(({ key }) => key === subtypeKey);
      const group = testVehicleGroups.find(({ id }) => id === vehicleGroupId);
      const fuel = testFuels.find(({ id }) => id === fuelTypeId);

      const vehicleTexts = [
        name,
        make,
        model,
        type?.value,
        subtype?.value,
        group?.name,
        year,
        fuel?.name,
        registration,
        tracker?.name,
        description
      ].map(value => value?.toString() ?? '');

      expect(rowCellTexts)
        .withContext('render cells text')
        .toEqual(vehicleTexts);
    });
  });
});
