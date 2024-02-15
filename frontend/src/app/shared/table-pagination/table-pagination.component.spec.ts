import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSelectHarness } from '@angular/material/select/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
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
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Начало"]',
        text: 'keyboard_double_arrow_left',
        variant: 'icon'
      })
    );

    await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Предыдущая"]',
        text: 'keyboard_arrow_left',
        variant: 'icon'
      })
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

    await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Следующая"]',
        text: 'keyboard_arrow_right',
        variant: 'icon'
      })
    );

    await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Конец"]',
        text: 'keyboard_double_arrow_right',
        variant: 'icon'
      })
    );
  });

  it('should switch the page', async () => {
    const pageInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#pagination-form'
      })
    );

    let page = 5;

    await pageInput.setValue(`${page}`);

    const nextButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Следующая"]',
        text: 'keyboard_arrow_right',
        variant: 'icon'
      })
    );

    await nextButton.click();

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set next page')
      .toBeResolvedTo(`${++page}`);

    await nextButton.click();

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set next page')
      .toBeResolvedTo(`${++page}`);

    const previousButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Предыдущая"]',
        text: 'keyboard_arrow_left',
        variant: 'icon'
      })
    );

    await previousButton.click();

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set previous page')
      .toBeResolvedTo(`${--page}`);

    await previousButton.click();

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set previous page')
      .toBeResolvedTo(`${--page}`);

    const beginningButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Начало"]',
        text: 'keyboard_double_arrow_left',
        variant: 'icon'
      })
    );

    await beginningButton.click();

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set the 1st page')
      .toBeResolvedTo(`${1}`);

    const endButton = await loader.getHarness(
      MatButtonHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[title="Конец"]',
        text: 'keyboard_double_arrow_right',
        variant: 'icon'
      })
    );

    await endButton.click();

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set the last page')
      .toBeResolvedTo(`${pagination.pagination.total}`);
  });

  it('should reset page while changing pagination size', async () => {
    const pageInput = await loader.getHarness(
      MatInputHarness.with({
        ancestor: 'form#pagination-form'
      })
    );

    const page = 5;

    await pageInput.setValue(`${page}`);

    const sizeSelect = await loader.getHarness(
      MatSelectHarness.with({
        ancestor: 'form#pagination-form',
        selector: '[placeholder="Размер"]',
      })
    );

    await sizeSelect.clickOptions({
      text: '200'
    });

    await expectAsync(
      pageInput.getValue()
    )
      .withContext('set the 1st page')
      .toBeResolvedTo(`${1}`);
  });

  it('should render messages summary', async () => {
    const messagesSummaryDe = fixture.debugElement.query(
      By.css('output')
    );

    expect(messagesSummaryDe)
      .withContext('render messages summary element')
      .not.toBeNull();

    expect(messagesSummaryDe.nativeElement.textContent)
      .withContext('render messages summary text')
      .toBe(`Отображается с ${pagination.pagination.pageIndex * PAGE_SIZE - PAGE_SIZE + 1} по ${
        pagination.pagination.pageIndex === pagination.pagination.total
          ? pagination.pagination.records % PAGE_SIZE
          : pagination.pagination.pageIndex * PAGE_SIZE
      } из ${pagination.pagination.records} сообщений`);
  });
});

const pagination: Pagination = {
  pagination: {
    pageIndex: PAGE_NUM,
    total: 10,
    records: 453
  }
};
