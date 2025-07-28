using Microsoft.EntityFrameworkCore;

namespace Shared.Application.Common.Models
{
    public class PaginationResponse<T>
    {
        public PaginationResponse(List<T> items, int count, int page, int pageSize)
        {
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;

            Data.AddRange(items);
        }

        public static async Task<PaginationResponse<T>> ToPagedListAsync(IQueryable<T> source, PaginationFilter paginationFilter, CancellationToken cancellationToken)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                                    .Take(paginationFilter.PageSize)
                                    .ToListAsync(cancellationToken);

            return new PaginationResponse<T>(items, count, paginationFilter.PageNumber, paginationFilter.PageSize);
        }

        public List<T> Data { get; set; } = new List<T>();

        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
