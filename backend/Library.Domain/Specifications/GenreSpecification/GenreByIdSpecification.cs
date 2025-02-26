using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.GenreSpecification
{
    public class GenreByIdSpecification : BaseSpecification<Genre>
    {
        public GenreByIdSpecification(Guid id) 
            => AddCriteria(g => g.Id == id);
    }
}
