import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { Observable } from 'rxjs';

import { SystemService } from '../system.service';

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
    MatInputModule
  ],
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SignInComponent implements OnInit {
  protected systemVersion$!: Observable<string>;
  protected signInForm!: FormGroup;

  /**
   * Initialize Sign in form.
   */
  #initSignInForm() {
    this.signInForm = this.fb.group({
      username: '',
      password: ''
    });
  }

  constructor(private fb: FormBuilder, private systemService: SystemService) { }

  ngOnInit() {
    this.systemVersion$ = this.systemService.getVersion$;

    this.#initSignInForm();
  }
}
