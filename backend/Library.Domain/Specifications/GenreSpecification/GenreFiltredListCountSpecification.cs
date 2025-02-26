using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.GenreSpecification
{
    public class GenreFiltredListCountSpecification : BaseSpecification<Genre>
    {
        public GenreFiltredListCountSpecification(string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Criteria = g =>
                    (g.Name).ToLower().Contains(searchTerm.ToLower());
            }

            ApplyOrderBy(g => g.Id);
        }
    }
}
