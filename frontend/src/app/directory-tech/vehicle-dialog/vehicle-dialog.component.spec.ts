import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatSnackBarHarness } from '@angular/material/snack-bar/testing';

import { Observable, of } from 'rxjs';

import { Fuel, NewVehicle, VehicleGroup, VehicleService, VehicleSubtype, VehicleType } from '../vehicle.service';

import { NumberOnlyInputDirective } from 'src/app/shared/number-only-input/number-only-input.directive';
import { VehicleDialogComponent, VEHICLE_CREATED, VEHICLE_UPDATED } from './vehicle-dialog.component';

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
    documentRootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
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
      .toBe('???????????????????? ???????????????????????? ????????????????');

    component['data'] = testNewVehicle;

    fixture.detectChanges();

    expect(titleTextDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('?????????????? ???????????????????? ?? ?????????????????????? ????????????????');
  });

  it('should render vehicle form', async () => {
    const vehicleFormDe = fixture.debugElement.query(By.css('form#vehicle-form'));

    expect(vehicleFormDe)
      .withContext('render Vehicle form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '???????????????????????? ????????????'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '??????????????????????????'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '????????????'
      })
    );

    const yearInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '?????? ????????????????????????'
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
      .toBe('?????? ????????????????????????');

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="???????????? ??????????"]'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="?????? ????????????"]'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="???????????? ????????????'
      })
    );

    loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="?????? ??????????????"]'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '?????????????????????????????? ??????????'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '?????????????????????? ??????????'
      })
    );

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '???????????????? ?????????? ????????????'
      })
    );

    const trackerInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: 'GPS-????????????'
      })
    );

    await expectAsync(
      trackerInput.getType()
    )
      .withContext('render tracker input type')
      .toBeResolvedTo('number');

    expect(trackerInputDe.nativeElement.placeholder)
      .withContext('render tracker input with `NumberOnlyDirective`')
      .toBe('GPS-????????????');

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '????????????????'
      })
    );
  });

  it('should render update vehicle form', async () => {
    component['data'] = testNewVehicle;

    component.ngOnInit();

    fixture.detectChanges();

    const vehicleFormDe = fixture.debugElement.query(By.css('form#vehicle-form'));

    expect(vehicleFormDe)
      .withContext('render Vehicle form element')
      .not.toBeNull();

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '???????????????????????? ????????????',

        value: testNewVehicle.name
      }));

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '??????????????????????????',

        value: testNewVehicle.make
      }));

    loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#vehicle-form',
        placeholder: '????????????',

        value: testNewVehicle.model
      }));

    const typeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form',
        selector: '[placeholder="?????? ????????????"]'
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
        selector: '[placeholder="???????????? ????????????"]'
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
        selector: '[placeholder="?????? ??????????????"]'
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
        text: '??????????????????'
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

    spyOn(vehicleService, 'createVehicle')
      .and.callFake(() => of({}));

    const saveButton = await loader.getHarness(
      MatButtonHarness.with({
        selector: '[form="vehicle-form"]'
      })
    );

    await saveButton.click();

    const testVehicle: NewVehicle = {
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

    expect(vehicleService.createVehicle)
      .toHaveBeenCalledWith(testVehicle);

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

    const [, , , fuelSelect] = await loader.getAllHarnesses(
      MatSelectHarness.with({
        ancestor: 'form#vehicle-form'
      })
    );

    await fuelSelect.clickOptions({
      text: testFuels[1].name
    });

    const updatedModel = 'Hilux';
    const updatedName = `${testNewVehicle.make} ${updatedModel}`;

    await nameInput.setValue(updatedName);
    await modelInput.setValue(updatedModel);

    const updatedYear = 2020;

    const year = updatedYear?.toString();

    await yearInput.setValue(year);

    const updatedRegistrationNumber = '8888TH88';

    await registrationInput.setValue(updatedRegistrationNumber);

    const updatedDescription = '??????????????';

    await descriptionInput.setValue(updatedDescription);

    spyOn(vehicleService, 'updateVehicle')
      .and.callFake(() => of({}));

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
