import { ChangeDetectionStrategy, Component, ElementRef, ErrorHandler, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

import { forkJoin, map, Observable, Subscription } from 'rxjs';

import { Fuel, NewVehicle, Vehicle, VehicleGroup, VehicleService, VehicleSubtype, VehicleType } from '../vehicle.service';

import { NumberOnlyInputDirective } from '../../../app/shared/number-only-input/number-only-input.directive';

@Component({
  selector: 'bio-vehicle-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    NumberOnlyInputDirective
  ],
  templateUrl: './vehicle-dialog.component.html',
  styleUrls: ['./vehicle-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehicleDialogComponent implements OnInit, OnDestroy {
  protected vehicleData$!: Observable<{
    groups: VehicleGroup[];
    fuels: Fuel[];
    type: KeyValue<VehicleType, string>[];
    subtype: KeyValue<VehicleSubtype, string>[];
  }>;

  protected vehicleForm!: VehicleForm;
  protected maxManufacturingYear = maxManufacturingYear;

  /**
   * Submit Vehicle form, checking validation state.
   */
  protected submitVehicleForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.vehicleForm;

    if (invalid) {
      return;
    }

    const {
      basic,
      registration: registrationGroup,
      additional
    } = value;

    const { name, make, model, year, type, subtype, group, fuel } = basic!;
    const { registration, inventory, serial, tracker } = registrationGroup!;
    const { description } = additional!;

    const vehicle: NewVehicle = {
      id: this.data?.id,
      name: name!,
      make: make!,
      model: model!,
      manufacturingYear: year ?? undefined,
      vehicleGroupId: group ? Number(group) : undefined,
      type: type!,
      subType: subtype!,
      fuelTypeId: Number(fuel!),
      registrationNumber: registration ?? undefined,
      inventoryNumber: inventory ?? undefined,
      serialNumber: serial ?? undefined,
      trackerId: tracker ?? undefined,
      description: description ?? undefined
    };

    const vehicle$: Observable<Vehicle | null> = this.data
      ? this.vehicleService.updateVehicle(vehicle)
      : this.vehicleService.createVehicle(vehicle);

    this.#subscription = vehicle$.subscribe({
      next: () => {
        const message = this.data ? VEHICLE_UPDATED : VEHICLE_CREATED;

        this.snackBar.open(message);

        this.dialogRef.close(true);
      },
      error: (error: HttpErrorResponse) => {
        const isClientError = error.status
          .toString()
          .startsWith('4');

        const isBadRequestError = error.status === 400;
        const isAuthError = isClientError && [401, 403].includes(error.status);
        const isFormError = isClientError && !isAuthError;

        if (isBadRequestError) {
          Object
            .entries<string[]>(error.error.errors)
            .forEach(([key, value]) => {
              let path: string | undefined;

              switch (key) {
                case 'Name':
                  path = 'basic.name';
              }

              if (path) {
                const errors: ValidationErrors = {
                  serverErrors: {
                    messages: value.join('\n')
                  }
                };

                this.vehicleForm
                  .get(path)
                  ?.setErrors(errors);

                delete error.error.errors[key];
              }
            });

          const hasErrors = Object
            .keys(error.error.errors)
            .length;

          if (hasErrors) {
            // print remaining unprocessed errors
            this.errorHandler.handleError(error);
          }
        } else if (isFormError) {
          const errors: ValidationErrors = {
            serverErrors: {
              messages: error.error?.messages ?? [error.error]
            }
          };

          this.vehicleForm.setErrors(errors);

          setTimeout(() => {
            this.elementRef.nativeElement
              .querySelector('form > mat-error')
              .scrollIntoView({
                behavior: 'smooth'
              });
          });
        } else {
          this.errorHandler.handleError(error);
        }
      }
    });
  }

  #subscription: Subscription | undefined;

  /**
   * Get groups, fuels, type, subtype. Set vehicle data.
   */
  #setVehicleData() {
    this.vehicleData$ = forkJoin([
      this.vehicleService.vehicleGroups$,
      this.vehicleService.fuels$,
      this.vehicleService.vehicleType$,
      this.vehicleService.vehicleSubtype$
    ])
      .pipe(
        map(([groups, fuels, type, subtype]) => ({ groups, fuels, type, subtype }))
      );
  }

  /**
   * Initialize vehicle form.
   */
  #initVehicleForm() {
    this.vehicleForm = this.fb.group({
      basic: this.fb.group({
        name: this.fb.nonNullable.control(this.data?.name, [
          Validators.required,
          Validators.maxLength(100)
        ]),
        make: this.fb.nonNullable.control(this.data?.make, [
          Validators.required,
          Validators.maxLength(30)
        ]),
        model: this.fb.nonNullable.control(this.data?.model, [
          Validators.required,
          Validators.maxLength(30)
        ]),
        year: this.fb.nonNullable.control(this.data?.manufacturingYear, [
          Validators.max(maxManufacturingYear),
          Validators.pattern(YEAR_PATTERN)
        ]),
        group: this.fb.nonNullable.control(this.data?.vehicleGroupId),
        type: this.fb.nonNullable.control(this.data?.type, Validators.required),
        subtype: this.fb.nonNullable.control(this.data?.subType, Validators.required),
        fuel: this.fb.nonNullable.control(this.data?.fuelTypeId, Validators.required)
      }),
      registration: this.fb.group({
        registration: this.fb.nonNullable.control(this.data?.registrationNumber, [
          Validators.maxLength(15),
          Validators.pattern(REGISTRATION_NUMBER_PATTERN)
        ]
        ),
        inventory: this.fb.nonNullable.control(
          this.data?.inventoryNumber,
          Validators.maxLength(30)
        ),
        serial: this.fb.nonNullable.control(
          this.data?.serialNumber,
          Validators.maxLength(40)
        ),
        tracker: this.fb.nonNullable.control(this.data?.trackerId)
      }),
      additional: this.fb.group({
        description: this.fb.nonNullable.control(
          this.data?.description,
          Validators.maxLength(500)
        )
      })
    });
  }

  constructor(
    private errorHandler: ErrorHandler,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) protected data: NewVehicle | undefined,
    private elementRef: ElementRef,
    private dialogRef: MatDialogRef<VehicleDialogComponent, true | '' | undefined>,
    private snackBar: MatSnackBar,
    private vehicleService: VehicleService
  ) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setVehicleData();
    this.#initVehicleForm();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

