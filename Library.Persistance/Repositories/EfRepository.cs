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

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T>? query = _entities.AsQueryable();
            if (includesProperties.Any())
            {
                foreach (Expression<Func<T, object>>? included in
               includesProperties)
                {
                    query = query.Include(included);
                }
            }
            return await query.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
        }
        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<T>? query = _entities.AsQueryable();

            return await query.ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T>? query = _entities.AsQueryable();
            if (includesProperties.Any())
            {
                foreach (Expression<Func<T, object>>? included in
               includesProperties)
                {
                    query = query.Include(included);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync(cancellationToken);
        }

        public IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T> query = _entities.AsQueryable();

            if (includesProperties?.Any() == true)
            {
                foreach (var included in includesProperties)
                {
                    query = query.Include(included);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public T Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var entry = _entities.Add(entity);
            return entry.Entity;
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T>? query = _entities.AsQueryable();

            return query.FirstOrDefaultAsync(filter,cancellationToken);
        }

    }
}
