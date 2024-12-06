namespace Library.Application.DTOs
{
    public class BookLendingDTO : BookCatalogDTO
    {
        public DateTime? ReturnDate { get; set; }
    }
}
