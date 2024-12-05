using System;

namespace Library.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int AuthorId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable => BorrowedAt is null;
        public int? UserId { get; set; }
        public DateTime? BorrowedAt { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
    }
}
//Background tasks with hosted services in ASP.NET Core