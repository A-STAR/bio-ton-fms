import { Directive, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[bioAllowedCharsInput]',
  standalone: true
})
export class AllowedCharsInputDirective {
  @Input() protected set bioAllowedCharsInput(pattern: RegExp) {
    this.allowedCharsPattern = pattern;
  }

  protected allowedCharsPattern?: RegExp;

  /**
   * Prevent not allowed characters from input.
   *
   * @param event Input `InputEvent`.
   */
  @HostListener('beforeinput', ['$event'])
  protected handleKeyDown(event: InputEvent) {
    if (event.data) {
      const isRestrictedChar = !this.allowedCharsPattern?.test(event.data);

      if (isRestrictedChar) {
        event.preventDefault();
      }
    }
  }

  /**
   * Prevent not allowed characters from paste into input.
   *
   * @param event Paste `ClipboardEvent`.
   */
  @HostListener('paste', ['$event'])
  protected handlePaste(event: ClipboardEvent) {
    const data = event.clipboardData?.getData('text/plain');

    if (data) {
      const hasRestrictedChar = !this.allowedCharsPattern?.test(data);

      if (hasRestrictedChar) {
        event.preventDefault();
      }
    }
  }
}
