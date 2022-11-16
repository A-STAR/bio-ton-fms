import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';

import { Observable, of } from 'rxjs';

import { Fuel, NewVehicle, VehicleGroup, VehicleService } from '../vehicle.service';

import { VehicleDialogComponent } from './vehicle-dialog.component';

import { testFuels, testNewVehicle, testVehicleGroups, testVehicleSubtypeEnum, testVehicleTypeEnum } from '../vehicle.service.spec';

describe('VehicleDialogComponent', () => {
  let component: VehicleDialogComponent;
  let fixture: ComponentFixture<VehicleDialogComponent>;
  let loader: HarnessLoader;
  let vehicleService: VehicleService;

  const dialogRef = jasmine.createSpyObj<MatDialogRef<VehicleDialogComponent, true | ''>>('MatDialogRef', ['close']);

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
        ],
        providers: [
          {
            provide: MAT_DIALOG_DATA,
            useValue: undefined
          },
          {
            provide: MatDialogRef,
            useValue: dialogRef
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(VehicleDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);
    vehicleService = TestBed.inject(VehicleService);

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
      .toBe('Добавление технического средства');

    component['data'] = testNewVehicle;

    fixture.detectChanges();

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

    const trackerInput = await loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'GPS трекер'
    }));

    await expectAsync(
      trackerInput.getType()
    )
      .withContext('render tracker input type')
      .toBeResolvedTo('number');

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Описание'
    }));
  });

  it('should render update vehicle form', async () => {
    component['data'] = testNewVehicle;

    component.ngOnInit();

    fixture.detectChanges();

    const vehicleFormDe = fixture.debugElement.query(By.css('form#vehicle-form'));

    expect(vehicleFormDe)
      .withContext('render Vehicle form element')
      .not.toBeNull();

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Наименование машины',
      value: testNewVehicle.name
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Производитель',
      value: testNewVehicle.make
    }));

    loader.getHarness(MatInputHarness.with({
      ancestor: 'form#vehicle-form',
      placeholder: 'Модель',
      value: testNewVehicle.model
    }));

    const typeSelect = await loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Тип машины"]'
    }));

    await expectAsync(
      typeSelect.getValueText()
    )
      .withContext('render type select text')
      .toBeResolvedTo(testVehicleTypeEnum[0].value);

    const subtypeSelect = await loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Подтип машины"]'
    }));

    await expectAsync(
      subtypeSelect.getValueText()
    )
      .withContext('render subtype select text')
      .toBeResolvedTo(testVehicleSubtypeEnum[2].value);

    const fuelSelect = await loader.getHarness(MatSelectHarness.with({
      ancestor: 'form#vehicle-form',
      selector: '[placeholder="Тип топлива"]'
    }));

    await expectAsync(
      fuelSelect.getValueText()
    )
      .withContext('render fuel select text')
      .toBeResolvedTo(testFuels[0].name);
  });

  it('should submit invalid vehicle form', async () => {
    spyOn(component, 'submitVehicleForm')
      .and.callThrough();

    spyOn(vehicleService, 'createVehicle')
      .and.callThrough();

    const saveButton = await loader.getHarness(MatButtonHarness.with({
      selector: '[form="vehicle-form"]',
      text: 'Сохранить'
    }));

    await saveButton.click();

    expect(component.submitVehicleForm)
      .toHaveBeenCalled();

    expect(vehicleService.createVehicle)
      .not.toHaveBeenCalled();
  });

  it('should submit vehicle form', async () => {
    spyOn(component, 'submitVehicleForm')
      .and.callThrough();

    spyOn(vehicleService, 'createVehicle')
      .and.callThrough();

    const saveButton = await loader.getHarness(MatButtonHarness.with({
      selector: '[form="vehicle-form"]'
    }));

    await saveButton.click();

    expect(component.submitVehicleForm)
      .toHaveBeenCalled();

    expect(vehicleService.createVehicle)
      .not.toHaveBeenCalled();
  });

  it('should submit create vehicle form', async () => {
    const [nameInput, makeInput, modelInput] = await loader.getAllHarnesses(MatInputHarness.with({
      ancestor: 'form#vehicle-form'
    }));

    const [groupSelect, typeSelect, subtypeSelect, fuelSelect] = await loader.getAllHarnesses(MatSelectHarness.with({
      ancestor: 'form#vehicle-form'
    }));

    const { name, make, model } = testNewVehicle;

    await typeSelect.clickOptions({
      text: testVehicleTypeEnum[0].value
    });

    await subtypeSelect.clickOptions({
      text: testVehicleSubtypeEnum[2].value
    });

    await fuelSelect.clickOptions({
      text: testFuels[0].name
    });

    await nameInput.setValue(name);
    await makeInput.setValue(make);
    await modelInput.setValue(model);

    spyOn(component, 'submitVehicleForm')
      .and.callThrough();

    spyOn(vehicleService, 'createVehicle')
      .and.callFake(() => of({}));

    const saveButton = await loader.getHarness(MatButtonHarness.with({
      selector: '[form="vehicle-form"]'
    }));

    await saveButton.click();

    expect(component.submitVehicleForm)
      .toHaveBeenCalled();

    const testVehicle: NewVehicle = {
      name: testNewVehicle.name,
      make: testNewVehicle.make,
      model: testNewVehicle.model,
      type: testNewVehicle.type,
      manufacturingYear: undefined,
      subType: testNewVehicle.subType,
      fuelTypeId: testNewVehicle.fuelTypeId,
      vehicleGroupId: undefined,
      registrationNumber: undefined,
      inventoryNumber: undefined,
      serialNumber: undefined,
      trackerId: undefined,
      description: undefined
    };

    expect(vehicleService.createVehicle)
      .toHaveBeenCalledWith(testVehicle);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(true);

    await groupSelect.clickOptions({
      text: testVehicleGroups[2].name
    });

    await saveButton.click();

    testVehicle.vehicleGroupId = testNewVehicle.vehicleGroupId;

    expect(vehicleService.createVehicle)
      .toHaveBeenCalledWith(testVehicle);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(true);
  });
});
