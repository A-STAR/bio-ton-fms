import { ErrorHandler } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogContent, MatDialogClose } from '@angular/material/dialog';
import { MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of, throwError } from 'rxjs';

import { Fuel, NewVehicle, Vehicle, VehicleGroup, VehicleService, VehicleSubtype, VehicleType } from '../vehicle.service';

import { NumberOnlyInputDirective } from '../../shared/number-only-input/number-only-input.directive';
import { VehicleDialogComponent, VEHICLE_CREATED, VEHICLE_UPDATED } from './vehicle-dialog.component';

import { environment } from '../../../environments/environment';
import { testFuels, testNewVehicle, testVehicleGroups, testVehicleSubtypeEnum, testVehicleTypeEnum } from '../vehicle.service.spec';

describe('VehicleDialogComponent', () => {
  let component: VehicleDialogComponent;
  let fixture: ComponentFixture<VehicleDialogComponent>;
  let documentRootLoader: HarnessLoader;
  let loader: HarnessLoader;
  let vehicleService: VehicleService;

  const dialogRef = jasmine.createSpyObj<MatDialogRef<VehicleDialogComponent, true | '' | undefined>>('MatDialogRef', ['close']);

  let vehicleGroupsSpy: jasmine.Spy<(this: VehicleService) => Observable<VehicleGroup[]>>;
  let fuelsSpy: jasmine.Spy<(this: VehicleService) => Observable<Fuel[]>>;
  let vehicleTypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<VehicleType, string>[]>>;
  let vehicleSubtypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<VehicleSubtype, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          HttpClientTestingModule,
          MatSnackBarModule,
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
    vehicleService = TestBed.inject(VehicleService);
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    loader = TestbedHarnessEnvironment.loader(fixture);

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
    const titleDe = fixture.debugElement.query(By.css('[mat-dialog-title]'));

    expect(titleDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('Добавление технического средства');

    component['data'] = testNewVehicle;

    fixture.detectChanges();
    await fixture.whenStable();

    expect(titleDe.nativeElement.textContent)
      .withContext('render dialog update title text')
      .toBe('Сводная информация о техническом средстве');
  });

  it('should render vehicle form', async () => {
    const dialogContentDe = fixture.debugElement.query(
      By.directive(MatDialogContent)
    );

    expect(dialogContentDe)
      .withContext('render dialog content element')
      .not.toBeNull();

    const vehicleFormDe = dialogContentDe.query(
      By.css('form#vehicle-form')
    );

    expect(vehicleFormDe)
      .withContext('render vehicle form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Наименование машины'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Производитель'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Модель'
      })
    );

    const yearInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Год производства'
      })
    );

    await expectAsync(
      yearInput.getType()
    )
      .withContext('render year input type')
      .toBeResolvedTo('number');

    const [yearInputDe, trackerInputDe] = fixture.debugElement.queryAll(
      By.directive(NumberOnlyInputDirective)
    );

    expect(yearInputDe.nativeElement.placeholder)
      .withContext('render year input with `NumberOnlyDirective`')
      .toBe('Год производства');

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Группа машин"]'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Тип машины"]'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Подтип машины'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Тип топлива"]'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Регистрационный номер'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Инвентарный номер'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Серийный номер кузова'
      })
    );

    const trackerInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'GPS-трекер'
      })
    );

    await expectAsync(
      trackerInput.getType()
    )
      .withContext('render tracker input type')
      .toBeResolvedTo('number');

    expect(trackerInputDe.nativeElement.placeholder)
      .withContext('render tracker input with `NumberOnlyDirective`')
      .toBe('GPS-трекер');

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Описание'
      })
    );
  });

  it('should render dialog actions', async () => {
    const cancelButton = await loader.getHarnessOrNull(
      MatButtonHarness.with({
        text: 'Отмена',
        variant: 'stroked'
      })
    );

    expect(cancelButton)
      .withContext('render close button')
      .not.toBeNull();

    const dialogCloseDe = fixture.debugElement.query(
      By.directive(MatDialogClose)
    );

    expect(dialogCloseDe)
      .withContext('render dialog close element')
      .not.toBeNull();

    loader.getHarnessOrNull(
      MatButtonHarness.with({
        text: 'Отправить',
        variant: 'flat'
      })
    );
  });

  it('should render update vehicle form', async () => {
    component['data'] = testNewVehicle;

    component.ngOnInit();

    fixture.detectChanges();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Наименование машины',
        value: testNewVehicle.name
      }));

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Производитель',
        value: testNewVehicle.make
      }));

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'Модель',
        value: testNewVehicle.model
      }));

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Тип машины"]'
      })
    );

    await expectAsync(
      typeSelect.getValueText()
    )
      .withContext('render type select text')
      .toBeResolvedTo(testVehicleTypeEnum[0].value);

    const subtypeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Подтип машины"]'
      })
    );

    await expectAsync(
      subtypeSelect.getValueText()
    )
      .withContext('render subtype select text')
      .toBeResolvedTo(testVehicleSubtypeEnum[2].value);

    const fuelSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="Тип топлива"]'
      })
    );

    await expectAsync(
      fuelSelect.getValueText()
    )
      .withContext('render fuel select text')
      .toBeResolvedTo(testFuels[0].name);
  });

  it('should submit invalid vehicle form', async () => {
    spyOn(vehicleService, 'createVehicle')
      .and.callThrough();

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="vehicle-form"]',
        text: 'Сохранить'
      })
    );

    await saveButton.click();

    expect(vehicleService.createVehicle)
      .not.toHaveBeenCalled();
  });

  it('should submit create vehicle form', async () => {
    const [nameInput, makeInput, modelInput] = await loader.getAllHarnesses(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form'
      })
    );

    const [groupSelect, typeSelect, subtypeSelect, fuelSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form'
      })
    );

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

    const newVehicle: NewVehicle = {
      id: undefined,
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

    const testVehicleResponse: Vehicle = {
      id: testNewVehicle.id!,
      name: testNewVehicle.name,
      make: testNewVehicle.make,
      model: testNewVehicle.model,
      manufacturingYear: testNewVehicle.manufacturingYear,
      type: testVehicleTypeEnum[0],
      subType: testVehicleSubtypeEnum[2],
      fuelType: {
        id: testNewVehicle.fuelTypeId,
        value: testFuels[0].name
      }
    };

    const createVehicleSpy = spyOn(vehicleService, 'createVehicle')
      .and.callFake(() => of(testVehicleResponse));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="vehicle-form"]'
      })
    );

    await saveButton.click();

    expect(vehicleService.createVehicle)
      .toHaveBeenCalledWith(newVehicle);

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(VEHICLE_CREATED);

    await expectAsync(
      snackBar.hasAction()
    )
      .toBeResolvedTo(false);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(true);

    /* Coverage for selected vehicle `group`. */

    await groupSelect.clickOptions({
      text: testVehicleGroups[2].name
    });

    await saveButton.click();

    const url = `${environment.api}/api/telematica/vehicle`;

    const nameErrorText = `Машина с именем ${testVehicleResponse.name} уже существует`;
    const manufacturingYearText = `Год производства должен быть больше 0`;

    let testErrorResponse = new HttpErrorResponse({
      error: {
        errors: {
          Name: [nameErrorText],
          ManufacturingYear: [manufacturingYearText]
        }
      },
      status: 400,
      statusText: 'Bad Request',
      url
    });

    const errorHandler = TestBed.inject(ErrorHandler);

    spyOn(console, 'error');
    const handleErrorSpy = spyOn(errorHandler, 'handleError');

    createVehicleSpy.and.callFake(() => throwError(() => testErrorResponse));

    // test for the vehicle request `400 Bad Request` error response
    await saveButton.click();

    const [nameFormField] = await loader.getAllHarnesses(
      MatFormFieldHarness.with({
        ancestor: 'form#vehicle-form'
      })
    );

    const [nameError] = await nameFormField.getErrors();

    await expectAsync(
      nameError.getText()
    )
      .withContext('render name field error text')
      .toBeResolvedTo(nameErrorText);

    expect(handleErrorSpy)
      .toHaveBeenCalledWith(testErrorResponse);

    handleErrorSpy.calls.reset();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    testErrorResponse = new HttpErrorResponse({
      error: {
        messages: [
          `Имя машины ${testVehicleResponse.name} зарезервировано`,
          `Инвентарный номер машины ${testVehicleResponse.name} зарезервирован`
        ]
      },
      status: 409,
      statusText: 'Conflict',
      url
    });

    createVehicleSpy.and.callFake(() => throwError(() => testErrorResponse));

    // test for the vehicle request error response
    await saveButton.click();

    let formErrorDes = fixture.debugElement.queryAll(
      By.css('form > mat-error')
    );

    expect(formErrorDes.length)
      .withContext('render form error elements')
      .toBe(testErrorResponse.error.messages.length);

    formErrorDes.forEach((formErrorDe, index) => {
      expect(formErrorDe.nativeElement.textContent)
        .withContext('render form error element text')
        .toBe(testErrorResponse.error.messages[index]);
    });

    expect(handleErrorSpy)
      .not.toHaveBeenCalled();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    /* Coverage for a common server error fallback. */

    testErrorResponse = new HttpErrorResponse({
      error: `Имя машины ${testVehicleResponse.name} зарезервировано`,
      status: 409,
      statusText: 'Conflict',
      url
    });

    createVehicleSpy.and.callFake(() => throwError(() => testErrorResponse));

    await saveButton.click();

    formErrorDes = fixture.debugElement.queryAll(
      By.css('form > mat-error')
    );

    expect(formErrorDes.length)
      .withContext('render form error element')
      .toBe(1);

    expect(formErrorDes[0].nativeElement.textContent)
      .withContext('render form error element text')
      .toBe(testErrorResponse.error);

    expect(handleErrorSpy)
      .not.toHaveBeenCalled();

    // reset form invalid state
    await nameInput.setValue('');
    await nameInput.setValue(name);

    handleErrorSpy.calls.reset();

    /* Coverage for authorization error response */

    testErrorResponse = new HttpErrorResponse({
      status: 403,
      statusText: 'Forbidden',
      url
    });

    createVehicleSpy.and.callFake(() => throwError(() => testErrorResponse));

    await saveButton.click();

    formErrorDes = fixture.debugElement.queryAll(
      By.css('form > mat-error')
    );

    expect(formErrorDes.length)
      .withContext('render no form error element')
      .toBe(0);

    expect(handleErrorSpy)
      .toHaveBeenCalledWith(testErrorResponse);
  });

  it('should submit update vehicle form', async () => {
    component['data'] = testNewVehicle;

    component.ngOnInit();

    const [
      nameInput,
      ,
      modelInput,
      yearInput,
      registrationInput,
      ,
      ,
      ,
      descriptionInput
    ] = await loader.getAllHarnesses(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form'
      })
    );

    const updatedModel = 'Hilux';
    const updatedName = `${testNewVehicle.make} ${updatedModel}`;

    await nameInput.setValue(updatedName);
    await modelInput.setValue(updatedModel);

    const updatedYear = 2020;

    const year = updatedYear?.toString();

    await yearInput.setValue(year);

    const [, , , fuelSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form'
      })
    );

    await fuelSelect.clickOptions({
      text: testFuels[1].name
    });

    const updatedRegistrationNumber = '8888TH88';

    await registrationInput.setValue(updatedRegistrationNumber);

    const updatedDescription = 'Патруль';

    await descriptionInput.setValue(updatedDescription);

    spyOn(vehicleService, 'updateVehicle')
      .and.callFake(() => of(null));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="vehicle-form"]'
      })
    );

    await saveButton.click();

    expect(vehicleService.updateVehicle)
      .toHaveBeenCalledWith({
        ...testNewVehicle,
        name: updatedName,
        model: updatedModel,
        manufacturingYear: updatedYear,
        fuelTypeId: testFuels[1].id,
        registrationNumber: updatedRegistrationNumber,
        description: updatedDescription
      });

    const snackBar = await documentRootLoader.getHarness(MatSnackBarHarness);

    await expectAsync(
      snackBar.getMessage()
    )
      .toBeResolvedTo(VEHICLE_UPDATED);

    expect(dialogRef.close)
      .toHaveBeenCalledWith(true);
  });
});
