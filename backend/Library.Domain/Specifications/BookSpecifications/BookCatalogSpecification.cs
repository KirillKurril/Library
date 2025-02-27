using Library.Domain.Abstractions;
using Library.Domain.Entities;


namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookCatalogSpecification : BaseSpecification<Book>
    {
        public BookCatalogSpecification(
            string? searchTerm,
            Guid? genreId,
            Guid? authorId,
            bool? availableOnly,
            int? pageNo = null,
            int? itemsPerPage = null)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                AddCriteria(b => b.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            if (genreId.HasValue)
            {
                AddCriteria(b => b.GenreId == genreId.Value);
            }

            if (authorId.HasValue)
            {
                AddCriteria(b => b.AuthorId == authorId.Value);
            }

            if (availableOnly == true)
            {
                AddCriteria(b => b.Quantity > 0);
            }

            ApplyOrderBy(b => b.Id);

            if (pageNo.HasValue && itemsPerPage.HasValue)
                ApplyPaging(
                    (pageNo.Value - 1) * itemsPerPage.Value,
                    itemsPerPage.Value
                    );
        }
    }
}
