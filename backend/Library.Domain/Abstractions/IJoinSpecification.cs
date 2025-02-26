using System.Linq.Expressions;

namespace Library.Domain.Abstractions
{
    public interface IJoinSpecification<TSource, TJoinEntity, TResult>
    {
        Expression<Func<TSource, object>> LeftKey { get; }
        Expression<Func<TJoinEntity, object>> RightKey { get; }
        Expression<Func<TSource, TJoinEntity, TResult>> ResultSelector { get; }
    }
}
