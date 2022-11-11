import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { forkJoin, map, Observable } from 'rxjs';

import { Fuel, VehicleGroup, VehicleService } from '../vehicle.service';

@Component({
  selector: 'bio-vehicle-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './vehicle-dialog.component.html',
  styleUrls: ['./vehicle-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehicleDialogComponent implements OnInit {
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
        name: this.fb.nonNullable.control<string | undefined>(undefined),
        make: this.fb.nonNullable.control<string | undefined>(undefined),
        model: this.fb.nonNullable.control<string | undefined>(undefined),
        year: this.fb.nonNullable.control<number | undefined>(undefined),
        group: this.fb.nonNullable.control<number | undefined>(undefined),
        type: this.fb.nonNullable.control<string | undefined>(undefined),
        subtype: this.fb.nonNullable.control<string | undefined>(undefined),
        fuel: this.fb.nonNullable.control<number | undefined>(undefined)
      }),
      registration: this.fb.group({
        registration: this.fb.nonNullable.control<string | undefined>(undefined),
        inventory: this.fb.nonNullable.control<string | undefined>(undefined),
        serial: this.fb.nonNullable.control<string | undefined>(undefined),
        tracker: this.fb.nonNullable.control<number | undefined>(undefined)
      }),
      additional: this.fb.group({
        description: this.fb.nonNullable.control<string | undefined>(undefined)
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

  constructor(private fb: FormBuilder, private vehicleService: VehicleService) { }

  ngOnInit() {
    this.#setVehicleData();
    this.#initVehicleForm();
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
