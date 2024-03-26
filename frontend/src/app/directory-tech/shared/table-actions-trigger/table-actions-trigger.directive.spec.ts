import { Component, DebugElement } from '@angular/core';
import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { DEFERRED_HOVER_CLASS_NAME, HOVER_DELAY_MS, TableActionsTriggerDirective } from './table-actions-trigger.directive';

describe('TableActionsTriggerDirective', () => {
  let fixture: ComponentFixture<TestActionButtonComponent>;
  let directiveDe: DebugElement;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TestActionButtonComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TestActionButtonComponent);

    fixture.detectChanges();

    directiveDe = fixture.debugElement.query(
      By.directive(TableActionsTriggerDirective)
    );
  });

  it('should create an instance', () => {
    const directive = new TableActionsTriggerDirective();

    expect(directive)
      .toBeTruthy();
  });

  it('should prevent adding deferred hover class on mouse leave before delay expires', fakeAsync(() => {
    directiveDe.triggerEventHandler('mouseleave', {
      target: directiveDe.nativeElement
    });

    tick(HOVER_DELAY_MS);

    expect(directiveDe.classes)
      .withContext('have deferred hover class after delay')
      .not.toEqual(
        jasmine.objectContaining({
          [DEFERRED_HOVER_CLASS_NAME]: true
        })
      );
  }));

  it('should defer adding deferred hover class after delay on mouse enter', fakeAsync(() => {
    directiveDe.triggerEventHandler('mouseenter', {
      target: directiveDe.nativeElement
    });

    expect(directiveDe.classes)
      .withContext('initially defer adding deferred hover class')
      .not.toEqual(
        jasmine.objectContaining({
          [DEFERRED_HOVER_CLASS_NAME]: true
        })
      );

    const beforeDelayMS = HOVER_DELAY_MS - 1;

    tick(beforeDelayMS);

    expect(directiveDe.classes)
      .withContext('defer adding deferred hover class right before delay expires')
      .not.toEqual(
        jasmine.objectContaining({
          [DEFERRED_HOVER_CLASS_NAME]: true
        })
      );

    tick(HOVER_DELAY_MS);

    expect(directiveDe.classes)
      .withContext('add deferred hover class after delay')
      .toEqual(
        jasmine.objectContaining({
          [DEFERRED_HOVER_CLASS_NAME]: true
        })
      );
  }));

  it('should remove deferred hover class on mouse leave', fakeAsync(() => {
    directiveDe.triggerEventHandler('mouseenter', {
      target: directiveDe.nativeElement
    });

    tick(HOVER_DELAY_MS);

    expect(directiveDe.classes)
      .withContext('have deferred hover class after delay')
      .toEqual(
        jasmine.objectContaining({
          [DEFERRED_HOVER_CLASS_NAME]: true
        })
      );

    directiveDe.triggerEventHandler('mouseleave', {
      target: directiveDe.nativeElement
    });

    expect(directiveDe.classes)
      .withContext('remove deferred hover class after leaving button')
      .not.toEqual(
        jasmine.objectContaining({
          [DEFERRED_HOVER_CLASS_NAME]: true
        })
      );
  }));
});

@Component({
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    TableActionsTriggerDirective
  ],
  template: `<button mat-icon-button bioTableActionsTrigger>
  <mat-icon>more_horiz</mat-icon>
</button>
`
})
class TestActionButtonComponent { }
