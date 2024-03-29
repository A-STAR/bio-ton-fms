import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit } from '@angular/core';
import { CommonModule, formatDate, formatNumber } from '@angular/common';

import { CreatePopupContentProps, FeatureLayerAdapter, FitOptions, MainLayerAdapter, WebMap, createWebMap } from '@nextgis/webmap';
import MapAdapter from '@nextgis/mapboxgl-map-adapter';
import maplibregl from 'maplibre-gl';
import { FeatureProperties, LngLatArray, LngLatBoundsArray } from '@nextgis/utils';
import { createQmsAdapter } from '@nextgis/qms-kit';
import { NgwLayerAdapterType, NgwLayerOptions, createNgwLayerAdapter } from '@nextgis/ngw-kit';
import NgwConnector, { ResourceDefinition } from '@nextgis/ngw-connector';
import { Feature, FeatureCollection, LineString, Point, Position } from 'geojson';
import { getIcon } from '@nextgis/icons';

import { firstValueFrom, map, tap } from 'rxjs';

import { LocationAndTrackResponse, TechService } from '../../tech/tech.service';
import { RelativeTimePipe, localeID } from '../../tech/shared/relative-time.pipe';

import { MonitoringTech } from '../../tech/tech.component';

import { environment } from '../../../environments/environment';

