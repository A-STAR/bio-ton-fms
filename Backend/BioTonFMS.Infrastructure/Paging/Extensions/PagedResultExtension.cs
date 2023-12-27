namespace BioTonFMS.Infrastructure.Paging.Extensions
{
    public static class PagedResultExtension
    {
        public static PagedResult<T> GetPagedQueryable<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>
            {
                PageSize = pageSize,
                TotalRowCount = query.Count()
            };

            var pageCount = (double)result.TotalRowCount / pageSize;
            result.TotalPageCount = (int)Math.Ceiling(pageCount);
            result.CurrentPage = result.TotalPageCount < page ? result.TotalPageCount : page;

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}
