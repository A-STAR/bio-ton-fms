import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatInputHarness } from '@angular/material/input/testing';

import { TablePaginationComponent } from './table-pagination.component';

import { PAGE_NUM, PAGE_SIZE, Pagination } from '../../directory-tech/shared/pagination';

describe('TablePaginationComponent', () => {
  let component: TablePaginationComponent;
  let fixture: ComponentFixture<TablePaginationComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          NoopAnimationsModule,
          TablePaginationComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(TablePaginationComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);

    component = fixture.componentInstance;
    component.pagination = pagination.pagination;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render pagination form', async () => {
    const paginationFormDe = fixture.debugElement.query(
      By.css('form#pagination-form')
    );

    expect(paginationFormDe)
      .withContext('render pagination form element')
      .not.toBeNull();

    const sizeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[placeholder="Размер"]',
      })
    );

    await expectAsync(
      sizeSelect.getValueText()
    )
      .toBeResolvedTo(
        PAGE_SIZE.toString()
      );

    await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[bioNumberOnlyInput]',
        value: pagination.pagination.pageIndex.toString()
      })
    );

    const totalPageDe = paginationFormDe.query(
      By.css('label[for="page"] ~ span')
    );

    expect(totalPageDe)
      .withContext('render total pages element')
      .not.toBeNull();

    expect(totalPageDe.nativeElement.textContent)
      .withContext('render total pages text')
      .toBe(`из ${pagination.pagination.total}`);
  });
});

const pagination: Pagination = {
  pagination: {
    pageIndex: PAGE_NUM,
    total: 10,
    records: 453
  }
};
