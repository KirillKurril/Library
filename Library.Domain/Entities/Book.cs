namespace Library.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable => Quantity > 0;

    }
}
//Background tasks with hosted services in ASP.NET Core