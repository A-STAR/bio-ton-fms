import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import AgroComponent from './agro.component';
import { MapComponent } from '../shared/map/map.component';

describe('AgroComponent', () => {
  let component: AgroComponent;
  let fixture: ComponentFixture<AgroComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          AgroComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(AgroComponent);

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
      .withContext('render `bio-map` component')
      .not.toBeNull();
  });
});
