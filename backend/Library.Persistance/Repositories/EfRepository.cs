using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Library.Persistance.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _entities;
        public EfRepository(AppDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            var query = _entities.AsQueryable();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            query = spec.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled && spec.Skip.HasValue && spec.Take.HasValue)
                query = query.Skip(spec.Skip.Value).Take(spec.Take.Value);

            return query;
        }

        public async Task<T?> FirstOrDefault(
             ISpecification<T> specification,
             CancellationToken cancellationToken = default
         )
        {
            var query = ApplySpecification(specification);
            var item = await query.FirstOrDefaultAsync(cancellationToken);
            return item;
        }
        public async Task<IReadOnlyList<T>> GetAsync(
             ISpecification<T> specification,
             CancellationToken cancellationToken = default
         )
        {
            var query = ApplySpecification(specification);
            var items = await query.ToListAsync(cancellationToken);
            return items;
        }

        public async Task<int> CountAsync(
            ISpecification<T> specification,
            CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.CountAsync(cancellationToken);
        }
        public T Add(T entity)
        {
            var entry = _entities.Add(entity);
            return entry.Entity;
        }

        public void Update(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }
    }
}
