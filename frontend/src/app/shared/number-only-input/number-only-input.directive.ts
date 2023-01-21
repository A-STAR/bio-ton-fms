import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[bioNumberOnlyInput]',
  standalone: true
})
export class NumberOnlyInputDirective {
  /**
   * Prevent characters other than number for input of type `number`.
   *
   * @param event Input `KeyboardEvent`.
   */
  @HostListener('keydown', ['$event'])
  protected handleKeyDown(event: KeyboardEvent) {
    const isCharacter = forbiddenCharacters.includes(event.key);

    if (isCharacter) {
      event.preventDefault();
    }
  }

  /**
    * Prevent characters other than number from paste into input of type `number`.
    *
    * @param event Paste `ClipboardEvent`.
    */
  @HostListener('paste', ['$event'])
  protected handlePaste(event: ClipboardEvent) {
    const data = event.clipboardData?.getData('text/plain');

    if (data) {
      const isCharacter = forbiddenCharacters.some(character => data.includes(character));

      if (isCharacter) {
        event.preventDefault();
      }
    }
  }
}

const forbiddenCharacters = ['e', 'E', '+', '-'];
