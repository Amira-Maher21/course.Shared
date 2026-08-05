namespace  Shared.Application.SharedModels
{
    public class IndexPageInfo<TData>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<TData>? Data { get; set; }
        public int? TotalCount { get; set; }

        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }

    }
}