@Component({
  selector: 'bio-map',
  standalone: true,
  imports: [CommonModule],
  providers: [RelativeTimePipe],
  template: '',
  styleUrls: ['./map.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapComponent implements OnInit {
  @Input() set location(location: LocationAndTrackResponse | null) {
    if (location) {
      this.#location = location;

      this.#closeMessagePopup(location);
      this.#setLocationLayers(location);
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
  #messagePopupTechID?: MonitoringTech['id'];

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

  /* istanbul ignore next */
  /**
   * Get a message, create a message popup content.
   *
   * @param properties Popup content properties.
   *
   * @returns Promise of popup content inner HTML.
   */
  #createMessagePopupContent = ({
    feature: { properties }
  }: CreatePopupContentProps<Feature<Point, FeatureProperties>>) => {
    const popup$ = this.techService
      .getMessage(properties['id'])
      .pipe(
        tap(() => {
          this.#messagePopupTechID = properties['techID'];
        }),
        map(({ generalInfo, trackerInfo }) => {
          const divEl = document.createElement('div');

          const headingEl = document.createElement('h1');

          headingEl.textContent = properties['techName'];

          const DEFAULT_DETAILS = '-';

          const descriptionListEl = document.createElement('dl');

          for (const property in generalInfo) {
            if (property === 'longitude') {
              continue;
            }

            const divEl = document.createElement('div');

            const descriptionTermEl = document.createElement('dt');

            const TERMS = {
              messageTime: 'Последняя точка:',
              numberOfSatellites: 'Спутники',
              speed: 'Скорость',
              latitude: 'Координаты'
            };

            const term = TERMS[property as keyof typeof TERMS];

            descriptionTermEl.textContent = term;

            divEl.appendChild(descriptionTermEl);

            const descriptionDetailsEls: HTMLElement[] = [];

            switch (property) {
              case 'messageTime': {
                // relative time
                let descriptionDetailsEl = document.createElement('dd');

                descriptionDetailsEl.textContent = this.relativeTimePipe.transform(generalInfo[property]);

                descriptionDetailsEls.push(descriptionDetailsEl);

                // date
                descriptionDetailsEl = document.createElement('dd');

                const details = generalInfo['messageTime']
                  ? formatDate(generalInfo['messageTime'], 'd MMMM y, H:mm', localeID)
                  : generalInfo[property]?.toString();

                descriptionDetailsEl.textContent = details ?? DEFAULT_DETAILS;

                descriptionDetailsEls.push(descriptionDetailsEl);

                break;
              }

              case 'speed': {
                const descriptionDetailsEl = document.createElement('dd');

                descriptionDetailsEl.textContent = generalInfo['speed']
                  ? `${formatNumber(generalInfo['speed'], 'en-US', '1.1-1')}  км/ч`
                  : DEFAULT_DETAILS;

                descriptionDetailsEls.push(descriptionDetailsEl);

                break;
              }

              case 'numberOfSatellites': {
                const descriptionDetailsEl = document.createElement('dd');

                descriptionDetailsEl.textContent = generalInfo[property]?.toString() ?? DEFAULT_DETAILS;

                descriptionDetailsEls.push(descriptionDetailsEl);

                break;
              }

              case 'latitude': {
                const descriptionDetailsEl = document.createElement('dd');

                // latitude
                let spanEl = document.createElement('span');

                spanEl.textContent = generalInfo['latitude']
                  ? `ш: ${formatNumber(generalInfo['latitude'], 'en-US', '1.6-6')}°`
                  : DEFAULT_DETAILS;

                descriptionDetailsEl.appendChild(spanEl);

                // longitude
                if (generalInfo['longitude']) {
                  spanEl = document.createElement('span');

                  spanEl.textContent = generalInfo['longitude']
                    ? `д: ${formatNumber(generalInfo['longitude'], 'en-US', '1.6-6')}°`
                    : DEFAULT_DETAILS;

                  descriptionDetailsEl.appendChild(spanEl);
                }

                descriptionDetailsEls.push(descriptionDetailsEl);
              }
            }

            divEl.append(...descriptionDetailsEls);
            descriptionListEl.appendChild(divEl);
          }

          divEl.append(headingEl, descriptionListEl);

          if (trackerInfo.sensors?.length) {
            const headingEl = document.createElement('h2');
            const descriptionListEl = document.createElement('dl');

            headingEl.textContent = 'Значения датчиков';

            for (const { name, value, unit } of trackerInfo.sensors) {
              const descriptionTermEl = document.createElement('dt');
              const descriptionDetailsEl = document.createElement('dd');

              descriptionTermEl.textContent = name;
              descriptionDetailsEl.textContent = `${value} ${unit}`;

              descriptionListEl.append(descriptionTermEl, descriptionDetailsEl);
            }

            divEl.append(headingEl, descriptionListEl);
          }

          if (trackerInfo.parameters?.length) {
            const headingEl = document.createElement('h2');
            const wrapperDivEl = document.createElement('div');

            headingEl.textContent = 'Параметры';

            for (const { paramName, lastValueDateTime, lastValueDecimal, lastValueString } of trackerInfo.parameters) {
              const spanEl = document.createElement('span');

              spanEl.textContent = `${paramName}=${lastValueString ?? lastValueDecimal ?? lastValueDateTime}`;

              wrapperDivEl.append(spanEl);
            }

            divEl.append(headingEl, wrapperDivEl);
          }

          return divEl.innerHTML;
        })
      );

    return firstValueFrom(popup$);
  };

  /**
   * Add, update map layers with tracks.
   *
   * @param features Map track features.
   */
  async #setTrackLayer(features: Feature<LineString>[]) {
    const track: FeatureCollection<LineString> = {
      type: 'FeatureCollection',
      features
    };

    await this.#map?.setLayerData(TRACK_LAYER_DEFINITION, track);

    let trackLayer = this.#map?.getLayer<FeatureLayerAdapter<FeatureProperties, LineString>>(TRACK_LAYER_DEFINITION);

    if (!trackLayer && features.length) {
      trackLayer = await this.#map?.addFeatureLayer({
        id: TRACK_LAYER_DEFINITION,
        data: track,
        paint: {
          strokeColor: '#0306DF',
          strokeOpacity: 0.8,
          weight: 3
        }
      });
    }
  }

  /**
   * Add, update map layer with location icons.
   *
   * @param features Map location features.
   */
  async #setLocationLayer(features: Feature<Point>[]) {
    const icons: FeatureCollection<Point> = {
      type: 'FeatureCollection',
      features
    };

    await this.#map?.setLayerData(LOCATION_LAYER_DEFINITION, icons);

    let locationLayer = this.#map?.getLayer<FeatureLayerAdapter<FeatureProperties, Point>>(LOCATION_LAYER_DEFINITION);

    if (!locationLayer && features.length) {
      locationLayer = await this.#map?.addFeatureLayer({
        id: LOCATION_LAYER_DEFINITION,
        data: icons,
        paint: getIcon({
          svg: TECH_SVG,
          color: '#D3007C',
          stroke: TECH_SVG_STROKE,
          size: TECH_SVG_SIZE
        })
      });
    }
  }

  /**
   * Add, update map layers with message icons.
   *
   * @param collections Map message feature collections.
   */
  async #setMessageLayer(collections: FeatureCollection<Point>[]) {
    let messageLayer = this.#map?.getLayer<FeatureLayerAdapter<FeatureProperties, Point>>(MESSAGE_LAYER_DEFINITION);

    await messageLayer?.clearLayer?.();

    if (!messageLayer && collections.length) {
      messageLayer = await this.#map?.addFeatureLayer({
        id: MESSAGE_LAYER_DEFINITION,
        data: {
          type: 'FeatureCollection',
          features: []
        },
        paint: {
          color: '#1413FA',
          opacity: 1,
          strokeColor: '#FFFFFF00',
          radius: 4,
          weight: 5
        },
        selectable: true,
        popupOnSelect: true,
        popupOptions: {
          autoPan: true,
          maxWidth: 400,
          unselectOnClose: true,
          createPopupContent: this.#createMessagePopupContent
        }
      });
    }

    for (const messages of collections) {
      await messageLayer?.addData?.(messages);
    }
  }

  /**
   * Gather location data, set track, location, message layers.
   *
   * A helper method to handle asynchronous adding, updating map layers with location from `Input`.
   *
   * @param location Location and tracks with messages.
   */
  async #setLocationLayers(location: LocationAndTrackResponse) {
    const tracks: Feature<LineString>[] = [];
    const locations: Feature<Point>[] = [];
    const messages: FeatureCollection<Point>[] = [];

    for (const {
      vehicleId: techID,
      vehicleName: techName,
      latitude,
      longitude,
      track
    } of location.tracks) {
      const location: Feature<Point> = {
        type: 'Feature',
        geometry: {
          type: 'Point',
          coordinates: [longitude, latitude]
        },
        properties: {
          id: techID
        }
      };

      locations.push(location);

      if (track) {
        const coordinates: Position[] = [];
        const messageFeatures: Feature<Point>[] = [];

        for (const {
          messageId: id,
          latitude,
          longitude
        } of track) {
          if (longitude && latitude) {
            const position: Position = [longitude, latitude];

            coordinates.push(position);

            const message: Feature<Point> = {
              type: 'Feature',
              geometry: {
                type: 'Point',
                coordinates: position
              },
              properties: { id, techID, techName }
            };

            messageFeatures.push(message);
          }
        }

        const trackFeature: Feature<LineString> = {
          type: 'Feature',
          properties: {},
          geometry: {
            type: 'LineString',
            coordinates
          }
        };

        tracks.push(trackFeature);

        const trackMessages: FeatureCollection<Point> = {
          type: 'FeatureCollection',
          features: messageFeatures
        };

        messages.push(trackMessages);
      }
    }

    await this.#setTrackLayer(tracks);
    await this.#setLocationLayer(locations);
    await this.#setMessageLayer(messages);
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
  #fitView({ viewBounds, tracks }: LocationAndTrackResponse) {
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
    } else if (!tracks.length) {
      this.#map?.fitLayer(FIELD_LAYER_DEFINITION, FIT_OPTIONS);
    }
  }

  /**
   * Close message layer popup for unselected track.
   *
   * @param location Location and tracks.
   */
  #closeMessagePopup({ tracks }: LocationAndTrackResponse) {
    const track = tracks.find(({ vehicleId, track }) => vehicleId === this.#messagePopupTechID && /* istanbul ignore next */ track?.length);

    if (!track) {
      // close popup by unselecting the message layer as `closePopup` is not implemented
      this.#map?.unSelectLayer(MESSAGE_LAYER_DEFINITION);

      this.#messagePopupTechID = undefined;
    }
  }

  constructor(private elementRef: ElementRef, private techService: TechService, private relativeTimePipe: RelativeTimePipe) { }

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

