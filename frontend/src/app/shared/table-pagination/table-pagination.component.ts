import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { NumberOnlyInputDirective } from '../number-only-input/number-only-input.directive';

import { PAGE_NUM as MIN_PAGE, PAGE_SIZE as INITIAL_SIZE, Pagination } from '../../directory-tech/shared/pagination';

@Component({
  selector: 'bio-table-pagination',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    NumberOnlyInputDirective
  ],
  templateUrl: './table-pagination.component.html',
  styleUrls: ['./table-pagination.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TablePaginationComponent implements OnInit {
  /**
   * Table pagination.
   *
   * @returns Pagination.
   */
  get pagination() {
    return this.#pagination;
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  @Input() set pagination(pagination: Pagination['pagination']) {
    this.#pagination = pagination;

    this.paginationForm
      ?.get('page')
      ?.setValue(pagination.pageIndex, {
        emitEvent: false
      });
  }

  protected paginationForm!: PaginationForm;
  #pagination!: Pagination['pagination'];

  /**
   * Switch page by pagination buttons.
   *
   * @param page Page index.
   */
  protected onSwitchPage(page: number) {
    this.paginationForm
      .get('page')
      ?.setValue(page);
  }

  /**
   * Initialize Pagination form.
   */
  #initPaginationForm() {
    this.paginationForm = this.fb.group({
      size: this.fb.nonNullable.control(INITIAL_SIZE),
      page: this.fb.nonNullable.control(this.#pagination.pageIndex, {
        validators: [
          Validators.min(MIN_PAGE),
          Validators.max(this.pagination.total)
        ],
        updateOn: 'blur'
      })
    });
  }

  constructor(private fb: FormBuilder) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initPaginationForm();
  }
}

type PaginationForm = FormGroup<{
  size: FormControl<number>;
  page: FormControl<number>;
}>;
