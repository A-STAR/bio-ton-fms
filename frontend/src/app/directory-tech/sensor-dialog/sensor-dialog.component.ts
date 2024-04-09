import { ChangeDetectionStrategy, Component, ElementRef, ErrorHandler, Inject, OnInit, QueryList, ViewChildren } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatExpansionModule, MatExpansionPanel } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Observable, Subscription, forkJoin, map, share, tap } from 'rxjs';

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
    MatExpansionModule,
    MatButtonModule,
    NumberOnlyInputDirective
  ],
  templateUrl: './sensor-dialog.component.html',
  styleUrls: ['./sensor-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SensorDialogComponent implements OnInit {
  @ViewChildren(MatExpansionPanel) private settingsPanels!: QueryList<MatExpansionPanel>;

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
      const settingsGroupPaths = ['general', 'refueling', 'drain'];

      settingsGroupPaths.forEach((path, index) => {
        const settingsGroup = this.sensorForm.get(`basic.${path}`);

        if (settingsGroup!.invalid) {
          this.settingsPanels
            .get(index)!
            .open();
        }
      });

      return;
    }

    const {
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
      description,
      general,
      refueling,
      drain
    } = value.basic!;

    const { startTimeout, fixErrors, fuelUseCalculation, fuelUseTimeCalculation } = general!;

    const newSensor: NewSensor = {
      trackerId: this.data.trackerID ?? this.data.sensor!.tracker.id,
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
      description: description ?? undefined,
      startTimeout: startTimeout ?? undefined,
      fixErrors: fixErrors ?? undefined,
      fuelUseCalculation: fuelUseCalculation ?? undefined,
      fuelUseTimeCalculation: fuelUseTimeCalculation ?? undefined,
      minRefueling: refueling?.min ?? undefined,
      refuelingTimeout: refueling?.timeout ?? undefined,
      fullRefuelingTimeout: refueling?.fullTimeout ?? undefined,
      refuelingLookup: refueling?.lookup ?? undefined,
      refuelingCalculation: refueling?.calculation ?? undefined,
      refuelingRawCalculation: refueling?.rawCalculation ?? undefined,
      minDrain: drain?.min ?? undefined,
      drainTimeout: drain?.timeout ?? undefined,
      drainStopTimeout: drain?.stopTimeout ?? undefined,
      drainLookup: drain?.lookup ?? undefined,
      drainCalculation: drain?.calculation ?? undefined,
      drainRawCalculation: drain?.rawCalculation ?? undefined
    };

    let sensor$: Observable<Sensor | null>;

    if (this.data.sensor && 'id' in this.data.sensor) {
      newSensor.id = this.data.sensor.id;

      sensor$ = this.sensorService.updateSensor(newSensor);
    } else {
      sensor$ = this.sensorService.createSensor(newSensor);
    }

    this.#subscription = sensor$.subscribe({
      next: (response: Sensor | null) => {
        const message = this.data.sensor && 'id' in this.data.sensor ? SENSOR_UPDATED : SENSOR_CREATED;

        this.snackBar.open(message);

        let sensor: Sensor;

        if (this.data.sensor && 'id' in this.data.sensor) {
          sensor = this.#serializeSensor(newSensor);
        } else {
          // TODO: remove local settings
          sensor = {
            ...response!,
            startTimeout: startTimeout ?? undefined,
            fixErrors: fixErrors ?? undefined,
            fuelUseCalculation: fuelUseCalculation ?? undefined,
            fuelUseTimeCalculation: fuelUseTimeCalculation ?? undefined,
            minRefueling: refueling?.min ?? undefined,
            refuelingTimeout: refueling?.timeout ?? undefined,
            fullRefuelingTimeout: refueling?.fullTimeout ?? undefined,
            refuelingLookup: refueling?.lookup ?? undefined,
            refuelingCalculation: refueling?.calculation ?? undefined,
            refuelingRawCalculation: refueling?.rawCalculation ?? undefined,
            minDrain: drain?.min ?? undefined,
            drainTimeout: drain?.timeout ?? undefined,
            drainStopTimeout: drain?.stopTimeout ?? undefined,
            drainLookup: drain?.lookup ?? undefined,
            drainCalculation: drain?.calculation ?? undefined,
            drainRawCalculation: drain?.rawCalculation ?? undefined
          };
        }

        this.dialogRef.close(sensor);
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

                  break;

                case 'Formula': {
                  path = 'basic.formula';

                  break;
                }
              }

              if (path) {
                const errors: ValidationErrors = {
                  serverErrors: {
                    messages: value.join('\n')
                  }
                };

                this.sensorForm
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

          this.sensorForm.setErrors(errors);

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
    const { id, trackerId, sensorTypeId, unitId, validatorId, ...rest } = newSensor;

    const type = this.#sensorGroups
      .flatMap(({ sensorTypes = [] }) => sensorTypes)
      .find(({ id }) => id === sensorTypeId)!;

    const unit = this.#units.find(({ id }) => id === unitId)!;

    const sensor: Sensor = {
      id: id!,
      tracker: this.data.sensor!.tracker,
      sensorType: {
        id: sensorTypeId,
        value: type.name
      },
      unit: {
        id: unitId,
        value: unit.name
      },
      ...rest
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
   * Map `Sensor | Omit<Sensor, 'id'>` to `NewSensor`.
   *
   * @param sensor `Sensor | Omit<Sensor, 'id'>` Sensor.
   *
   * @returns `NewSensor` sensor.
   */
  #deserializeSensor(sensor: Sensor | Omit<Sensor, 'id'>): NewSensor {
    const { tracker, sensorType, unit, validator, ...rest } = sensor;

    const newSensor: NewSensor = {
      trackerId: tracker.id,
      sensorTypeId: sensorType.id,
      unitId: unit.id,
      validatorId: validator?.id,
      ...rest
    };

    return newSensor;
  }

  /**
   * Initialize Sensor form.
   */
  #initSensorForm() {
    let sensor: NewSensor | undefined;

    if (this.data.sensor) {
      sensor = this.#deserializeSensor(this.data.sensor!);
    }

    this.sensorForm = this.fb.group({
      basic: this.fb.group({
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
            Validators.pattern(FUEL_PATTERN)
          ]
        ),
        description: this.fb.nonNullable.control(
          sensor?.description,
          Validators.maxLength(500)
        ),
        general: this.fb.group({
          startTimeout: this.fb.nonNullable.control(sensor?.startTimeout, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER)
          ]),
          fixErrors: this.fb.nonNullable.control(sensor?.fixErrors),
          fuelUseCalculation: this.fb.nonNullable.control(sensor?.fuelUseCalculation),
          fuelUseTimeCalculation: this.fb.nonNullable.control(sensor?.fuelUseTimeCalculation)
        }),
        refueling: this.fb.group({
          min: this.fb.nonNullable.control(sensor?.minRefueling, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER),
            Validators.pattern(FUEL_PATTERN)
          ]),
          timeout: this.fb.nonNullable.control(sensor?.refuelingTimeout, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER)
          ]),
          fullTimeout: this.fb.nonNullable.control(sensor?.fullRefuelingTimeout, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER)
          ]),
          lookup: this.fb.nonNullable.control(sensor?.refuelingLookup),
          calculation: this.fb.nonNullable.control(sensor?.refuelingCalculation),
          rawCalculation: this.fb.nonNullable.control(sensor?.refuelingRawCalculation)
        }),
        drain: this.fb.group({
          min: this.fb.nonNullable.control(sensor?.minDrain, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER),
            Validators.pattern(FUEL_PATTERN)
          ]),
          timeout: this.fb.nonNullable.control(sensor?.drainTimeout, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER)
          ]),
          stopTimeout: this.fb.nonNullable.control(sensor?.drainStopTimeout, [
            Validators.min(0),
            Validators.max(Number.MAX_SAFE_INTEGER)
          ]),
          lookup: this.fb.nonNullable.control(sensor?.drainLookup),
          calculation: this.fb.nonNullable.control(sensor?.drainCalculation),
          rawCalculation: this.fb.nonNullable.control(sensor?.drainRawCalculation)
        })
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
    private errorHandler: ErrorHandler,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) protected data: SensorDialogData,
    private elementRef: ElementRef,
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
  trackerID?: Tracker['id'];
  sensor?: Sensor | Omit<Sensor, 'id'>;
  sensors: Sensor[];
};

