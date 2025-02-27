using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookLendingByBookIdUserIdSpecification : BaseSpecification<BookLending>
    {
        public BookLendingByBookIdUserIdSpecification(Guid bookId, Guid userId)
        {
            AddCriteria(bl => bl.BookId == bookId);
            AddCriteria(bl => bl.UserId == userId);
        }
    }
}
