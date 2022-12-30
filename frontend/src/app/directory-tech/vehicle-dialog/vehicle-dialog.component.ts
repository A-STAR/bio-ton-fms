import { ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatLegacySnackBar as MatSnackBar } from '@angular/material/legacy-snack-bar';

import { firstValueFrom, forkJoin, map, Observable, Subscription } from 'rxjs';

import { Fuel, NewVehicle, VehicleGroup, VehicleService } from '../vehicle.service';

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
    MatButtonModule
  ],
  templateUrl: './vehicle-dialog.component.html',
  styleUrls: ['./vehicle-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehicleDialogComponent implements OnInit, OnDestroy {
  protected vehicleData$!: Observable<{
    groups: VehicleGroup[];
    fuels: Fuel[];
    type: KeyValue<string, string>[];
    subtype: KeyValue<string, string>[];
  }>;

  protected vehicleForm!: FormGroup<VehicleForm>;

  /**
   * Submit Vehicle form, checking validation state.
   */
  protected async submitVehicleForm() {
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

    const vehicle$ = this.data
      ? this.vehicleService.updateVehicle(vehicle)
      : this.vehicleService.createVehicle(vehicle);

    await firstValueFrom(vehicle$);

    const message = this.data ? VEHICLE_UPDATED : VEHICLE_CREATED;

    this.snackBar.open(message);
    this.dialogRef.close(true);
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  #subscription: Subscription | undefined;

  /**
   * Get groups, fuels, type, subtype. Set vehicle data.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
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
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #initVehicleForm() {
    this.vehicleForm = this.fb.group({
      basic: this.fb.group({
        name: this.fb.nonNullable.control<string | undefined>(this.data?.name, Validators.required),
        make: this.fb.nonNullable.control<string | undefined>(this.data?.make, Validators.required),
        model: this.fb.nonNullable.control<string | undefined>(this.data?.model, Validators.required),
        year: this.fb.nonNullable.control<number | undefined>(this.data?.manufacturingYear, Validators.pattern(YEAR_PATTERN)),
        group: this.fb.nonNullable.control<number | undefined>(this.data?.vehicleGroupId),
        type: this.fb.nonNullable.control<string | undefined>(this.data?.type, Validators.required),
        subtype: this.fb.nonNullable.control<string | undefined>(this.data?.subType, Validators.required),
        fuel: this.fb.nonNullable.control<number | undefined>(this.data?.fuelTypeId, Validators.required)
      }),
      registration: this.fb.group({
        registration: this.fb.nonNullable.control<string | undefined>(
          this.data?.registrationNumber,
          Validators.pattern(REGISTRATION_NUMBER_PATTERN)
        ),
        inventory: this.fb.nonNullable.control<string | undefined>(this.data?.inventoryNumber),
        serial: this.fb.nonNullable.control<string | undefined>(this.data?.serialNumber),
        tracker: this.fb.nonNullable.control<number | undefined>(this.data?.trackerId)
      }),
      additional: this.fb.group({
        description: this.fb.nonNullable.control<string | undefined>(this.data?.description)
      })
    });
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) protected data: NewVehicle | undefined,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<VehicleDialogComponent, true | ''>,
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

type VehicleForm = {
  basic: FormGroup<{
    name: FormControl<string | undefined>;
    make: FormControl<string | undefined>;
    model: FormControl<string | undefined>;
    year: FormControl<number | undefined>;
    group: FormControl<number | undefined>;
    type: FormControl<string | undefined>;
    subtype: FormControl<string | undefined>;
    fuel: FormControl<number | undefined>;
  }>;
  registration: FormGroup<{
    registration: FormControl<string | undefined>;
    inventory: FormControl<string | undefined>;
    serial: FormControl<string | undefined>;
    tracker: FormControl<number | undefined>;
  }>;
  additional: FormGroup<{
    description: FormControl<string | undefined>;
  }>;
}

const YEAR_PATTERN = /\d{4}/;
const REGISTRATION_NUMBER_PATTERN = /^[a-zA-Zа-яА-ЯёЁ0-9]+$/;

export const VEHICLE_CREATED = 'Машина создана';
export const VEHICLE_UPDATED = 'Машина обновлена';
