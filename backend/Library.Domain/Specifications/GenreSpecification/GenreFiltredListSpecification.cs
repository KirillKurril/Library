using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.GenreSpecification
{
    public class GenreFiltredListSpecification : BaseSpecification<Genre>
    {
        public GenreFiltredListSpecification(
            string? searchTerm,
            int? pageNumber,
            int? pageSize)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Criteria = g =>
                    (g.Name).ToLower().Contains(searchTerm.ToLower());
            }

            ApplyOrderBy(g => g.Id);

            if (pageNumber.HasValue && pageSize.HasValue)
                ApplyPaging(
                    (pageNumber.Value - 1) * pageSize.Value,
                    pageSize.Value
                );
        }
    }
}
