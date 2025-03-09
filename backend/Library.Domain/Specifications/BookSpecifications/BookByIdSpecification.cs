using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookByIdSpecification : BaseSpecification<Book>
    {
        public BookByIdSpecification(Guid id)
        {
            AddCriteria(b =>  b.Id == id);

            AddInclude(b => b.Author);
            AddInclude(b => b.Genre);
        }
    }
}
