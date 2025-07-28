using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Models
{
    public class BaseFilter
    {
        /// <summary>
        /// Column Wise Search is Supported.
        /// </summary>
        public Search? AdvancedSearch { get; set; }

        /// <summary>
        /// Keyword to Search in All the available columns of the Resource.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Advanced column filtering with logical operators and query operators is supported.
        /// </summary>
        public Filter? AdvancedFilter { get; set; }
    }

    public class Search
    {
        public List<string> Fields { get; set; } = new();
        public string? Keyword { get; set; }
    }

    public static class FilterOperator
    {
        public const string EQ = "eq";
        public const string NEQ = "neq";
        public const string LT = "lt";
        public const string LTE = "lte";
        public const string GT = "gt";
        public const string GTE = "gte";
        public const string STARTSWITH = "startswith";
        public const string ENDSWITH = "endswith";
        public const string CONTAINS = "contains";
        public const string LIKE = "like";
    }

    public static class FilterLogic
    {
        public const string AND = "and";
        public const string OR = "or";
        public const string XOR = "xor";
    }

    public class Filter
    {
        public string? Logic { get; set; }

        public IEnumerable<Filter>? Filters { get; set; }

        public string? Field { get; set; }

        public string? Operator { get; set; }

        public object? Value { get; set; }
    }
}
