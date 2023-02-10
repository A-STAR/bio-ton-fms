import { ComponentFixture, TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { Observable, of } from 'rxjs';

import { SensorGroup, SensorService, SensorType, Unit } from '../sensor.service';

import { SensorDialogComponent } from './sensor-dialog.component';

import { testSensorDataTypeEnum, testSensorGroups, testSensorTypes, testUnits, testValidationTypeEnum } from '../sensor.service.spec';

describe('SensorDialogComponent', () => {
  let component: SensorDialogComponent;
  let fixture: ComponentFixture<SensorDialogComponent>;

  let sensorGroupsSpy: jasmine.Spy<(this: SensorService) => Observable<SensorGroup[]>>;
  let sensorTypesSpy: jasmine.Spy<(this: SensorService) => Observable<SensorType[]>>;
  let unitsSpy: jasmine.Spy<(this: SensorService) => Observable<Unit[]>>;
  let sensorDataTypeSpy: jasmine.Spy<(this: SensorService) => Observable<KeyValue<string, string>[]>>;
  let validationTypeSpy: jasmine.Spy<(this: SensorService) => Observable<KeyValue<string, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          SensorDialogComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(SensorDialogComponent);

    const sensorService = TestBed.inject(SensorService);

    component = fixture.componentInstance;

    const sensorGroups$ = of(testSensorGroups);
    const sensorTypes$ = of(testSensorTypes);
    const units$ = of(testUnits);
    const sensorDataType$ = of(testSensorDataTypeEnum);
    const validationType$ = of(testValidationTypeEnum);

    sensorGroupsSpy = spyOnProperty(sensorService, 'sensorGroups$')
      .and.returnValue(sensorGroups$);

    sensorTypesSpy = spyOnProperty(sensorService, 'sensorTypes$')
      .and.returnValue(sensorTypes$);

    unitsSpy = spyOnProperty(sensorService, 'units$')
      .and.returnValue(units$);

    sensorDataTypeSpy = spyOnProperty(sensorService, 'sensorDataType$')
      .and.returnValue(sensorDataType$);

    validationTypeSpy = spyOnProperty(sensorService, 'validationType$')
      .and.returnValue(validationType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get sensor groups, sensor types, units', () => {
    expect(sensorGroupsSpy)
      .toHaveBeenCalled();

    expect(sensorTypesSpy)
      .toHaveBeenCalled();

    expect(unitsSpy)
      .toHaveBeenCalled();
  });

  it('should get sensor data type, validation enums', () => {
    expect(sensorDataTypeSpy)
      .toHaveBeenCalled();

    expect(validationTypeSpy)
      .toHaveBeenCalled();
  });
});
