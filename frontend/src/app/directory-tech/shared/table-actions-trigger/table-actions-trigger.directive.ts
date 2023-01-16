import { Directive, HostListener, OnDestroy } from '@angular/core';

import { delay, of, Subscription } from 'rxjs';

@Directive({
  selector: '[bioTableActionsTrigger]',
  standalone: true
})
export class TableActionsTriggerDirective implements OnDestroy {
  /**
   * Defer the hover effect of the table action button by adding a special hover class
   * after the delay to avoid false triggering of the action menu display.
   *
   * @param event Enter `MouseEvent`.
   */
  @HostListener('mouseenter', ['$event'])
  protected handleMouseEnter({ target }: MouseEvent) {
    this.#subscription = of(undefined)
      .pipe(
        delay(HOVER_DELAY_MS)
      )
      .subscribe(() => {
        (target as HTMLButtonElement)
          .classList.add(DEFERRED_HOVER_CLASS_NAME);
      });
  }

  /**
   * Unsubscribe from the stream of adding a deferred hover class
   * after moving away from the button.
   *
   * Remove the deferred hover class in case it has already been added.
   *
   * @param event Leave `MouseEvent`.
   */
  @HostListener('mouseleave', ['$event'])
  protected handleMouseLeave({ target }: MouseEvent) {
    this.#subscription?.unsubscribe();

    const buttonEl = target as HTMLButtonElement;

    if (buttonEl.classList.contains(DEFERRED_HOVER_CLASS_NAME)) {
      buttonEl.classList.remove(DEFERRED_HOVER_CLASS_NAME);
    }
  }

  #subscription?: Subscription;

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

export const HOVER_DELAY_MS = 300;
export const DEFERRED_HOVER_CLASS_NAME = 'deferred-hover';
