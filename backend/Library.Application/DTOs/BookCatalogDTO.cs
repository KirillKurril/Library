namespace Library.Application.DTOs
{
    public class BookCatalogDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid GenreId { get; set; }
        public Guid AuthorId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
