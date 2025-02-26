using Library.Domain.Abstractions;

namespace Library.Domain.Specifications
{
    public class AllItemsSpecification<T> : BaseSpecification<T>
        where T : BaseEntity
    {
        public AllItemsSpecification() { }
    }
}
