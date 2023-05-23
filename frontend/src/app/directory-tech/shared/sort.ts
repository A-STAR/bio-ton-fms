export enum SortDirection {
  Acending = 'ascending',
  Descending = 'descending'
}

/**
 * Sort options accept custom `SortBy` enum generic.
 */
export type SortOptions<SortBy> = Partial<{
  sortBy: SortBy;
  sortDirection: SortDirection;
}>;
