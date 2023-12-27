import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogConfig, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'bio-confirmation-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule
  ],
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfirmationDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) protected data: InnerHTML['innerHTML']) { }
}

export const confirmationDialogConfig: MatDialogConfig<InnerHTML['innerHTML']> = {
  width: '630px'
};

export const confirmationDialogContentPartials: {
  contentStart: string;
  contentEnd: string;
} = {
  contentStart: 'Вы действительно хотите удалить ',
  contentEnd: '?'
};

/**
 * Construct confirmation dialog content.
 *
 * @param entity Entity for action confirmation.
 * @param isTextContent HTML or plain text content flag.
 * @param contentStart Text content start partial.
 * @param contentEnd Text content end partial.
 *
 * @returns Confirmation dialog content.
 */
export function getConfirmationDialogContent<T extends boolean | undefined = undefined>(
  entity: string,
  isTextContent?: T,
  contentStart = confirmationDialogContentPartials.contentStart,
  contentEnd = confirmationDialogContentPartials.contentEnd
) {
  const paragraphEl = document.createElement('p');
  const strongEl = document.createElement('strong');

  strongEl.append(entity);
  paragraphEl.append(contentStart, strongEl, contentEnd);

  return (
    isTextContent ? paragraphEl.textContent : paragraphEl.innerHTML
  ) as T extends boolean | undefined ? InnerHTML['innerHTML'] : Node['textContent'];
}
