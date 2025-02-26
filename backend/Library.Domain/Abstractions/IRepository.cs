
namespace Library.Domain.Abstractions
{
    public interface IRepository<T> where T : BaseEntity
    {
        public IQueryable<T> ApplySpecification(ISpecification<T> spec);

        Task<T?> FirstOrDefault(
            ISpecification<T>? specification = null,
            CancellationToken cancellationToken = default
        );
        Task<IReadOnlyList<T>> GetAsync(
            ISpecification<T>? specification = null,
            CancellationToken cancellationToken = default
        );

        public Task<int> CountAsync(
            ISpecification<T> spec,
            CancellationToken cancellationToken = default);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);


    }
}
