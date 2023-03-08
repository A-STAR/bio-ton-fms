import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { NumberOnlyInputDirective } from './number-only-input.directive';

describe('NumberOnlyInputDirective', () => {
  let fixture: ComponentFixture<TestNumberInputComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TestNumberInputComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TestNumberInputComponent);

    fixture.detectChanges();
  });

  it('should create an instance', () => {
    const directive = new NumberOnlyInputDirective();

    expect(directive)
      .toBeTruthy();
  });

  it('should prevent characters from input', () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    const event = new KeyboardEvent('keydown', {
      key: 'e',
      cancelable: true
    });

    const isEventSucceeded = (directiveDe.nativeElement as HTMLInputElement)
      .dispatchEvent(event);

    expect(isEventSucceeded)
      .withContext('prevent keydown event')
      .toBe(false);
  });

  it('should prevent characters from paste', () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    const clipboardData = new DataTransfer();

    clipboardData.setData('text/plain', 'e');

    const event = new ClipboardEvent('paste', {
      clipboardData,
      cancelable: true
    });

    const isEventSucceeded = (directiveDe.nativeElement as HTMLInputElement)
      .dispatchEvent(event);

    expect(isEventSucceeded)
      .withContext('prevent paste event')
      .toBe(false);
  });
});

@Component({
  standalone: true,
  imports: [NumberOnlyInputDirective],
  template: `
    <input type="number" bioNumberOnlyInput>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
class TestNumberInputComponent { }
