import { ChangeDetectionStrategy, Component, Inject, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { forkJoin, map, Observable } from 'rxjs';

import { NewSensor, SensorGroup, SensorService, SensorType, Unit } from '../sensor.service';

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
    NumberOnlyInputDirective
  ],
  templateUrl: './sensor-dialog.component.html',
  styleUrls: ['./sensor-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SensorDialogComponent implements OnInit {
  protected sensorData$!: Observable<{
    groups: SensorGroup[];
    validators: SensorType[];
    units: Unit[];
    dataType: KeyValue<string, string>[];
    validation: KeyValue<string, string>[];
  }>;

  protected sensorForm!: FormGroup<SensorForm>;

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
        tracker: this.fb.nonNullable.control<NewSensor['trackerId'] | undefined>(this.data.trackerId!),
        name: this.fb.nonNullable.control<NewSensor['name'] | undefined>(undefined),
        type: this.fb.nonNullable.control<NewSensor['sensorTypeId'] | undefined>(undefined),
        dataType: this.fb.nonNullable.control<NewSensor['dataType'] | undefined>(undefined),
        formula: this.fb.nonNullable.control<NewSensor['formula'] | undefined>(undefined),
        unit: this.fb.nonNullable.control<NewSensor['unitId'] | undefined>(undefined),
        validator: this.fb.nonNullable.control<NewSensor['validatorId'] | undefined>(undefined),
        validationType: this.fb.nonNullable.control<NewSensor['validationType'] | undefined>(undefined),
        lastReceived: this.fb.nonNullable.control<NewSensor['useLastReceived'] | undefined>(undefined),
        visibility: this.fb.nonNullable.control<NewSensor['visibility'] | undefined>(undefined),
        fuelUse: this.fb.nonNullable.control<NewSensor['fuelUse'] | undefined>(undefined),
        description: this.fb.nonNullable.control<NewSensor['description'] | undefined>(undefined)
      })
    });
  }

  constructor(private fb: FormBuilder, @Inject(MAT_DIALOG_DATA) protected data: SensorDialogData, private sensorService: SensorService) {}

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
