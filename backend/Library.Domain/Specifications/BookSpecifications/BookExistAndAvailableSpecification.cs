using Library.Domain.Abstractions;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Specifications.BookSpecifications
{
    public class BookExistAndAvailableSpecification : BaseSpecification<Book>
    {
        public BookExistAndAvailableSpecification(Guid id)
        {
            AddCriteria(b => b.Id ==  id);
            AddCriteria(b => b.IsAvailable == true);
        }
    }
}
