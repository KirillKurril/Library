namespace Library.Application.DTOs
{
    public class UpdateBookDTO
    {
        public Guid Id { get; }
        public string? ISBN { get; }
        public string? Title { get; }
        public string? Description { get; }
        public Guid? Quantity { get; }
        public Guid? GenreId { get; }
        public Guid? AuthorId { get; }
    }
}
