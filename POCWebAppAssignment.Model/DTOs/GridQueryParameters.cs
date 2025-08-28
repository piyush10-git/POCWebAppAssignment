using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Model.DTOs
{
    public class GridQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<ColumnFilter>? Filters { get; set; } = new();
        public List<ColumnSort>? Sorts { get; set; } = new();
    }

    public class ColumnFilter
    {
        public string Field { get; set; } = string.Empty;   // Column name
        public string Operator { get; set; } = "contains";  // eq, contains, startswith, etc.
        public string Value { get; set; } = string.Empty;   // Value to match
    }
    public class ColumnSort
    {
        public string Field { get; set; } = string.Empty;   // Column name
        public string Direction { get; set; } = "asc";      // "asc" or "desc"
    }


}
