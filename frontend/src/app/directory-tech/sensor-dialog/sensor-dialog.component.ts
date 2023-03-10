import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

import { forkJoin, map, Observable, Subscription } from 'rxjs';

import { NewSensor, Sensor, SensorGroup, SensorService, SensorType, Unit } from '../sensor.service';

import { NumberOnlyInputDirective } from '../../shared/number-only-input/number-only-input.directive';

import { Tracker } from '../tracker.service';

@Component({
  selector: 'bio-sensor-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule,
    NumberOnlyInputDirective
  ],
  templateUrl: './sensor-dialog.component.html',
  styleUrls: ['./sensor-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SensorDialogComponent implements OnInit {
  protected get hiddenFuelUse() {
    const type = this.sensorForm.get('basic.type')?.value;

    return !(type && this.fuelUseTypeIds.includes(type));
  }

  protected sensorData$!: Observable<{
    groups: SensorGroup[];
    validators: SensorType[];
    units: Unit[];
    dataType: KeyValue<string, string>[];
    validation: KeyValue<string, string>[];
  }>;

  protected sensorForm!: FormGroup<SensorForm>;
  protected fuelUseTypeIds: SensorGroup['id'][] = [3, 13, 14];

  /**
   * Submit sensor form, checking validation state.
   */
  protected submitSensorForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.sensorForm;

    if (invalid) {
      return;
    }

    const {
      tracker,
      name,
      type,
      dataType,
      formula,
      unit,
      validator,
      validationType,
      lastReceived,
      visibility,
      fuelUse,
      description
    } = value.basic!;

    const sensor: NewSensor = {
      trackerId: tracker!,
      name: name!,
      sensorTypeId: type!,
      dataType: dataType!,
      formula: formula!,
      unitId: unit!,
      validatorId: validator ?? undefined,
      validationType: validationType!,
      useLastReceived: lastReceived ?? false,
      visibility: visibility ?? false,
      fuelUse: fuelUse ?? undefined,
      description: description ?? undefined
    };

    this.#subscription = this.sensorService
      .createSensor(sensor)
      .subscribe((sensor: Sensor) => {
        this.snackBar.open(SENSOR_CREATED);

        this.dialogRef.close(sensor);
      });
  }

  /**
   * Toggle control disabled state on selection change conditionally.
   *
   * @param event `MatSelectionChange` event.
   * @param path Control path.
   */
  protected onControlSelectionChange({ value }: MatSelectChange, path: string | readonly (string | number)[]) {
    const control = this.sensorForm.get(path);

    (path === 'basic.validationType' && value === undefined) || (path === 'basic.fuelUse' && this.hiddenFuelUse)
      ? control?.disable()
      : control?.enable();
  }

  #subscription: Subscription | undefined;

  /**
   * Get sensor groups, sensor types, units. Set sensor data.
   */
  #setSensorData() {
    this.sensorData$ = forkJoin([
      this.sensorService.sensorGroups$,
      this.sensorService.sensorTypes$,
      this.sensorService.units$,
      this.sensorService.sensorDataType$,
      this.sensorService.validationType$
    ])
      .pipe(
        map(([groups, validators, units, dataType, validation]) => ({ groups, validators, units, dataType, validation }))
      );
  }

  /**
   * Initialize Sensor form.
   */
  #initSensorForm() {
    this.sensorForm = this.fb.group({
      basic: this.fb.group({
        tracker: this.fb.nonNullable.control<NewSensor['trackerId'] | undefined>(this.data.trackerId!, Validators.required),
        name: this.fb.nonNullable.control<NewSensor['name'] | undefined>(undefined, [
          Validators.required,
          Validators.maxLength(100)
        ]),
        type: this.fb.nonNullable.control<NewSensor['sensorTypeId'] | undefined>(undefined, Validators.required),
        dataType: this.fb.nonNullable.control<NewSensor['dataType'] | undefined>(undefined, Validators.required),
        formula: this.fb.nonNullable.control<NewSensor['formula'] | undefined>(undefined, Validators.required),
        unit: this.fb.nonNullable.control<NewSensor['unitId'] | undefined>(undefined, Validators.required),
        validator: this.fb.nonNullable.control<NewSensor['validatorId'] | undefined>(undefined),
        validationType: this.fb.nonNullable.control<NewSensor['validationType'] | undefined>({
          value: undefined,
          disabled: true
        }, Validators.required),
        lastReceived: this.fb.nonNullable.control<NewSensor['useLastReceived'] | undefined>(undefined),
        visibility: this.fb.nonNullable.control<NewSensor['visibility'] | undefined>(undefined),
        fuelUse: this.fb.nonNullable.control<NewSensor['fuelUse'] | undefined>(
          {
            value: undefined,
            disabled: true
          },
          Validators.pattern(FUEL_USE_PATTERN)
        ),
        description: this.fb.nonNullable.control<NewSensor['description'] | undefined>(
          undefined,
          Validators.maxLength(500)
        )
      })
    });
  }

  constructor(
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) protected data: SensorDialogData,
    private dialogRef: MatDialogRef<SensorDialogComponent, Sensor>,
    private snackBar: MatSnackBar,
    private sensorService: SensorService
  ) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#setSensorData();
    this.#initSensorForm();
  }
}

export type SensorDialogData = Partial<{
  trackerId: Tracker['id'];
}>

type SensorForm = {
  basic: FormGroup<{
    tracker: FormControl<NewSensor['trackerId'] | undefined>;
    name: FormControl<NewSensor['name'] | undefined>;
    type: FormControl<NewSensor['sensorTypeId'] | undefined>;
    dataType: FormControl<NewSensor['dataType'] | undefined>;
    formula: FormControl<NewSensor['formula'] | undefined>;
    unit: FormControl<NewSensor['unitId'] | undefined>;
    validator: FormControl<NewSensor['validatorId'] | undefined>;
    validationType: FormControl<NewSensor['validationType'] | undefined>;
    lastReceived: FormControl<NewSensor['useLastReceived'] | undefined>;
    visibility: FormControl<NewSensor['visibility'] | undefined>;
    fuelUse: FormControl<NewSensor['fuelUse'] | undefined>;
    description: FormControl<NewSensor['description'] | undefined>;
  }>;
}

export const SENSOR_CREATED = 'Сенсор создан';
const FUEL_USE_PATTERN = /^\d+(?:\.\d{1,2})?$/;
