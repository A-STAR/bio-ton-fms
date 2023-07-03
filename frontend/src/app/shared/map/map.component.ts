import { ChangeDetectionStrategy, Component, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { createWebMap, MainLayerAdapter, WebMap } from '@nextgis/webmap';
import MapAdapter from '@nextgis/mapboxgl-map-adapter';
import maplibregl from 'maplibre-gl';
import { LngLatArray } from '@nextgis/utils';

@Component({
  selector: 'bio-map',
  standalone: true,
  imports: [CommonModule],
  template: '',
  styleUrls: ['./map.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapComponent implements OnInit {
  #map!: WebMap<MapAdapter, MainLayerAdapter>;

  /**
   * Add map fullscreen control.
   */
  #addFullscreenControl() {
    const fullscreenControl = new maplibregl.FullscreenControl();

    this.#map.addControl(fullscreenControl, CONTROL_POSITION);
  }

  /**
   * Initialize map, add base layer, fullscreen control.
   */
  async #initMap() {
    this.#map = await createWebMap({
      mapAdapter: new MapAdapter(),
      target: this.elementRef.nativeElement,
      center: DEFAULT_POSITION,
      zoom: DEFAULT_ZOOM,
      controls: ['ZOOM'],
      controlsOptions: {
        ZOOM: {
          position: CONTROL_POSITION
        }
      }
    });

    await this.#map.addBaseLayer('OSM');

    this.#addFullscreenControl();
  }

  constructor(private elementRef: ElementRef) {}

  // eslint-disable-next-line @typescript-eslint/member-ordering
  async ngOnInit() {
    await this.#initMap();
  }
}

const DEFAULT_POSITION: LngLatArray = [50.13, 53.17];
const DEFAULT_ZOOM = 10;

const CONTROL_POSITION = 'top-right';
