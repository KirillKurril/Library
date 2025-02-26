using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookLendingsByBookIdSpecification : BaseSpecification<BookLending>
    {
        public BookLendingsByBookIdSpecification(Guid bookId)
        {
            AddCriteria(bl => bl.BookId == bookId);
        }
    }
}
