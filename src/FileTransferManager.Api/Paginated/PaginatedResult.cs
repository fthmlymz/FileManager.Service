namespace FileTransferManager.Api.Paginated
{
    public class PaginatedResult<T> : Result<T>
    {
        public PaginatedResult()
        {

        }
        public PaginatedResult(IReadOnlyList<T> data)
        {
            Data = data;
        }

        public PaginatedResult(bool succeeded, IReadOnlyList<T> data = default, List<string> messages = null, int count = 0, int pageNumber = 1, int pageSize = 10)
        {
            Data = data;
            CurrentPage = pageNumber;
            Succeeded = succeeded;
            Messages = messages;
            PageSize = pageSize;
            TotalPages = (count + pageSize - 1) / pageSize;
            TotalCount = count;
            HasPreviousPage = CurrentPage > 1;
            HasNextPage = CurrentPage < TotalPages;
        }

        public IReadOnlyList<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public static PaginatedResult<T> Create(IReadOnlyList<T> data, int count, int pageNumber, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, pageNumber, pageSize);
        }
    }

}
