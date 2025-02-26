using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BorrowedBooksSpecification : BaseSpecification<BookLending>
    {
        public BorrowedBooksSpecification(
            Guid userId,
            string? searchTerm,
            int? pageNo,
            int? itemsPerPage)
        {
            AddCriteria(bl => bl.UserId == userId);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                AddCriteria(bl =>
                    bl.Book.Title.ToLower().Contains(searchTerm.ToLower())
                );
            }

            AddInclude(bl => bl.Book);

            if (pageNo.HasValue && itemsPerPage.HasValue)
                ApplyPaging(
                    (pageNo.Value - 1) * itemsPerPage.Value,
                    itemsPerPage.Value
                    );
            
        }
    }
}
