import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { AllowedCharsInputDirective } from './allowed-chars-input.directive';

describe('AllowedCharsInputDirective', () => {
  let fixture: ComponentFixture<TestInputComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TestInputComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TestInputComponent);

    fixture.detectChanges();
  });

  it('should create an instance', () => {
    const directive = new AllowedCharsInputDirective();

    expect(directive)
      .toBeTruthy();
  });

  it('should prevent not allowed characters from input', () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(AllowedCharsInputDirective)
    );

    const event = new InputEvent('beforeinput', {
      data: 'a',
      cancelable: true
    });

    const isEventSucceeded = (directiveDe.nativeElement as HTMLInputElement)
      .dispatchEvent(event);

    expect(isEventSucceeded)
      .withContext('prevent keydown event')
      .toBe(false);
  });

  it('should prevent not allowed characters from paste', () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(AllowedCharsInputDirective)
    );

    const clipboardData = new DataTransfer();

    const testData = '12;00';

    clipboardData.setData('text/plain', testData);

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
  imports: [AllowedCharsInputDirective],
  template: `<input [bioAllowedCharsInput]="pattern">
`,
  changeDetection: ChangeDetectionStrategy.OnPush
})
class TestInputComponent {
  protected pattern = TEST_PATTERN;
}

const TEST_PATTERN = /^[\d.:]+$/;

