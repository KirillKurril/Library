namespace Library.Application.DTOs
{
    public class BookCatalogDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int GenreId { get; set; }
        public int AuthorId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
