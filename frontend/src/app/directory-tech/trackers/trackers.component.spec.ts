import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { Observable, of } from 'rxjs';

import { Trackers, TrackersService } from '../trackers.service';

import TrackersComponent from './trackers.component';

import { testTrackers } from '../trackers.service.spec';

describe('TrackersComponent', () => {
  let component: TrackersComponent;
  let fixture: ComponentFixture<TrackersComponent>;

  let trackersSpy: jasmine.Spy<() => Observable<Trackers>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          TrackersComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackersComponent);

    const trackersService = TestBed.inject(TrackersService);

    component = fixture.componentInstance;

    const trackers$ = of(testTrackers);

    trackersSpy = spyOn(trackersService, 'getTrackers')
      .and.returnValue(trackers$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get trackers', () => {
    expect(trackersSpy)
      .toHaveBeenCalled();
  });
});
