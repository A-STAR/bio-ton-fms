import { Component } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { DEFERRED_HOVER_CLASS_NAME, HOVER_DELAY_MS, TableActionsTriggerDirective } from './table-actions-trigger.directive';

describe('TableActionsTriggerDirective', () => {
  let fixture: ComponentFixture<TestActionButtonComponent>;
  let buttonEl: HTMLButtonElement;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TestActionButtonComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TestActionButtonComponent);

    fixture.detectChanges();

    const directiveDe = fixture.debugElement.query(
      By.directive(TableActionsTriggerDirective)
    );

    buttonEl = directiveDe.nativeElement as HTMLButtonElement;
  });

  it('should create an instance', () => {
    const directive = new TableActionsTriggerDirective();

    expect(directive)
      .toBeTruthy();
  });

  it('should prevent adding deferred hover class on mouse leave before delay expires', fakeAsync(() => {
    const event = new MouseEvent('mouseleave');

    buttonEl.dispatchEvent(event);

    tick(HOVER_DELAY_MS);

    expect(
      buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)
    )
      .withContext('have deferred hover class after delay')
      .toBeFalse();
  }));

  it('should defer adding deferred hover class after delay on mouse enter', fakeAsync(() => {
    const event = new MouseEvent('mouseenter');

    buttonEl.dispatchEvent(event);

    expect(
      buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)
    )
      .withContext('initially defer adding deferred hover class')
      .toBeFalse();

    const beforeDelayMS = HOVER_DELAY_MS - 1;

    tick(beforeDelayMS);

    expect(
      buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)
    )
      .withContext('defer adding deferred hover class right before delay expires')
      .toBeFalse();

    tick(HOVER_DELAY_MS);

    expect(
      buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)
    )
      .withContext('add deferred hover class after delay')
      .toBeTrue();
  }));

  it('should remove deferred hover class on mouse leave', fakeAsync(() => {
    let event = new MouseEvent('mouseenter');

    buttonEl.dispatchEvent(event);

    tick(HOVER_DELAY_MS);

    expect(
      buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)
    )
      .withContext('have deferred hover class after delay')
      .toBeTrue();

    event = new MouseEvent('mouseleave');

    buttonEl.dispatchEvent(event);

    expect(
      buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)
    )
      .withContext('remove deferred hover class after leaving button')
      .toBeFalse();
  }));
});

@Component({
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    TableActionsTriggerDirective
  ],
  template: `
    <button mat-icon-button bioTableActionsTrigger>
      <mat-icon>more_horiz</mat-icon>
    </button>
  `
})
class TestActionButtonComponent { }
