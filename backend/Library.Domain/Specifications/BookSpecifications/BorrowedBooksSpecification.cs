using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BorrowedBooksSpecification : BaseSpecification<BookLending>
    {
        public BorrowedBooksSpecification(
            Guid userId,
            int? pageNo,
            int? itemsPerPage,
            string? searchTerm)
        {
            AddCriteria(b => 
                b.UserId == userId)
        }
    }
}
