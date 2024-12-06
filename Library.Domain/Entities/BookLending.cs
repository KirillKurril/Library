namespace Library.Domain.Entities
{
    public class BookLending : BaseEntity
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime? BorrowedAt { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
