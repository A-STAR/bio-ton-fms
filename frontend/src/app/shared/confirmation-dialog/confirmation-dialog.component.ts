import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogConfig, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'bio-confirmation-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfirmationDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) protected data: ConfirmationDialogData) { }
}

export type ConfirmationDialogData = {
  content: InnerHTML['innerHTML'];
}

export const confirmationDialogConfig: MatDialogConfig<ConfirmationDialogData> = {
  width: '630px'
};

export const confirmationDialogContentPartials: [string, string] = ['Вы действительно хотите удалить ', '?'];

export function getConfirmationDialogContent<T extends boolean | undefined = undefined>(
  name: string,
  isTextContent?: T,
  [contentStart, contentEnd] = confirmationDialogContentPartials
) {
  const paragraphEl = document.createElement('p');
  const strongEl = document.createElement('strong');

  strongEl.append(name);
  paragraphEl.append(contentStart, strongEl, contentEnd);

  return (
    isTextContent ? paragraphEl.textContent : paragraphEl.innerHTML
  ) as T extends boolean | undefined ? InnerHTML['innerHTML'] : Node['textContent'];
}