type VehicleForm = FormGroup<{
  basic: FormGroup<{
    name: FormControl<NewVehicle['name'] | undefined>;
    make: FormControl<NewVehicle['make'] | undefined>;
    model: FormControl<NewVehicle['model'] | undefined>;
    year: FormControl<NewVehicle['manufacturingYear']>;
    group: FormControl<NewVehicle['vehicleGroupId']>;
    type: FormControl<NewVehicle['type'] | undefined>;
    subtype: FormControl<NewVehicle['subType'] | undefined>;
    fuel: FormControl<NewVehicle['fuelTypeId'] | undefined>;
  }>;
  registration: FormGroup<{
    registration: FormControl<NewVehicle['registrationNumber']>;
    inventory: FormControl<NewVehicle['inventoryNumber']>;
    serial: FormControl<NewVehicle['serialNumber']>;
    tracker: FormControl<NewVehicle['trackerId']>;
  }>;
  additional: FormGroup<{
    description: FormControl<NewVehicle['description'] | undefined>;
  }>;
}>;

const maxManufacturingYear = new Date()
  .getFullYear();

const YEAR_PATTERN = /\d{4}/;
const REGISTRATION_NUMBER_PATTERN = /^[a-zA-Zа-яА-ЯёЁ0-9]+$/;

export const VEHICLE_CREATED = 'Машина создана';
export const VEHICLE_UPDATED = 'Машина обновлена';
