import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { Subscription, debounceTime, filter } from 'rxjs';

import { NumberOnlyInputDirective } from '../number-only-input/number-only-input.directive';

import { PAGE_NUM as INITIAL_PAGE, PAGE_SIZE as INITIAL_SIZE, Pagination, PaginationOptions } from '../../directory-tech/shared/pagination';

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
export class TablePaginationComponent implements OnInit, OnDestroy {
  @Output() protected paginationChange = new EventEmitter<PaginationOptions>();

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

  #pagination!: Pagination['pagination'];
  #subscription?: Subscription;

  /**
   * Initialize Pagination form.
   *
   * Reset page on size value change. Emit pagination change event.
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

    this.#subscription = this.paginationForm.get('size')
      ?.valueChanges
      .pipe(
        debounceTime(DEBOUNCE_DUE_TIME)
      )
      .subscribe(() => {
        this.paginationForm
          .get('page')
          ?.setValue(INITIAL_PAGE);
      });

    const subscription = this.paginationForm.valueChanges
      .pipe(
        filter((value): value is {
          page: number;
          size: number;
        } => value.page !== undefined),
        debounceTime(DEBOUNCE_DUE_TIME)
      )
      .subscribe(({ page, size }) => {
        this.paginationChange.emit({
          pageNum: page,
          pageSize: size
        });
      });

    this.#subscription?.add(subscription);
  }

  constructor(private fb: FormBuilder) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initPaginationForm();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

type PaginationForm = FormGroup<{
  size: FormControl<number>;
  page: FormControl<number>;
}>;

const MIN_PAGE = INITIAL_PAGE;

export const DEBOUNCE_DUE_TIME = 300;
