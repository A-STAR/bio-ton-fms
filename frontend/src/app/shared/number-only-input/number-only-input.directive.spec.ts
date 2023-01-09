import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { NumberOnlyInputDirective } from './number-only-input.directive';

describe('NumberOnlyInputDirective', () => {
  let fixture: ComponentFixture<TestNumberInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TestNumberInputComponent,
        NumberOnlyInputDirective
      ]
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

  it('should prevent characters from input', async () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    const input = directiveDe.nativeElement as HTMLInputElement;

    const event = new KeyboardEvent('keydown', {
      key: 'e'
    });

    input.dispatchEvent(event);
    fixture.detectChanges();

    expect(input.value)
      .withContext('prevent character input')
      .toBe('');
  });

  it('should prevent characters from paste', async () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(NumberOnlyInputDirective)
    );

    const input = directiveDe.nativeElement as HTMLInputElement;
    const clipboardData = new DataTransfer();

    clipboardData.setData('text/plain', 'e');

    const event = new ClipboardEvent('paste', { clipboardData });

    input.dispatchEvent(event);
    fixture.detectChanges();

    expect(input.value)
      .withContext('prevent character paste')
      .toBe('');
  });
});

@Component({
  standalone: true,
  imports: [NumberOnlyInputDirective],
  template: '<input type="number" bioNumberOnlyInput>'
})
class TestNumberInputComponent { }
