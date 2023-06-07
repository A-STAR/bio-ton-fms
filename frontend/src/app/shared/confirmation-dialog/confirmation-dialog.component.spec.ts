import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogActions, MatDialogContent, MatDialogTitle, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';
import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatButtonHarness } from '@angular/material/button/testing';

import { ConfirmationDialogComponent, getConfirmationDialogContent } from './confirmation-dialog.component';

import { testNewVehicle } from 'src/app/directory-tech/vehicle.service.spec';

describe('ConfirmationDialogComponent', () => {
  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmationDialogComponent],
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: getConfirmationDialogContent(testNewVehicle.name)
        }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

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

  it('should render dialog actions', async () => {
    const dialogActionsDe = fixture.debugElement.query(
      By.directive(MatDialogActions)
    );

    expect(dialogActionsDe)
      .withContext('render dialog actions element')
      .not.toBeNull();

    const cancelButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'mat-dialog-actions',
        variant: 'stroked',
        text: 'Отмена'
      })
    );

    const deleteButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'mat-dialog-actions',
        variant: 'flat',
        text: 'Удалить'
      })
    );

    const buttonHosts = await parallel(() => [cancelButton, deleteButton].map(
      button => button.host()
    ));

    const dialogResultAttributes = await parallel(() => buttonHosts.map(
      buttonHost => buttonHost.getAttribute('ng-reflect-dialog-result')
    ));

    expect(dialogResultAttributes[0])
      .withContext('render cancel button dialog result attribute')
      .toBe('false');

    expect(dialogResultAttributes[1])
      .withContext('render delete button dialog result attribute')
      .toBe('true');
  });
});
