import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { DATE_PIPE_DEFAULT_OPTIONS, formatDate, registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';

import { RelativeTimePipe, localeID } from '../relative-time.pipe';

import { TechMonitoringStateComponent } from './tech-monitoring-state.component';

import { ConnectionStatus, MovementStatus } from '../../tech.service';
import { MonitoringTech } from '../../tech.component';

import { dateFormat } from '../../../directory-tech/trackers/trackers.component.spec';
import { testMonitoringVehicles } from '../../tech.service.spec';

describe('TechMonitoringStateComponent', () => {
  let component: TechMonitoringStateComponent;
  let fixture: ComponentFixture<TechMonitoringStateComponent>;
  let loader: HarnessLoader;
  let relativeTimePipe: RelativeTimePipe;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TechMonitoringStateComponent],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: localeID
          },
          {
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: { dateFormat }
          },
          RelativeTimePipe
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(TechMonitoringStateComponent);
    relativeTimePipe = TestBed.inject(RelativeTimePipe);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;
  });

  it('should create', () => {
    mockTestTech(component, fixture, testMonitoringVehicles[0]);

    expect(component)
      .toBeTruthy();
  });

  it('should render state', fakeAsync(async () => {
    await testStateRendering(component, fixture, loader, relativeTimePipe, testMonitoringVehicles[0]);
  }));

  it('should render not connected tech without data and satellites state', fakeAsync(async () => {
    await testStateRendering(component, fixture, loader, relativeTimePipe, testMonitoringVehicles[1]);
  }));

  it('should render stopped tech with satellites state', fakeAsync(async () => {
    await testStateRendering(component, fixture, loader, relativeTimePipe, testMonitoringVehicles[2]);
  }));
});

/**
 * Mock test tech.
 *
 * @param component `TechMonitoringStateComponent` test component.
 * @param fixture `ComponentFixture` of `TechMonitoringStateComponent` test component.
 * @param testTech Test tech.
 */
function mockTestTech(component: TechMonitoringStateComponent,
  fixture: ComponentFixture<TechMonitoringStateComponent>, testTech: MonitoringTech) {
  component.tech = testTech;

  fixture.detectChanges();
}

/**
 * Test monitoring state harnesses.
 *
 * @param component `TechMonitoringStateComponent` test component.
 * @param fixture `ComponentFixture` of `TechMonitoringStateComponent` test component.
 * @param loader `HarnessLoader` instance.
 * @param relativeTimePipe `RelativeTimePipe` instance.
 * @param testTech Test tech.
 *
 * @returns `Promise` of `void` state rendering test.
 */
async function testStateRendering(
  component: TechMonitoringStateComponent,
  fixture: ComponentFixture<TechMonitoringStateComponent>,
  loader: HarnessLoader,
  relativeTimePipe: RelativeTimePipe,
  testTech: MonitoringTech
) {
  mockTestTech(component, fixture, testTech);

  const harnesses: PromiseLike<MatButtonHarness | MatIconHarness>[] = [
    loader.getHarness(
      MatButtonHarness.with({
        selector: '.mat-primary',
        variant: 'icon',
        text: 'route'
      })
    ),
    loader.getHarness(
      MatButtonHarness.with({
        selector: '.mat-primary',
        variant: 'icon',
        text: 'my_location'
      })
    ),
    loader.getHarness(
      MatIconHarness.with({
        selector: testTech.movementStatus === MovementStatus.Moving ? '.mat-accent' : '.mat-primary',
        name: testTech.movementStatus === MovementStatus.Moving
          ? 'play_arrow'
          : testTech.movementStatus === MovementStatus.Stopped ? 'stop' : 'sensors_off'
      })
    ),
    loader.getHarness(
      MatIconHarness.with({
        selector: testTech.connectionStatus === ConnectionStatus.Connected ? '.mat-accent' : '.mat-primary',
        name: 'podcasts'
      })
    ),
    loader.getHarness(
      MatIconHarness.with({
        selector: testTech.numberOfSatellites
          ? testTech.numberOfSatellites > 3 ? '.mat-accent' : '.mat-primary'
          : '.mat-warn',
        name: 'signal_cellular_alt'
      })
    )
  ];

  if (testTech.tracker) {
    const sendTrackerCommandButtonHarness = loader.getHarness(
      MatButtonHarness.with({
        selector: '.mat-accent',
        variant: 'icon',
        text: 'sms'
      })
    );

    harnesses.push(sendTrackerCommandButtonHarness);
  }

  const states = await parallel(() => harnesses);

  states.forEach(async (state, index) => {
    const stateEl = await state.host();
    const stateTitle = await stateEl?.getAttribute('title');

    const movementTitle = testTech.movementStatus === MovementStatus.Moving
      ? 'В движении'
      : testTech.movementStatus === MovementStatus.Stopped ? 'Остановка' : 'Нет данных';

    const connectionTime = testTech.lastMessageTime
      ? `, ${relativeTimePipe.transform(testTech.lastMessageTime)} (${formatDate(testTech.lastMessageTime, dateFormat, localeID)})`
      : '';

    const connectionTitle = `${testTech.connectionStatus === ConnectionStatus.Connected ? 'Подключен' : 'Не подключен'}${connectionTime}`;

    const satellitesTitle = `Захвачено ${testTech.numberOfSatellites ?? 0} спутников`;

    const titles = ['Построить трек', 'Следить за объектом', movementTitle, connectionTitle, satellitesTitle, 'Отправить команду'];

    expect(stateTitle)
      .withContext('render state title')
      .toBe(titles[index]);
  });
}
