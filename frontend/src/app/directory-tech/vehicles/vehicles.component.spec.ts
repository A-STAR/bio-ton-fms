import { ComponentFixture, TestBed } from '@angular/core/testing';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { Observable, of } from 'rxjs';

import { Fuel, VehicleGroup, Vehicles, VehicleService } from '../vehicle.service';

import { VehiclesComponent } from './vehicles.component';

import { testFuels, testVehicleGroups, testVehicles } from '../vehicle.service.spec';

import { testVehicleSubTypeEnum, testVehicleTypeEnum } from '../vehicle.service.spec';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;

  let vehiclesSpy: jasmine.Spy<() => Observable<Vehicles>>;
  let vehicleGroupsSpy: jasmine.Spy<(this: VehicleService) => Observable<VehicleGroup[]>>;
  let fuelsSpy: jasmine.Spy<(this: VehicleService) => Observable<Fuel[]>>;
  let vehicleTypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<string, string>[]>>;
  let vehicleSubTypeSpy: jasmine.Spy<(this: VehicleService) => Observable<KeyValue<string, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          VehiclesComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(VehiclesComponent);

    const vehicleService = TestBed.inject(VehicleService);

    component = fixture.componentInstance;

    const vehicles$ = of(testVehicles);
    const vehicleGroups$ = of(testVehicleGroups);
    const fuels$ = of(testFuels);
    const vehicleType$ = of(testVehicleTypeEnum);
    const vehicleSubType$ = of(testVehicleSubTypeEnum);

    vehiclesSpy = spyOn(vehicleService, 'getVehicles')
      .and.returnValue(vehicles$);

    vehicleGroupsSpy = spyOnProperty(vehicleService, 'vehicleGroups$')
      .and.returnValue(vehicleGroups$);

    fuelsSpy = spyOnProperty(vehicleService, 'fuels$')
      .and.returnValue(fuels$);

    vehicleTypeSpy = spyOnProperty(vehicleService, 'vehicleType$')
      .and.returnValue(vehicleType$);

    vehicleSubTypeSpy = spyOnProperty(vehicleService, 'vehicleSubType$')
      .and.returnValue(vehicleSubType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get vehicles', () => {
    expect(vehiclesSpy)
      .toHaveBeenCalled();
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

    expect(vehicleSubTypeSpy)
      .toHaveBeenCalled();
  });
});
