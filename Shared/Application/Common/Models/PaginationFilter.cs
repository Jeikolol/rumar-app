using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Models
{
    public class PaginationFilter : BaseFilter
    {
        public int PageNumber { get; set; } = 1;

        public Guid LastId { get; set; }

        const int MaxPageSize = 50; //TODO: Move that to another place

        private int _pageSize = 20;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        public string[]? OrderBy { get; set; }
        public string[]? OrderByDesc { get; set; }
    }

    public static class PaginationFilterExtensions
    {
        public static bool HasOrderBy(this PaginationFilter filter) =>
            filter.OrderBy?.Any() is true;

        public static bool OrderByDesc(this PaginationFilter filter) =>
            filter.OrderByDesc?.Any() is true;
    }
}
