import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogTitle } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';

import { ConfirmationDialogComponent } from './confirmation-dialog.component';

describe('ConfirmationDialogComponent', () => {
  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmationDialogComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render dialog title', () => {
    const dialogTitleDe = fixture.debugElement.query(
      By.directive(MatDialogTitle)
    );

    expect(dialogTitleDe)
      .withContext('render dialog title element')
      .not.toBeNull();

    expect(dialogTitleDe.nativeElement.textContent)
      .withContext('render dialog title text')
      .toBe('Удаление');
  });
});
