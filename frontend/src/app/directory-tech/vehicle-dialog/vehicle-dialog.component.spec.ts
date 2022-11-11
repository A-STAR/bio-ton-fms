import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';

import { Observable, of } from 'rxjs';

import { Fuel, VehicleGroup, VehicleService } from '../vehicle.service';

import { VehicleDialogComponent } from './vehicle-dialog.component';

import { testFuels, testVehicleGroups, testVehicleSubtypeEnum, testVehicleTypeEnum } from '../vehicle.service.spec';

describe('VehicleDialogComponent', () => {
  let component: VehicleDialogComponent;
  let fixture: ComponentFixture<VehicleDialogComponent>;
  let loader: HarnessLoader;

  let vehicleGroupsSpy: jasmine.Spy<(this: VehicleService) => Observable<VehicleGroup[]>>;
  let fuelsSpy: jasmine.Spy<(this: VehicleService) => Observable<Fuel[]>>;
  let vehicleTypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<string, string>[]>>;
  let vehicleSubtypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<string, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          VehicleDialogComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(VehicleDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    const vehicleService = TestBed.inject(VehicleService);

    component = fixture.componentInstance;

    const vehicleGroups$ = of(testVehicleGroups);
    const fuels$ = of(testFuels);
    const vehicleType$ = of(testVehicleTypeEnum);
    const vehicleSubtype$ = of(testVehicleSubtypeEnum);

    vehicleGroupsSpy = spyOnProperty(vehicleService, 'vehicleGroups$')
      .and.returnValue(vehicleGroups$);

    fuelsSpy = spyOnProperty(vehicleService, 'fuels$')
      .and.returnValue(fuels$);

    vehicleTypeSpy = spyOnProperty(vehicleService, 'vehicleType$')
      .and.returnValue(vehicleType$);

    vehicleSubtypeSpy = spyOnProperty(vehicleService, 'vehicleSubtype$')
      .and.returnValue(vehicleSubtype$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
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

    expect(vehicleSubtypeSpy)
      .toHaveBeenCalled();
  });

  it('should render dialog title', async () => {
    const titleTextDe = fixture.debugElement.query(By.css('[mat-dialog-title]'));

    expect(titleTextDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleTextDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('Сводная информация о техническом средстве');
  });

  it('should render vehicle form', async () => {
    const vehicleFormDe = fixture.debugElement.query(By.css('form#vehicle-form'));

    expect(vehicleFormDe)
      .withContext('render Vehicle form element')
      .not.toBeNull();

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Наименование машины'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Производитель'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Модель'
    }));

    const yearInput = await loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Год производства'
    }));

    await expectAsync(
      yearInput.getType()
    )
      .withContext('render year input type')
      .toBeResolvedTo('number');

    loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Группа машин"]'
    }));

    loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Тип машины"]'
    }));

    loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Подтип машины'
    }));

    loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Тип топлива"]'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Регистрационный номер'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Инвентарный номер'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Серийный номер кузова'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'GPS трекер'
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Описание'
    }));
  });
});
