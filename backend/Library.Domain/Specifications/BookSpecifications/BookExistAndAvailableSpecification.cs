using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookExistAndAvailableSpecification : BaseSpecification<Book>
    {
        public BookExistAndAvailableSpecification(Guid id)
        {
            AddCriteria(b => b.Id ==  id);
            AddCriteria(b => b.Quantity > 0);
        }
    }
}
