import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[bioStopClickPropagation]',
  standalone: true
})
export class StopClickPropagationDirective {
  /**
   * Stop click event propagation.
   *
   * @param event Click `MouseEvent`.
   */
  @HostListener('click', ['$event'])
  protected handleClick(event: MouseEvent) {
    event.stopPropagation();
  }
}
