import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackerCommandDialogComponent } from './tracker-command-dialog.component';

describe('TrackerCommandDialogComponent', () => {
  let component: TrackerCommandDialogComponent;
  let fixture: ComponentFixture<TrackerCommandDialogComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TrackerCommandDialogComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TrackerCommandDialogComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });
});