type SensorForm = FormGroup<{
  basic: FormGroup<{
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
    general: FormGroup<{
      startTimeout: FormControl<NewSensor['startTimeout'] | undefined>;
      fixErrors: FormControl<NewSensor['fixErrors'] | undefined>;
      fuelUseCalculation: FormControl<NewSensor['fuelUseCalculation'] | undefined>;
      fuelUseTimeCalculation: FormControl<NewSensor['fuelUseTimeCalculation'] | undefined>;
    }>;
    refueling: FormGroup<{
      min: FormControl<NewSensor['minRefueling'] | undefined>;
      timeout: FormControl<NewSensor['refuelingTimeout'] | undefined>;
      fullTimeout: FormControl<NewSensor['fullRefuelingTimeout'] | undefined>;
      lookup: FormControl<NewSensor['refuelingLookup'] | undefined>;
      calculation: FormControl<NewSensor['refuelingCalculation'] | undefined>;
      rawCalculation: FormControl<NewSensor['refuelingRawCalculation'] | undefined>;
    }>;
    drain: FormGroup<{
      min: FormControl<NewSensor['minDrain'] | undefined>;
      timeout: FormControl<NewSensor['drainTimeout'] | undefined>;
      stopTimeout: FormControl<NewSensor['drainStopTimeout'] | undefined>;
      lookup: FormControl<NewSensor['drainLookup'] | undefined>;
      calculation: FormControl<NewSensor['drainCalculation'] | undefined>;
      rawCalculation: FormControl<NewSensor['drainRawCalculation'] | undefined>;
    }>;
  }>;
}>;

const FUEL_PATTERN = /^\d+(?:\.\d{1,2})?$/;

export const SENSOR_CREATED = 'Датчик создан';
export const SENSOR_UPDATED = 'Датчик обновлён';
