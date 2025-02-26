using Library.Domain.Abstractions;
using Library.Domain.Entities;

namespace Library.Domain.Specifications.AuthorSpecification
{
    public class BookByIsbnSpecification : BaseSpecification<Book>
    {
        public BookByIsbnSpecification(string isbn)
        {
            AddCriteria(b => b.ISBN == isbn);

            AddInclude(b => b.Author);
            AddInclude(b => b.Genre);
        }
    }
}
