import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { KeyValue } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatDialogTitle } from '@angular/material/dialog';

import { Observable, of } from 'rxjs';

import { TrackerService, TrackerTypeEnum } from '../tracker.service';

import { TrackerDialogComponent } from './tracker-dialog.component';

import { testTrackerTypeEnum } from '../tracker.service.spec';

describe('TrackerDialogComponent', () => {
  let component: TrackerDialogComponent;
  let fixture: ComponentFixture<TrackerDialogComponent>;
  let trackerService: TrackerService;
  let vehicleService: VehicleService;

  let trackerTypeSpy: jasmine.Spy<(this: TrackerService) => Observable<KeyValue<TrackerTypeEnum, string>[]>>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          HttpClientTestingModule,
          TrackerDialogComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerDialogComponent);
    trackerService = TestBed.inject(TrackerService);

    component = fixture.componentInstance;

    const trackerType$ = of(testTrackerTypeEnum);

    trackerTypeSpy = spyOnProperty(trackerService, 'trackerType$')
      .and.returnValue(trackerType$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should get tracker type enum', () => {
    expect(trackerTypeSpy)
      .toHaveBeenCalled();
  });

  it('should render dialog title', async () => {
    const titleTextDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(titleTextDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(titleTextDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('Добавление GPS-трекера');
  });
});
