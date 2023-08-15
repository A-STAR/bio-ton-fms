import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TechMonitoringInfoComponent } from './tech-monitoring-info.component';

describe('TechMonitoringInfoComponent', () => {
  let component: TechMonitoringInfoComponent;
  let fixture: ComponentFixture<TechMonitoringInfoComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TechMonitoringInfoComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TechMonitoringInfoComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });
});
