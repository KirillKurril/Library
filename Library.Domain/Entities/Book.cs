using System;

namespace Library.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public Guid AuthorId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public Guid UserId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime ActualReturnDate { get; set; }
    }
}