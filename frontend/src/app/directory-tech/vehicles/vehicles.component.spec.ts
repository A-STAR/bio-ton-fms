import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { Observable, of } from 'rxjs';

import { Vehicles, VehicleService } from '../vehicle.service';

import { VehiclesComponent } from './vehicles.component';
import { testVehicles } from '../vehicle.service.spec';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;

  let vehiclesSpy: jasmine.Spy<() => Observable<Vehicles>>;

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

    vehiclesSpy = spyOn(vehicleService, 'getVehicles')
      .and.returnValue(vehicles$);

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
});
