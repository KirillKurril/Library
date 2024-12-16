namespace Library.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }

        public Guid GenreId { get; set; }
        public Guid AuthorId { get; set; }
        
        public Genre? Genre { get; set; }
        public Author? Author { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsAvailable => Quantity > 0;

    }
}
