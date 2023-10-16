using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RocketPDF.Domain.Entities;
using RocketPDF.EntityFrameworkCore.Helpers;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Specifications;
using System.Linq.Expressions;

namespace RocketPDF.EntityFrameworkCore.Repositories
{
    public class QueryableRepository<TEntity, TKey> : QueryableRepository<TEntity>, IQueryableRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
        where TKey : IComparable<TKey>
    {
        public QueryableRepository(NpgsqlContext context) : base(context)
        {
        }

        public TEntity? Get(TKey id)
        {
            return DbSet.Where(x => x.Id.Equals(id)).FirstOrDefault();
        }

        public async ValueTask<TEntity?> GetAsync(TKey id)
        {
            return await DbSet.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async ValueTask<bool> ExistAsync(TKey id)
        {
            return await DbSet.AsNoTracking().Where(x => x.Id.Equals(id)).AnyAsync();
        }

        public async ValueTask<int> DeleteAsync(TKey id)
        {
            return await DbSet.AsNoTracking().Where(x => x.Id.Equals(id)).ExecuteDeleteAsync();
        }
    }

    public class QueryableRepository<TEntity> : Repository<TEntity>, IQueryableRepository<TEntity>
        where TEntity : Entity
    {
        public IUnitOfWork UnitOfWork => context;

        public QueryableRepository(NpgsqlContext context) : base(context)
        {
        }

        public IEnumerable<string>? GetPrimaryKeys()
        {
            return DbSet.EntityType.FindPrimaryKey()?.Properties.Select(p => p.Name);
        }

        public virtual IQueryable<TEntity> GetQueryable()
        {
            return GetQueryableIncluding();
        }

        public virtual IQueryable<TEntity> GetQueryableIncluding(
            params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = DbSet.AsQueryable();

            if (propertySelectors == null || propertySelectors.Length == 0)
            {
                return query;
            }

            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }

            return query;
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAllAsync(int skip, int take)
        {
            return await DbSet.Skip(skip).Take(take).ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take)
        {
            return await DbSet.Where(predicate).Skip(skip).Take(take).ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var inc = DbSet.Include(includes[0]);
            for (int i = 1; i < includes.Length; i++)
            {
                inc = inc.Include(includes[i]);
            }
            return await inc.ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(int skip, int take, params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes.Length == 0)
                return await DbSet.ToListAsync();
            var inc = DbSet.Include(includes[0]);
            for (int i = 1; i < includes.Length; i++)
            {
                inc = inc.Include(includes[i]);
            }
            return await inc.Skip(skip).Take(take).ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var inc = DbSet.Include(includes[0]);
            for (int i = 1; i < includes.Length; i++)
            {
                inc = inc.Include(includes[i]);
            }
            return await inc.Where(predicate).ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TEntity>> ListAndIncludeAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, params Expression<Func<TEntity, object>>[] includes)
        {
            var inc = DbSet.Include(includes[0]);
            for (int i = 1; i < includes.Length; i++)
            {
                inc = inc.Include(includes[i]);
            }
            return await inc.Where(predicate).Skip(skip).Take(take).ToListAsync();
        }

        public ValueTask<TEntity?> FindAsync(params object[] ids)
        {
            return DbSet.FindAsync(ids);
        }

        public async ValueTask<TEntity?> FindAsync(IDictionary<string, object?> primaryKeys)
        {
            return await DbSet.Where(ExpressionHelper.CreateFilterExpression<TEntity>(BuildPrimaryKeyFilters(primaryKeys))).FirstOrDefaultAsync();
        }

        public async ValueTask<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync();
        }

        public async ValueTask<TEntity?> FindAsync(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async ValueTask<TEntity?> FindAndIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var inc = DbSet.Include(includes[0]);
            for (int i = 1; i < includes.Length; i++)
            {
                inc = inc.Include(includes[i]);
            }
            return await inc.Where(predicate).FirstOrDefaultAsync();
        }

        public async ValueTask<TEntity> AddAsync(TEntity entity)
        {
            var entityEntry = await DbSet.AddAsync(entity);
            return entityEntry.Entity;
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return DbSet.AddRangeAsync(entities);
        }

        public async ValueTask<long> CountAsync()
        {
            return await DbSet.AsNoTracking().LongCountAsync();
        }

        public async ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).LongCountAsync();
        }

        public async ValueTask<long> CountAsync(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).AsNoTracking().LongCountAsync();
        }

        public async ValueTask<bool> ExistAsync(params object[] ids)
        {
            var primaryKey = DbSet.EntityType.FindPrimaryKey()!;
            return await DbSet.AsNoTracking().Where(ExpressionHelper.CreateIdsExpression<TEntity>(primaryKey.Properties, ids)).AnyAsync();
        }

        public async ValueTask<bool> AnyAsync(IDictionary<string, object?> primaryKeys)
        {
            return await DbSet.AsNoTracking().Where(ExpressionHelper.CreateFilterExpression<TEntity>(BuildPrimaryKeyFilters(primaryKeys))).AnyAsync();
        }

        public async ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).AnyAsync();
        }

        public async ValueTask<bool> AnyAsync(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).AsNoTracking().AnyAsync();
        }

        public async ValueTask<int> DeleteAsync(params object[] ids)
        {
            var primaryKey = DbSet.EntityType.FindPrimaryKey()!;
            return await DbSet.AsNoTracking().Where(ExpressionHelper.CreateIdsExpression<TEntity>(primaryKey.Properties, ids)).ExecuteDeleteAsync();
        }

        public async ValueTask<int> DeleteAsync(IDictionary<string, object?> primaryKeys)
        {
            return await DbSet.AsNoTracking().Where(ExpressionHelper.CreateFilterExpression<TEntity>(BuildPrimaryKeyFilters(primaryKeys))).ExecuteDeleteAsync();
        }

        public async ValueTask<int> DeleteAsync(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).ExecuteDeleteAsync();
        }

        public async ValueTask<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ExecuteDeleteAsync();
        }

        protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(DbSet.AsQueryable(), specification);
        }

        protected IEnumerable<KeyValuePair<string, object?>> BuildPrimaryKeyFilters(IDictionary<string, object?> primaryKeys)
        {
            var primaryKey = DbSet.EntityType.FindPrimaryKey()!;
            foreach (var kvp in primaryKeys)
            {
                var key = primaryKey.Properties.FirstOrDefault(p => p.Name == kvp.Key);
                if (key != null)
                {
                    yield return new KeyValuePair<string, object?>(kvp.Key, GetTypeValue(key, kvp.Value));
                }
            }
        }

        protected object? GetTypeValue(IReadOnlyProperty property, object? value)
        {
            if (value == null)
            {
                // throw new ArgumentNullException(nameof(value), "Primary key cannot be null");
                return value;
            }
            if (property.ClrType == value.GetType())
            {
                return value;
            }
            if (property.ClrType == typeof(short) || property.ClrType == typeof(short?))
            {
                return Convert.ToInt16(value);
            }
            if (property.ClrType == typeof(int) || property.ClrType == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            if (property.ClrType == typeof(long) || property.ClrType == typeof(long?))
            {
                return Convert.ToInt64(value);
            }
            if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
            {
                return Convert.ToDecimal(value);
            }
            if (property.ClrType == typeof(DateTime))
            {
                return DateTime.Parse(value.ToString()!);
            }
            if (property.ClrType == typeof(DateOnly))
            {
                return DateOnly.Parse(value.ToString()!);
            }
            if (property.ClrType == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value.ToString()!);
            }
            if (property.ClrType == typeof(TimeOnly))
            {
                return TimeOnly.Parse(value.ToString()!);
            }
            return value;
        }
    }
}