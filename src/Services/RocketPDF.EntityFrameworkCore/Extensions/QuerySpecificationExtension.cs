using RocketPDF.Infrastructure.Specifications;

namespace RocketPDF.EntityFrameworkCore.Extensions
{
    public static class QuerySpecificationExtension
    {
        public static IQueryable<TEntity> Specify<TEntity>(this IQueryable<TEntity> query, ISpecification<TEntity> spec) where TEntity : class
        {
            return SpecificationEvaluator<TEntity>.GetQuery(query, spec);
        }
    }
}