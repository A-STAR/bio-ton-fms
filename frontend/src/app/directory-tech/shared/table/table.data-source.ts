import { DataSource } from '@angular/cdk/collections';

import { Observable, ReplaySubject } from 'rxjs';

export class TableDataSource<T> extends DataSource<T> {
  connect(): Observable<T[]> {
    return this.#dataSource$.asObservable();
  }

  disconnect() { }

  setDataSource(dataSource: T[]) {
    this.#dataSource$.next(dataSource);
  }

  #dataSource$ = new ReplaySubject<T[]>();

  constructor(dataSource: T[]) {
    super();

    this.setDataSource(dataSource);
  }
}
