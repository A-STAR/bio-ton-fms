import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';

import { TechMonitoringStateComponent } from './tech-monitoring-state.component';

import { ConnectionStatus, MonitoringVehicle, MovementStatus } from '../../tech.service';

import { testMonitoringVehicles } from '../../tech.service.spec';

describe('TechMonitoringStateComponent', () => {
  let component: TechMonitoringStateComponent;
  let fixture: ComponentFixture<TechMonitoringStateComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TechMonitoringStateComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TechMonitoringStateComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;
  });

  it('should create', () => {
    mockTestVehicle(component, fixture, testMonitoringVehicles[0]);

    expect(component)
      .toBeTruthy();
  });

  it('should render state', fakeAsync(async () => {
    await testStateRendering(component, fixture, loader, testMonitoringVehicles[0]);
  }));

  it('should render not connected vehicle without data and satellites state', fakeAsync(async () => {
    await testStateRendering(component, fixture, loader, testMonitoringVehicles[1]);
  }));

  it('should render stopped vehicle with satellites state', fakeAsync(async () => {
    await testStateRendering(component, fixture, loader, testMonitoringVehicles[2]);
  }));
});

/**
 * Mock test vehicle.
 *
 * @param component `TechMonitoringStateComponent` test component.
 * @param fixture `ComponentFixture` of `TechMonitoringStateComponent` test component.
 * @param testVehicle Test vehicle.
 */
function mockTestVehicle(component: TechMonitoringStateComponent,
  fixture: ComponentFixture<TechMonitoringStateComponent>, testVehicle: MonitoringVehicle) {
  component.vehicle = testVehicle;

  fixture.detectChanges();
}

/**
 * Test monitoring state harnesses.
 *
 * @param component `TechMonitoringStateComponent` test component.
 * @param fixture `ComponentFixture` of `TechMonitoringStateComponent` test component.
 * @param loader `HarnessLoader`.
 * @param testVehicle Test vehicle.
 *
 * @returns `Promise` of `void` state rendering test.
 */
async function testStateRendering(
  component: TechMonitoringStateComponent,
  fixture: ComponentFixture<TechMonitoringStateComponent>,
  loader: HarnessLoader,
  testVehicle: MonitoringVehicle
) {
  mockTestVehicle(component, fixture, testVehicle);

  const selector = '[bioStopClickPropagation]';

  const harnesses: PromiseLike<MatButtonHarness | MatIconHarness>[] = [
    loader.getHarness(
      MatButtonHarness.with({
        selector: `${selector}.mat-primary`,
        variant: 'icon',
        text: 'route'
      })
    ),
    loader.getHarness(
      MatButtonHarness.with({
        selector: `${selector}.mat-primary`,
        variant: 'icon',
        text: 'my_location'
      })
    ),
    loader.getHarness(
      MatIconHarness.with({
        selector: testVehicle.movementStatus === MovementStatus.Moving ? '.mat-accent' : '.mat-primary',
        name: testVehicle.movementStatus === MovementStatus.Moving ? 'play_arrow' : 'stop'
      })
    ),
    loader.getHarness(
      MatIconHarness.with({
        selector: testVehicle.connectionStatus === ConnectionStatus.Connected ? '.mat-accent' : '.mat-primary',
        name: 'podcasts'
      })
    ),
    loader.getHarness(
      MatIconHarness.with({
        selector: testVehicle.numberOfSatellites
          ? testVehicle.numberOfSatellites > 7 ? '.mat-accent' : testVehicle.numberOfSatellites > 4 ? '.mat-primary' : '.mat-warn'
          : '.mat-warn',
        name: 'signal_cellular_alt'
      })
    ),
    loader.getHarness(
      MatButtonHarness.with({
        selector: `${selector}.mat-accent`,
        variant: 'icon',
        text: 'sms'
      })
    )
  ];

  const states = await parallel(() => harnesses);

  states.forEach(async (state, index) => {
    const stateEl = await state.host();
    const stateTitle = await stateEl?.getAttribute('title');

    const titles = ['Показать путь', 'Показать местоположение', 'Движение', 'Соединение', 'Спутники', 'Отправить команду'];

    expect(stateTitle)
      .withContext('render state title')
      .toBe(titles[index]);
  });
}
