import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { createWebMap, FeatureLayerAdapter, FitOptions, MainLayerAdapter, WebMap } from '@nextgis/webmap';
import MapAdapter from '@nextgis/mapboxgl-map-adapter';
import maplibregl from 'maplibre-gl';
import { LngLatArray, LngLatBoundsArray } from '@nextgis/utils';
import { createQmsAdapter } from '@nextgis/qms-kit';
import { createNgwLayerAdapter, NgwLayerAdapterType, NgwLayerOptions } from '@nextgis/ngw-kit';
import NgwConnector, { ResourceDefinition } from '@nextgis/ngw-connector';
import { Feature, FeatureCollection, Point } from 'geojson';
import { getIcon } from '@nextgis/icons';

import { LocationAndTrackResponse } from '../../tech/tech.service';
import { MonitoringTech } from '../../tech/tech.component';

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
      this.#location = location;

      this.#setLocationLayer(location);
      this.#fitView(location);
    }
  }

  @Input() set point(id: MonitoringTech['id'] | undefined) {
    this.#point = id;

    if (id) {
      this.#pointMap();
    }
  }

  #map!: WebMap<MapAdapter, MainLayerAdapter>;
  #location?: LocationAndTrackResponse;
  #point?: MonitoringTech['id'];

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
   * Add, update map layer with location markers.
   *
   * @param location Location and tracks.
   */
  async #setLocationLayer({ tracks }: LocationAndTrackResponse) {
    const features: Feature<Point>[] = [];

    for (const {
      vehicleId: id,
      latitude,
      longitude
    } of tracks) {
      const marker: Feature<Point> = {
        type: 'Feature',
        geometry: {
          type: 'Point',
          coordinates: [longitude, latitude]
        },
        properties: { id }
      };

      features.push(marker);
    }

    const markers: FeatureCollection<Point> = {
      type: 'FeatureCollection',
      features
    };

    let locationLayer = this.#map?.getLayer<FeatureLayerAdapter>(LOCATION_LAYER_DEFINITION);

    if (!locationLayer) {
      locationLayer = await this.#map?.addFeatureLayer({
        id: LOCATION_LAYER_DEFINITION,
        paint: getIcon({
          svg: TECH_SVG_XML,
          color: '#8FAB93',
          stroke: 10,
          size: 60
        })
      });
    }

    await locationLayer?.clearLayer?.( /* istanbul ignore next */ _ => true);
    await locationLayer?.addData?.(markers);
  }

  /**
   * Set map view center to the point.
   */
  #pointMap() {
    const track = this.#location?.tracks.find(({ vehicleId }) => vehicleId === this.#point);

    if (track) {
      const coordinates: LngLatArray = [track.longitude, track.latitude];

      this.#map?.setCenter(coordinates);
    }
  }

  /**
   * Set a map view to the given geographical bounds or fit by default field layer bounds.
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

      if (this.#point) {
        this.#pointMap();
      }
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

const LOCATION_LAYER_DEFINITION = 'location';

const INITIAL_DURATION = 500;
const duration = 2500;
const padding = 60;

const FIT_OPTIONS: FitOptions = { duration, padding };

const TECH_SVG_SIZE_FACTOR = 5;

// eslint-disable-next-line max-len
const TECH_SVG_XML = `<svg viewBox="0 0 ${TECH_SVG_SIZE_FACTOR * 752} ${TECH_SVG_SIZE_FACTOR * 752}"><path d="M332 187.6c-2.5.2-10.8.8-18.5 1.4-26.8 2.1-55.3 7.1-64.4 11.3-5.5 2.6-15.4 11.7-18.7 17.2-1.3 2.3-7.6 18.2-13.9 35.3-6.3 17.1-12.1 32.6-12.9 34.6l-1.5 3.6h-17.4c-21.2 0-24.8.9-32.4 8.5-11 11.1-11 28.5-.1 39.2 5.7 5.5 11.9 8.2 21.3 9 6.4.5 7.6.9 7.1 2.2-.2.9-2.5 7.4-5 14.6-7.7 22.1-7.7 21.5-7.4 102.1l.3 70.9 3.2 6.6c3.9 7.9 10.2 14 18.1 17.7 5 2.4 7 2.7 15.7 2.7s10.7-.3 15.7-2.7c7.9-3.7 14.2-9.8 18.1-17.6 3.1-6.4 3.2-7.1 3.5-20.9l.3-14.3h265.8l.3 14.3c.3 13.8.4 14.5 3.5 20.9 3.9 7.8 10.2 13.9 18.1 17.6 5 2.4 7 2.7 15.7 2.7s10.7-.3 15.7-2.7c7.9-3.7 14.2-9.8 18.1-17.7l3.2-6.6.3-70.9c.3-80.6.3-80-7.4-102.1-2.5-7.2-4.8-13.7-5-14.6-.5-1.3.7-1.7 7.1-2.2 10-.9 16.5-3.8 21.9-9.8 10.4-11.6 10.1-27.5-.7-38.4-7.6-7.6-11.2-8.5-32.4-8.5h-17.4l-1.5-3.6c-.8-2-6.6-17.5-12.9-34.6-6.3-17.1-12.6-32.9-13.9-35.2-3.5-5.8-12.2-14.1-18.1-16.9-8.5-4.2-29.9-8.3-58.5-11.3-11.3-1.2-28-1.7-62-1.9-25.6-.1-48.5-.1-51 .1zm166.6 87.6c7.5 22.7 13.9 41.9 14.2 42.5.3 1-27.5 1.3-136.8 1.3-109.3 0-137.1-.3-136.8-1.3.3-.6 6.7-19.8 14.2-42.5l13.8-41.2h217.6l13.8 41.2zm-256 102.3c14.5 4.3 23 21.3 18 36.2-5.2 15.6-24.6 23.6-39.2 16-10-5.1-15.3-13.9-15.4-25.2 0-19.5 17.8-32.6 36.6-27zm284.1 0c6.5 1.9 13.9 8.5 16.9 15 3 6.4 3.3 17 .5 23.1-2.7 5.9-7.7 11.2-13.5 14.2-4.2 2.1-6.5 2.6-12.6 2.6-6.4 0-8.3-.4-12.9-2.9-10.7-5.9-15.7-15.2-14.9-27.6 1.3-18.2 18.6-29.7 36.5-24.4z"/></svg>`;