const FIELD_RESOURCE_DEFINITION: ResourceDefinition = 23;
const FIELD_LAYER_DEFINITION = 'field';

const TRACK_LAYER_DEFINITION = 'track';
const LOCATION_LAYER_DEFINITION = 'location';
const MESSAGE_LAYER_DEFINITION = 'message';

const INITIAL_DURATION = 500;
const duration = 2500;
const padding = 60;

const FIT_OPTIONS: FitOptions = { duration, padding };

const TECH_SVG_SIZE = 38;
const TECH_SVG_HEIGHT_RATIO = 377.637 / 463.52;
const TECH_SVG_HEIGHT = TECH_SVG_HEIGHT_RATIO * TECH_SVG_SIZE;
const TECH_SVG_STROKE = 3;
const TECH_SVG_VIEW_BOX_MIN_X = TECH_SVG_STROKE;
const TECH_SVG_VIEW_BOX_MIN_Y = TECH_SVG_STROKE - TECH_SVG_HEIGHT;
const TECH_SVG_VIEW_BOX_SIZE_FACTOR = 10.25;
const TECH_SVG_VIEW_BOX_SIZE = 2 * TECH_SVG_STROKE + TECH_SVG_VIEW_BOX_SIZE_FACTOR * TECH_SVG_SIZE;
const TECH_SVG_VIEW_BOX = `- ${TECH_SVG_VIEW_BOX_MIN_X} ${TECH_SVG_VIEW_BOX_MIN_Y} ${TECH_SVG_VIEW_BOX_SIZE} ${TECH_SVG_VIEW_BOX_SIZE}`;

