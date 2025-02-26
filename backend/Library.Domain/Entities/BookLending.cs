namespace Library.Domain.Entities
{
    public class BookLending : BaseEntity
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime ReturnDate { get; set; }

        public Book? Book { get; set; }
    }
}
