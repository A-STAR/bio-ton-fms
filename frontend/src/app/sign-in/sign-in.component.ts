import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatIconModule } from '@angular/material/icon';

import { Observable, Subscription } from 'rxjs';

import { SystemService } from '../system.service';
import { AuthService, Credentials } from '../auth.service';

@Component({
  selector: 'bio-sign-in',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class SignInComponent implements OnInit, OnDestroy {
  protected systemVersion$!: Observable<string>;
  protected signInForm!: FormGroup<SignInForm>;
  protected hidePassword = true;

  /**
   * Toggle password field visibility in Sign in form.
   *
   * @param event Click event going from a toggle password visibility button.
   */
  protected togglePasswordVisibility(event: MouseEvent) {
    event.stopPropagation();

    this.hidePassword = !this.hidePassword;
  }

  /**
   * Submit Sign in form, checking validation state.
   */
  protected async submitSignInForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.signInForm;

    if (invalid) {
      return;
    }

    this.#subscription = this.authService
      .signIn(value as Credentials)
      .subscribe({
        next: async () => {
          await this.router.navigate(['/'], {
            replaceUrl: true
          });
        },
        error: ({ error }: HttpErrorResponse) => {
          const errors: ValidationErrors = {
            serverError: {
              message: error.message ?? error
            }
          };

          this.signInForm.setErrors(errors);
        }
      });
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  #subscription: Subscription | undefined;

  /**
   * Initialize Sign in form.
   */
  // eslint-disable-next-line @typescript-eslint/member-ordering
  #initSignInForm() {
    this.signInForm = this.fb.group({
      username: this.fb.control<string | null>(null, [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(16)
      ]),
      password: this.fb.control<string | null>(null, Validators.required)
    });
  }

  constructor(private fb: FormBuilder, private router: Router, private systemService: SystemService, private authService: AuthService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.systemVersion$ = this.systemService.version$;

    this.#initSignInForm();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

type SignInForm = {
  username: FormControl<string | null>;
  password: FormControl<string | null>;
}
