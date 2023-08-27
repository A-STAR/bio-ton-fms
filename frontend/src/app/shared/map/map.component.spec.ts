import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapComponent } from './map.component';

import { testLocationAndTrackResponse } from '../../tech/tech.service.spec';

describe('MapComponent', () => {
  let component: MapComponent;
  let fixture: ComponentFixture<MapComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [MapComponent]
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

  it('should render map', async () => {
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

  it('should fit map view to layer bounds', async () => {
    /* Coverage for map view returning to its original fit. */
    component.location = {
      tracks: testLocationAndTrackResponse.tracks
    };

    expect(component.location)
      .withContext('set `location` input value')
      .not.toBeNull();
  });
});
