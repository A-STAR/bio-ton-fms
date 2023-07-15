import { ComponentFixture, TestBed, fakeAsync } from '@angular/core/testing';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatIconHarness } from '@angular/material/icon/testing';

import { TechMonitoringStateComponent } from './tech-monitoring-state.component';

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

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render state', fakeAsync(async () => {
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
          selector: '.mat-primary',
          name: 'stop'
        })
      ),
      loader.getHarness(
        MatIconHarness.with({
          selector: '.mat-primary',
          name: 'podcasts'
        })
      ),
      loader.getHarness(
        MatIconHarness.with({
          selector: '.mat-warn',
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
  }));
});
