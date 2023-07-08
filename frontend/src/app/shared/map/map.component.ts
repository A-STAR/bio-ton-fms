import { ChangeDetectionStrategy, Component, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { createWebMap, MainLayerAdapter, WebMap } from '@nextgis/webmap';
import MapAdapter from '@nextgis/mapboxgl-map-adapter';
import maplibregl from 'maplibre-gl';
import { LngLatArray } from '@nextgis/utils';
import { createQmsAdapter } from '@nextgis/qms-kit';
import { createNgwLayerAdapter, NgwLayerAdapterType, NgwLayerOptions } from '@nextgis/ngw-kit';
import NgwConnector from '@nextgis/ngw-connector';

import { environment } from '../../../environments/environment';

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

    const qmsAdapter = createQmsAdapter({
      qmsId: 448,
      webMap: this.#map
    });

    await this.#map.addBaseLayer(qmsAdapter);

    this.#addFullscreenControl();
  }

  /**
   * Add map layer with fields.
   */
  async #addFieldLayer() {
    const connector = new NgwConnector({
      baseUrl: environment.nextgis,
      auth: {
        login: AUTH_LOGIN,
        password: AUTH_PASSWORD
      }
    });

    const fieldLayerAdapter = createNgwLayerAdapter({
      resource: FIELD_RESOURCE,
      id: 'fields'
    }, this.#map, connector);

    await this.#map.addLayer<NgwLayerAdapterType, NgwLayerOptions>(fieldLayerAdapter);
  }

  constructor(private elementRef: ElementRef) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  async ngOnInit() {
    await this.#initMap();
    await this.#addFieldLayer();
  }
}

const DEFAULT_POSITION: LngLatArray = [50.13, 53.17];
const DEFAULT_ZOOM = 10;

const CONTROL_POSITION = 'top-right';

const AUTH_LOGIN = 'a.zubkova@bioton-agro.ru';
const AUTH_PASSWORD = 'asdfghjkl13';

const FIELD_RESOURCE = 109;
