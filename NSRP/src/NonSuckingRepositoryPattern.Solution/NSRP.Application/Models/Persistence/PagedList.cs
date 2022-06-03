namespace NSRP.Application.Models.Persistence
{
    public class PagedList<T>
    {
        public int PageTotal { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }

        public IReadOnlyList<T> Result { get; set; }

        public PagedList(
            int pageTotal,
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection,
            IReadOnlyList<T> result)
        {
            PageTotal = pageTotal;
            Page = page;
            PageSize = pageSize;
            SortColumn = sortColumn;
            SortDirection = sortDirection;
            Result = result;
        }
    }
}
