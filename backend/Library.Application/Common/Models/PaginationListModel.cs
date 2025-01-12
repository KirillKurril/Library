namespace Library.Application.Common.Models
{
    public class PaginationListModel<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
