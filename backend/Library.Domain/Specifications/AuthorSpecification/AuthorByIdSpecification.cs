using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.AuthorSpecification
{
    public class AuthorByIdSpecification : BaseSpecification<Author>
    {
        public AuthorByIdSpecification(Guid id)
            => AddCriteria(a => a.Id == id);
    }
}
