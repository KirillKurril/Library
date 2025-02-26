using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Domain.Specifications.BookSpecifications;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class ExpiredBooksSpecification : BaseSpecification<BookLending>
    {
        public ExpiredBooksSpecification()
        {
            AddCriteria(bl => bl.ReturnDate < DateTime.UtcNow);

            AddInclude(bl => bl.Book);
            AddInclude(bl => bl.Book.Author);
        }
    }
}

