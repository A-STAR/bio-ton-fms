import { DataSource } from '@angular/cdk/collections';

import { Observable, ReplaySubject } from 'rxjs';

export class TableDataSource<T> extends DataSource<T> {
  /**
   * Get data.
   *
   * @returns Data source data.
   */
  get data() {
    return this.#dataSource;
  }

  connect(): Observable<T[]> {
    return this.#dataSource$.asObservable();
  }

  // eslint-disable-next-line no-empty-function
  disconnect() { }

  /**
   * Set data source.
   *
   * @param dataSource Data source.
   */
  setDataSource(dataSource: T[]) {
    this.#dataSource = dataSource;

    this.#dataSource$.next(dataSource);
  }

  #dataSource$ = new ReplaySubject<T[]>();
  #dataSource!: T[];

  constructor(dataSource: T[]) {
    super();

    this.setDataSource(dataSource);
  }
}
