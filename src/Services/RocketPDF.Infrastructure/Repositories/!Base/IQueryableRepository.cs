using System.Linq.Expressions;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Specifications;

namespace RocketPDF.Infrastructure.Repositories
{
    public interface IQueryableRepository<TEntity, TKey> : IQueryableRepository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        ValueTask<TEntity?> GetAsync(TKey id);

        ValueTask<bool> ExistAsync(TKey id);

        ValueTask<int> DeleteAsync(TKey id);
    }

    public interface IQueryableRepository<TEntity> : IRepository<TEntity>
        where TEntity : IEntity
    {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<string>? GetPrimaryKeys();

        IQueryable<TEntity> GetQueryable();

        IQueryable<TEntity> GetQueryableIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        ValueTask<IReadOnlyList<TEntity>> ListAllAsync();

        ValueTask<IReadOnlyList<TEntity>> ListAllAsync(int skip, int take);

        ValueTask<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate);

        ValueTask<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take);

        ValueTask<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification);

        ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(params Expression<Func<TEntity, object>>[] includes);

        ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(int skip, int take, params Expression<Func<TEntity, object>>[] includes);

        ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, params Expression<Func<TEntity, object>>[] includes);

        ValueTask<TEntity?> FindAsync(params object[] ids);

        ValueTask<TEntity?> FindAsync(IDictionary<string, object?> primaryKeys);

        ValueTask<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);

        ValueTask<TEntity?> FindAsync(ISpecification<TEntity> specification);

        ValueTask<TEntity?> FindAndIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        ValueTask<TEntity> AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        ValueTask<long> CountAsync();

        ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> predicate);

        ValueTask<long> CountAsync(ISpecification<TEntity> specification);

        ValueTask<bool> ExistAsync(params object[] ids);

        ValueTask<bool> AnyAsync(IDictionary<string, object?> primaryKeys);

        ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        ValueTask<bool> AnyAsync(ISpecification<TEntity> specification);

        ValueTask<int> DeleteAsync(params object[] ids);

        ValueTask<int> DeleteAsync(IDictionary<string, object?> primaryKeys);

        ValueTask<int> DeleteAsync(ISpecification<TEntity> specification);

        ValueTask<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}