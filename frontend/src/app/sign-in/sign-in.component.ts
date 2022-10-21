import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
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
export class SignInComponent implements OnInit, OnDestroy {
  /**
   * Submit Sign in form, checking validation state.
   */
  async submitSignInForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.signInForm;

    if (invalid) {
      return;
    }

    this.#subscription = this.authService
      .signIn(value as Credentials)
      .subscribe(async () => {
        await this.router.navigate(['/'], {
          replaceUrl: true
        });
      });
  }

  protected systemVersion$!: Observable<string>;
  protected signInForm!: FormGroup;
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

  #subscription: Subscription | undefined;

  /**
   * Initialize Sign in form.
   */
  #initSignInForm() {
    this.signInForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  constructor(private fb: FormBuilder, private router: Router, private systemService: SystemService, private authService: AuthService) { }

  ngOnInit() {
    this.systemVersion$ = this.systemService.getVersion$;

    this.#initSignInForm();
  }

  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}
