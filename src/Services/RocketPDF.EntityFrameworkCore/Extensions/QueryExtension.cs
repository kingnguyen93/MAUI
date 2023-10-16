using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Services;
using RocketPDF.Shared.Helpers;

namespace RocketPDF.EntityFrameworkCore.Extensions
{
    public static class QueryExtension
    {
        public static IOrderedQueryable<TEntity> OrderByRequest<TEntity, TRequest>(this IQueryable<TEntity> query, TRequest request)
            where TRequest : IBaseRequestDto
        {
            return !string.IsNullOrWhiteSpace(request.Sorting)
                ? query.OrderBy(GetSortString(typeof(TEntity), request.Sorting))
                : (IOrderedQueryable<TEntity>)query;
        }

        public static IOrderedQueryable<TEntity> OrderByNewest<TEntity>(this IQueryable<TEntity> query)
            where TEntity : AuditedEntity
        {
            return query.OrderByDescending(e => e.CreatedDate);
        }

        public static IOrderedQueryable<TEntity> OrderByRequestOrNewest<TEntity, TRequest>(this IQueryable<TEntity> query, TRequest request)
            where TEntity : AuditedEntity
            where TRequest : IBaseRequestDto
        {
            return !string.IsNullOrWhiteSpace(request.Sorting)
                ? query.OrderBy(GetSortString(typeof(TEntity), request.Sorting))
                : query.OrderByDescending(c => c.CreatedDate);
        }

        public static IOrderedQueryable<TEntity> OrderByLastModified<TEntity>(this IQueryable<TEntity> query)
            where TEntity : AuditedEntity
        {
            return query.OrderByDescending(e => e.UpdatedDate ?? e.CreatedDate);
        }

        public static string GetSortString<TEntity>(string input)
        {
            return GetSortString(typeof(TEntity), input);
        }

        public static string GetSortString(Type type, string input)
        {
            return string.Join(", ", GetListSortString(type, input));
        }

        public static IEnumerable<string> GetListSortString<TEntity>(string input)
        {
            return GetListSortString(typeof(TEntity), input);
        }

        public static IEnumerable<string> GetListSortString(Type type, string input)
        {
            foreach (var item in input.Split(','))
            {
                var sortString = item.Trim().Split(' ');
                var fieldName = TypeHelper.GetFieldName(type, sortString[0]);
                yield return sortString.Length == 1 ? fieldName : $"{fieldName} {sortString[1]}";
            }
        }

        public static IQueryable<TEntity> SkipTake<TEntity, TRequest>(this IQueryable<TEntity> query, TRequest request)
            where TRequest : IBaseRequestDto
        {
            return query.SkipTake(request.PageSize * (request.PageIndex - 1), request.PageSize);
        }

        public static IQueryable<T> SkipTake<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            return query == null ? throw new ArgumentNullException(nameof(query)) : query.Skip(skipCount).Take(maxResultCount);
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, bool>> whereExpression)
        {
            return condition ? query.Where(whereExpression) : query;
        }

        public static IQueryable<TSource> OrderIf<TSource, TKey>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, TKey>> orderExpression)
        {
            return condition ? query.OrderBy(orderExpression) : query;
        }

        public static IQueryable<TSource> OrderDescendingIf<TSource, TKey>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, TKey>> orderExpression)
        {
            return condition ? query.OrderByDescending(orderExpression) : query;
        }
    }
}