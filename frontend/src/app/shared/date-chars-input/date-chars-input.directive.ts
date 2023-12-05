import { Directive } from '@angular/core';

import { AllowedCharsInputDirective } from '../allowed-chars-input/allowed-chars-input.directive';

@Directive({
  selector: '[bioDateCharsInput], [matDatepicker]',
  standalone: true
})
export class DateCharsInputDirective extends AllowedCharsInputDirective {
  protected override allowedCharsPattern = DATE_CHARS_PATTERN;
}

export const DATE_CHARS_PATTERN = /^[\d.]+$/;
