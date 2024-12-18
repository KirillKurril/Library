﻿using System.Linq.Expressions;

namespace Library.Domain.Abstractions
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(Guid id, CancellationToken
        cancellationToken = default,
        params Expression<Func<T, object>>[]?
        includesProperties);

        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken
        cancellationToken = default);

        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>
        filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]?
        includesProperties);

        IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[]? includesProperties);

        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[]? includesProperties);
    }
}
