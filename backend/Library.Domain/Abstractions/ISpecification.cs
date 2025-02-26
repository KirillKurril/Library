﻿using System.Linq.Expressions;

namespace Library.Domain.Abstractions
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        List<Func<IQueryable<T>, IQueryable<object>>>? Joins { get; }
        int? Take { get; }
        int? Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
