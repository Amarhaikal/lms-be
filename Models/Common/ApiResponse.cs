namespace QUANTM.Model.Common
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class PagedData<T>
    {
        public List<T> List { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
