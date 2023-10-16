using System.Linq.Expressions;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Specifications;

namespace RocketPDF.Infrastructure.Repositories
{
    public interface IMapperRepository<TEntity, TKey> : IMapperRepository<TEntity>, IQueryableRepository<TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        ValueTask<TResponse?> GetAsync<TResponse>(TKey id) where TResponse : class, new();
    }

    public interface IMapperRepository<TEntity> : IQueryableRepository<TEntity>
        where TEntity : IEntity
    {
        ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>() where TResponse : class, new();

        ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(int skip, int take) where TResponse : class, new();

        ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(Expression<Func<TEntity, bool>> predicate) where TResponse : class, new();

        ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(Expression<Func<TEntity, bool>> predicate, int skip, int take) where TResponse : class, new();

        ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(ISpecification<TEntity> specification) where TResponse : class, new();

        ValueTask<TResponse?> FindAsync<TResponse>(params object[] ids) where TResponse : class, new();

        ValueTask<TResponse?> FindAsync<TResponse>(IDictionary<string, object?> filters) where TResponse : class, new();

        ValueTask<TResponse?> FindAsync<TResponse>(Expression<Func<TEntity, bool>> predicate) where TResponse : class, new();

        ValueTask<TResponse?> FindAsync<TResponse>(ISpecification<TEntity> specification) where TResponse : class, new();
    }
}