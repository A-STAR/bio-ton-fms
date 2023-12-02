import { Directive } from '@angular/core';

import { AllowedCharsInputDirective } from '../allowed-chars-input/allowed-chars-input.directive';

@Directive({
  selector: '[bioTimeCharsInput]',
  standalone: true
})
export class TimeCharsInputDirective extends AllowedCharsInputDirective {
  protected override allowedCharsPattern = TIME_CHARS_PATTERN;
}

export const TIME_CHARS_PATTERN = /^[\d:]+$/;
