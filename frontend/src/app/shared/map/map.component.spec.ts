import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { MapComponent } from './map.component';

import { testLocationAndTrackResponse, testMonitoringVehicles } from '../../tech/tech.service.spec';

describe('MapComponent', () => {
  let component: MapComponent;
  let fixture: ComponentFixture<MapComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          MapComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(MapComponent);

    component = fixture.componentInstance;
    component.location = testLocationAndTrackResponse;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render map', () => {
    expect(fixture.debugElement.classes)
      .withContext('render component map class')
      .toEqual(
        jasmine.objectContaining({
          'maplibregl-map': true
        })
      );

    const mapCanvasEl = fixture.debugElement.nativeElement.querySelector('.maplibregl-canvas-container');

    expect(mapCanvasEl)
      .withContext('render map canvas element')
      .not.toBeNull();

    const mapControlsEl = fixture.debugElement.nativeElement.querySelector('.maplibregl-control-container');

    expect(mapControlsEl)
      .withContext('render map controls element')
      .not.toBeNull();
  });

  it('should fit map view to layer bounds', () => {
    /* Coverage for map view returning to its original fit. */
    component.location = {
      tracks: []
    };

    expect(component.location)
      .withContext('set `location` input value')
      .not.toBeNull();

    /* Coverage for map view fitting to layer bounds. */
    component.location = testLocationAndTrackResponse;
  });

  it('should point map view', () => {
    /* Coverage for map view setting to the point. */
    component.point = testMonitoringVehicles[0].id;

    expect(component.point)
      .withContext('set `point` input value')
      .not.toBeNull();

    /* Coverage for map view returning to the point after updating location bounds. */
    component.location = testLocationAndTrackResponse;
  });
});
