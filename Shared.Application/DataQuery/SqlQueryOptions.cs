namespace Shared.Application.DataQuery
{
    public class SqlQueryOptions
    {
        public List<QuerySortOption>? Sorts { get; set; }
        public int Skip { get; set; }
        public int? Take { get; set; }

        public bool GetTotal { get; set; }

        public List<string[]>? Filters { get; set; }
    }


    public class QuerySortOption
    {
        public string? FieldName { get; set; }
        public bool? IsAscending { get; set; }
        public bool? IsNullFirst { get; set; }
    }
}
