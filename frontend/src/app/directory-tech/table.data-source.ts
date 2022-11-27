import { DataSource } from '@angular/cdk/collections';

import { Observable, ReplaySubject } from 'rxjs';

export class TableDataSource<T> extends DataSource<T> {
  #dataSource$ = new ReplaySubject<T[]>();

  connect(): Observable<T[]> {
    return this.#dataSource$.asObservable();
  }

  disconnect() { }

  setDataSource(dataSource: T[]) {
    this.#dataSource$.next(dataSource);
  }

  constructor(dataSource: T[]) {
    super();

    this.setDataSource(dataSource);
  }
}
