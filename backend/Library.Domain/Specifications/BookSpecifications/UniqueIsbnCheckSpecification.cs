using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class UniqueIsbnCheckSpecification : BaseSpecification<Book>
    {
        public UniqueIsbnCheckSpecification(Guid id, string isbn)
        {
            AddCriteria(b => b.Id != id);
            AddCriteria(b => b.ISBN == isbn);
        }
    }
}
