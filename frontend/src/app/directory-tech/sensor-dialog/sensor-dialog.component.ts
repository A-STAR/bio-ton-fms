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

import { forkJoin, map, Observable, share, Subscription, tap } from 'rxjs';

import { NewSensor, Sensor, SensorGroup, SensorService, Unit } from '../sensor.service';

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
    dataType: KeyValue<string, string>[];
    units: Unit[];
    validation: KeyValue<string, string>[];
  }>;

  protected title!: string;
  protected sensorForm!: SensorForm;
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

    const newSensor: NewSensor = {
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

    let sensor$: Observable<Sensor | null>;

    if (this.data.sensor && 'id' in this.data.sensor) {
      newSensor.id = this.data.sensor.id;

      sensor$ = this.sensorService.updateSensor(newSensor);
    } else {
      sensor$ = this.sensorService.createSensor(newSensor);
    }

    this.#subscription = sensor$.subscribe((response: Sensor | null) => {
      const message = this.data.sensor && 'id' in this.data.sensor ? SENSOR_UPDATED : SENSOR_CREATED;

      this.snackBar.open(message);

      let sensor: Sensor;

      if (this.data.sensor && 'id' in this.data.sensor) {
        sensor = this.#serializeSensor(newSensor);
      } else {
        sensor = response!;
      }

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

  #sensorGroups!: SensorGroup[];
  #units!: Unit[];
  #subscription: Subscription | undefined;

  /**
   * Map `NewSensor` to `Sensor`.
   *
   * @param newSensor `NewSensor` sensor.
   *
   * @returns `Sensor` sensor.
   */
  #serializeSensor(newSensor: NewSensor) {
    const {
      name,
      sensorTypeId,
      dataType,
      formula,
      unitId,
      validatorId,
      validationType,
      useLastReceived,
      visibility,
      fuelUse,
      description
    } = newSensor;

    const type = this.#sensorGroups
      .flatMap(({ sensorTypes = [] }) => sensorTypes)
      .find(({ id }) => id === sensorTypeId)!;

    const unit = this.#units.find(({ id }) => id === unitId)!;

    const sensor: Sensor = {
      id: newSensor.id!,
      tracker: this.data.sensor!.tracker,
      name,
      sensorType: {
        id: sensorTypeId,
        value: type.name,
      },
      dataType,
      formula,
      unit: {
        id: unitId,
        value: unit.name
      },
      validationType,
      useLastReceived,
      visibility,
      fuelUse,
      description
    };

    if (validatorId) {
      const validator = this.data.sensors.find(({ id }) => id === validatorId)!;

      sensor.validator = {
        id: validatorId,
        value: validator.name
      };
    }

    return sensor;
  }

  /**
   * Map `Sensor` or `Omit<Sensor, 'id'>` to `NewSensor`.
   */
  #deserializeSensor(sensor: Sensor | Omit<Sensor, 'id'>): NewSensor {
    const {
      tracker,
      name,
      sensorType,
      dataType,
      formula,
      unit,
      validator,
      validationType,
      useLastReceived,
      visibility,
      fuelUse,
      description
    } = sensor;

    const newSensor: NewSensor = {
      trackerId: tracker.id,
      name,
      sensorTypeId: sensorType.id,
      dataType,
      formula,
      unitId: unit.id,
      validatorId: validator?.id,
      validationType,
      useLastReceived,
      visibility,
      fuelUse,
      description
    };

    if ('id' in sensor) {
      newSensor.id = sensor.id;
    }

    return newSensor;
  }

  /**
   * Initialize Sensor form.
   */
  #initSensorForm() {
    let trackerID: Tracker['id'];
    let sensor: NewSensor | undefined;

    if (this.data.trackerID) {
      trackerID = this.data.trackerID;
    } else {
      trackerID = this.data.sensor!.tracker.id;

      sensor = this.#deserializeSensor(this.data.sensor!);
    }

    this.sensorForm = this.fb.group({
      basic: this.fb.group({
        tracker: this.fb.nonNullable.control(trackerID, Validators.required),
        name: this.fb.nonNullable.control(sensor?.name, [
          Validators.required,
          Validators.maxLength(100)
        ]),
        type: this.fb.nonNullable.control(sensor?.sensorTypeId, Validators.required),
        dataType: this.fb.nonNullable.control(sensor?.dataType, Validators.required),
        formula: this.fb.nonNullable.control(sensor?.formula, Validators.required),
        unit: this.fb.nonNullable.control(sensor?.unitId, Validators.required),
        validator: this.fb.nonNullable.control(sensor?.validatorId),
        validationType: this.fb.nonNullable.control({
          value: sensor?.validationType,
          disabled: sensor?.validatorId === undefined
        }, Validators.required),
        lastReceived: this.fb.nonNullable.control(sensor?.useLastReceived),
        visibility: this.fb.nonNullable.control(sensor?.visibility),
        fuelUse: this.fb.nonNullable.control(
          {
            value: sensor?.fuelUse,
            disabled: !(sensor?.sensorTypeId && this.fuelUseTypeIds.includes(sensor.sensorTypeId))
          },
          [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER),
            Validators.pattern(FUEL_USE_PATTERN)
          ]
        ),
        description: this.fb.nonNullable.control(
          sensor?.description,
          Validators.maxLength(500)
        )
      })
    });
  }

  /**
   * Get sensor groups, data type, units, sensor types, validation type. Set sensor data.
   */
  #setSensorData() {
    this.sensorData$ = forkJoin([
      this.sensorService.sensorGroups$,
      this.sensorService.sensorDataType$,
      this.sensorService.units$,
      this.sensorService.validationType$
    ])
      .pipe(
        tap(([groups, , units]) => {
          this.#sensorGroups = groups;
          this.#units = units;
        }),
        map(([groups, dataType, units, validation]) => ({ groups, dataType, units, validation })),
        share()
      );
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
    this.title = this.data.sensor && 'id' in this.data.sensor ? 'Сводная информация о датчике' : 'Новый датчик';

    this.#setSensorData();
    this.#initSensorForm();
  }
}

export type SensorDialogData = {
  trackerID?: Tracker['id']
  sensor?: Sensor | Omit<Sensor, 'id'>;
  sensors: Sensor[];
};

type SensorForm = FormGroup<{
  basic: FormGroup<{
    tracker: FormControl<NewSensor['trackerId']>;
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
}>

const FUEL_USE_PATTERN = /^\d+(?:\.\d{1,2})?$/;

export const SENSOR_CREATED = 'Датчик создан';
export const SENSOR_UPDATED = 'Датчик обновлён';
