import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogContent, MatDialogTitle } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { ConfirmationDialogComponent, ConfirmationDialogData, getConfirmationDialogContent } from './confirmation-dialog.component';

import { testNewVehicle } from 'src/app/directory-tech/vehicle.service.spec';

describe('ConfirmationDialogComponent', () => {
  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;
  let data: ConfirmationDialogData;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmationDialogComponent],
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: testMatDialogData
        }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    data = TestBed.inject(MAT_DIALOG_DATA);

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

  it('should render dialog content', () => {
    const dialogContentDe = fixture.debugElement.query(
      By.directive(MatDialogContent)
    );

    expect(dialogContentDe)
      .withContext('render dialog content element')
      .not.toBeNull();

    expect(dialogContentDe.nativeElement.textContent)
      .withContext('render dialog content text')
      .toBe(
        getConfirmationDialogContent(testNewVehicle.name, true)
      );

    const strongDe = dialogContentDe.query(
      By.css('strong')
    );

    expect(strongDe)
      .withContext('render strong element')
      .not.toBeNull();

    expect(strongDe.nativeElement.textContent)
      .withContext('render strong text')
      .toBe(testNewVehicle.name);
  });
});

const testMatDialogData: ConfirmationDialogData = {
  content: getConfirmationDialogContent(testNewVehicle.name)
};
