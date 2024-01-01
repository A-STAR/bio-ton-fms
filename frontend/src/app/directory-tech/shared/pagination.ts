export type PaginationOptions = Partial<{
  pageNum: number;
  pageSize: number;
}>;

export type Pagination = {
  pagination: {
    pageIndex: number;
    total: number;
    records: number;
  };
};

export const PAGE_NUM = 1;
export const PAGE_SIZE = 50;
