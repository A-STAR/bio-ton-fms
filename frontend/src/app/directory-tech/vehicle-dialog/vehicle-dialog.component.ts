import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

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
  /**
   * Submit Vehicle form, checking validation state.
   */
  async submitVehicleForm() {
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
      name: name!,
      type: type!,
      trackerId: tracker,
      vehicleGroupId: group!,
      make: make!,
      model: model!,
      subType: subtype!,
      fuelTypeId: fuel!,
      manufacturingYear: year!,
      registrationNumber: registration!,
      inventoryNumber: inventory!,
      serialNumber: serial!,
      description: description!
    };

    const addVehicle$ = this.vehicleService.createVehicle(vehicle);

    await firstValueFrom(addVehicle$);

    this.dialogRef.close(true);
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
   * Initialize Vehicle form.
   */
  #initVehicleForm() {
    this.vehicleForm = this.fb.group({
      basic: this.fb.group({
        name: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        make: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        model: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        year: this.fb.nonNullable.control<number | undefined>(undefined, [
          Validators.required,
          Validators.pattern(YEAR_PATTERN)
        ]),
        group: this.fb.nonNullable.control<number | undefined>(undefined, Validators.required),
        type: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        subtype: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        fuel: this.fb.nonNullable.control<number | undefined>(undefined, Validators.required)
      }),
      registration: this.fb.group({
        registration: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        inventory: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        serial: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
        tracker: this.fb.nonNullable.control<number | undefined>(undefined)
      }),
      additional: this.fb.group({
        description: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required)
      })
    });
  }

  protected vehicleData$!: Observable<{
    groups: VehicleGroup[];
    fuels: Fuel[];
    type: KeyValue<string, string>[];
    subtype: KeyValue<string, string>[];
  }>;

  protected vehicleForm!: FormGroup<VehicleForm>;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<VehicleDialogComponent, true | ''>,
    private vehicleService: VehicleService
  ) { }

  ngOnInit() {
    this.#setVehicleData();
    this.#initVehicleForm();
  }

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
