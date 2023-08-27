import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { createWebMap, FitOptions, MainLayerAdapter, WebMap } from '@nextgis/webmap';
import MapAdapter from '@nextgis/mapboxgl-map-adapter';
import maplibregl from 'maplibre-gl';
import { LngLatArray, LngLatBoundsArray } from '@nextgis/utils';
import { createQmsAdapter } from '@nextgis/qms-kit';
import { createNgwLayerAdapter, NgwLayerAdapterType, NgwLayerOptions } from '@nextgis/ngw-kit';
import NgwConnector, { ResourceDefinition } from '@nextgis/ngw-connector';

import { LocationAndTrackResponse } from '../../tech/tech.service';

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
  @Input() set location(location: LocationAndTrackResponse | null) {
    if (location) {
      this.#fitView(location);
    }
  }

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
      resource: FIELD_RESOURCE_DEFINITION,
      id: FIELD_LAYER_DEFINITION
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
    this.#map.unSelectLayer(FIELD_LAYER_DEFINITION, () => true);

    await this.#map.showLayer(FIELD_LAYER_DEFINITION);

    await this.#map.fitLayer(FIELD_LAYER_DEFINITION, {
      ...FIT_OPTIONS,
      duration: INITIAL_DURATION
    });
  }

  /**
   * Set a map view that contains the given geographical bounds
   * or fit by default field layer bounds.
   *
   * @param location Location and tracks.
   */
  #fitView({ viewBounds }: LocationAndTrackResponse) {
    if (viewBounds) {
      const {
        upperLeftLatitude: north,
        upperLeftLongitude: west,
        bottomRightLongitude: east,
        bottomRightLatitude: south
      } = viewBounds;

      const bounds: LngLatBoundsArray = [west, south, east, north];

      this.#map?.fitBounds(bounds, FIT_OPTIONS);
    } else {
      this.#map?.fitLayer(FIELD_LAYER_DEFINITION, FIT_OPTIONS);
    }
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

const FIELD_RESOURCE_DEFINITION: ResourceDefinition = 109;
const FIELD_LAYER_DEFINITION = 'fields';

const INITIAL_DURATION = 500;
const duration = 2500;
const padding = 60;

const FIT_OPTIONS: FitOptions = { duration, padding };
