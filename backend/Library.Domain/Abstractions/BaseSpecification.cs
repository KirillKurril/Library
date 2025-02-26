using System.Linq.Expressions;

namespace Library.Domain.Abstractions
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
        public int? Take { get; protected set; }
        public int? Skip { get; protected set; }
        public bool IsPagingEnabled { get; protected set; }

        protected void AddCriteria(Expression<Func<T, bool>> newCriteria)
        {
            Criteria = Criteria == null
                ? newCriteria
                : CombineExpression(Criteria, newCriteria);
        }
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

        protected void ApplyOrderBy(Expression<Func<T, object>> orderBy)
            => OrderBy = orderBy;

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDesc)
            => OrderByDescending = orderByDesc;

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        private Expression<Func<T, bool>> CombineExpression(
           Expression<Func<T, bool>> left,
           Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftBody = ReplaceParameter(left, parameter);
            var rightBody = ReplaceParameter(right, parameter);

            var andExpression = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
        }

        private Expression ReplaceParameter(
            Expression<Func<T, bool>> expression,
            ParameterExpression newParameter)
        {
            return new ParameterReplacer(newParameter)
                .Visit(expression.Body);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression newParameter)
            {
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _newParameter;
            }
        }
    }
}
