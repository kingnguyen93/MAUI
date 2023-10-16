using System.Linq.Expressions;

namespace RocketPDF.Infrastructure.Specifications
{
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        protected SpecificationBase(Expression<Func<T, bool>> criteria = default)
        {
            Criteria = criteria;
            Conditions = new List<Expression<Func<T, bool>>>();
            Includes = new List<Expression<Func<T, object>>>();
            IncludeStrings = new List<string>();
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, bool>>> Conditions { get; }
        public List<Expression<Func<T, object>>> Includes { get; }
        public List<string> IncludeStrings { get; }
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        protected virtual void AddCondition(Expression<Func<T, bool>> conditionExpression)
        {
            Conditions.Add(conditionExpression);
        }

        protected virtual void AddConditionIf(bool condition, Expression<Func<T, bool>> conditionExpression)
        {
            if (condition)
            {
                Conditions.Add(conditionExpression);
            }
        }

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected virtual void AddSkipTake(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyPaging(int pageIndex, int pageSize)
        {
            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }
    }
}