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
  async #addFullscreenControl() {
    const fullscreenControl = new maplibregl.FullscreenControl();

    await this.#map.addControl(fullscreenControl, CONTROL_POSITION);
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

    await this.#addFullscreenControl();
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

    await this.#map.addLayer<NgwLayerAdapterType, NgwLayerOptions>(fieldLayerAdapter, {
      visibility: false,
      paint: {
        color: '#2E653C',
        strokeColor: '#FFFFFF',
        opacity: 1
      }
    });

    // put away visible vector edges
    this.#map.unSelectLayer('fields', () => true);

    await this.#map.showLayer('fields');

    await this.#map.fitLayer('fields', {
      duration: FIT_DURATION,
      padding: FIT_PADDING
    });
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

const FIT_DURATION = 670;
const FIT_PADDING = 60;