import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { StopClickPropagationDirective } from './stop-click-propagation.directive';

describe('StopClickPropagationDirective', () => {
  let fixture: ComponentFixture<TestButtonComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TestButtonComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TestButtonComponent);

    fixture.detectChanges();
  });

  it('should create an instance', () => {
    const directive = new StopClickPropagationDirective();

    expect(directive)
      .toBeTruthy();
  });

  it('should prevent characters from input', () => {
    const directiveDe = fixture.debugElement.query(
      By.directive(StopClickPropagationDirective)
    );

    const event = new MouseEvent('click');

    spyOn(event, 'stopPropagation');

    directiveDe.triggerEventHandler('click', event);

    expect(event.stopPropagation)
      .toHaveBeenCalled();
  });
});

@Component({
  standalone: true,
  imports: [StopClickPropagationDirective],
  template: `
    <button bioStopClickPropagation></button>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
class TestButtonComponent { }