// eslint-disable-next-line max-len
const TECH_SVG = `<svg version="1.1" xmlns="http://www.w3.org/2000/svg" width="${TECH_SVG_SIZE}" height="${TECH_SVG_SIZE}" viewBox="${TECH_SVG_VIEW_BOX}"><path d="M12.565 99.331c-1.913-.802-3.596-2.167-4.668-3.786-1.638-2.473-1.61-2.087-1.61-22.19 0-20.747-.006-20.672 2.015-26.445.662-1.891 1.263-3.63 1.335-3.864.11-.354-.21-.455-1.929-.611-2.716-.246-4.33-.97-5.796-2.602a7.409 7.409 0 01.69-10.606c1.786-1.528 2.876-1.764 8.165-1.764h4.651l3.275-8.93c1.801-4.91 3.575-9.494 3.943-10.185.897-1.689 3.412-4.039 5.195-4.855 3-1.374 11.817-2.746 21.261-3.31 5.82-.347 24.405-.17 29.029.275 7.974.77 14.231 1.914 16.743 3.062 1.733.792 4.253 3.158 5.14 4.828.367.69 2.142 5.274 3.943 10.186l3.274 8.93h4.651c5.29 0 6.38.235 8.165 1.763a7.409 7.409 0 01.691 10.606c-1.467 1.632-3.08 2.356-5.797 2.602-1.719.156-2.037.257-1.928.611.072.234.673 1.973 1.335 3.864 2.05 5.849 2.039 5.702 1.956 27.012l-.073 18.761-.69 1.455c-1.025 2.159-2.628 3.811-4.676 4.82-1.641.807-1.955.869-4.418.869-2.31 0-2.839-.09-4.107-.69-2.157-1.024-3.81-2.628-4.819-4.675-.84-1.706-.872-1.893-.96-5.542l-.091-3.777H26.18l-.093 3.77c-.083 3.405-.159 3.911-.781 5.225-1.02 2.156-2.625 3.81-4.673 4.82-1.54.758-2.07.878-4.153.941-2.033.062-2.616-.023-3.915-.568zm14.568-35.195c1.356-.687 2.635-1.959 3.423-3.405.348-.638.467-1.463.468-3.237.001-2.121-.078-2.517-.725-3.616-2.533-4.31-7.892-5.203-11.54-1.924-1.666 1.499-2.451 3.276-2.451 5.552 0 3.444 2.058 6.132 5.589 7.297 1.154.381 3.845.038 5.236-.667zm75.132 0c4.71-2.407 5.538-8.655 1.615-12.182-2.911-2.618-6.972-2.657-9.884-.094-2.927 2.577-3.415 7.019-1.103 10.059.908 1.194 3.202 2.738 4.374 2.944 1.422.25 3.75-.09 4.998-.727zM97.503 34.54a3402.59 3402.59 0 01-3.755-11.245l-3.633-10.914H32.524l-3.633 10.914a3404.522 3404.522 0 01-3.754 11.245c-.097.264 7.25.33 36.183.33 28.932 0 36.28-.066 36.183-.33z"/></svg>`;
