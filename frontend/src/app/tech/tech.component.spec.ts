import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import TechComponent from './tech.component';
import { MapComponent } from '../shared/map/map.component';

describe('TechComponent', () => {
  let component: TechComponent;
  let fixture: ComponentFixture<TechComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TechComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TechComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render map', () => {
    const mapDe = fixture.debugElement.query(
      By.directive(MapComponent)
    );

    expect(mapDe)
      .withContext('render map component')
      .not.toBeNull();
  });
});
