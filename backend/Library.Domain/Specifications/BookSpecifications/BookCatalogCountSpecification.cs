using Library.Domain.Abstractions;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookCatalogCountSpecification : BaseSpecification<Book>
    {
        public BookCatalogCountSpecification(
            string? searchTerm,
            Guid? genreId,
            Guid? authorId,
            bool? availableOnly)
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
                AddCriteria(b => b.IsAvailable);
            }
        }
    }
}
