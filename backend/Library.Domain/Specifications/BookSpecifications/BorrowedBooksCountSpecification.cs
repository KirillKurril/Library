using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BorrowedBooksCountSpecification : BaseSpecification<BookLending>
    {
        public BorrowedBooksCountSpecification(
                Guid userId,
                string searchTerm
        )
        {
            AddCriteria(bl => bl.UserId == userId);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                AddCriteria(bl =>
                    bl.Book.Title.ToLower().Contains(searchTerm.ToLower())
                );
            }
        }
    }
}
