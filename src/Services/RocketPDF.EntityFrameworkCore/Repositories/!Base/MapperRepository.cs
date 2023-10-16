using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RocketPDF.EntityFrameworkCore.Extensions;
using RocketPDF.EntityFrameworkCore.Helpers;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Specifications;
using RocketPDF.Domain.Entities;

namespace RocketPDF.EntityFrameworkCore.Repositories
{
    public class MapperRepository<TEntity, TKey> : MapperRepository<TEntity>, IMapperRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
        where TKey : IComparable<TKey>
    {
        public MapperRepository(NpgsqlContext context, IMapper mapper) : base(context, mapper)
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

        public async ValueTask<TResponse?> GetAsync<TResponse>(TKey id) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking();
            return await query
                .Where(x => x.Id.Equals(id))
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }

    public class MapperRepository<TEntity> : QueryableRepository<TEntity>, IMapperRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly IMapper mapper;

        public MapperRepository(NpgsqlContext context, IMapper mapper) : base(context)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>() where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking();
            return await query
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(int skip, int take) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking();
            return await query
                .Skip(skip)
                .Take(take)
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(Expression<Func<TEntity, bool>> predicate) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking().Where(predicate);
            return await query
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(Expression<Func<TEntity, bool>> predicate, int skip, int take) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking().Where(predicate);
            return await query
                .Skip(skip)
                .Take(take)
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async ValueTask<IReadOnlyList<TResponse>> ListAsync<TResponse>(ISpecification<TEntity> specification) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking().Specify(specification);
            return await query
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async ValueTask<TResponse?> FindAsync<TResponse>(params object[] ids) where TResponse : class, new()
        {
            var primaryKey = DbSet.EntityType.FindPrimaryKey()!;
            var query = DbSet.AsNoTracking().Where(ExpressionHelper.CreateIdsExpression<TEntity>(primaryKey.Properties, ids));
            return await query
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async ValueTask<TResponse?> FindAsync<TResponse>(IDictionary<string, object?> filters) where TResponse : class, new()
        {
            return await DbSet.AsNoTracking().Where(ExpressionHelper.CreateFilterExpression<TEntity>(BuildPrimaryKeyFilters(filters)))
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async ValueTask<TResponse?> FindAsync<TResponse>(Expression<Func<TEntity, bool>> predicate) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking().Where(predicate);
            return await query
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async ValueTask<TResponse?> FindAsync<TResponse>(ISpecification<TEntity> specification) where TResponse : class, new()
        {
            var query = DbSet.AsNoTracking().Specify(specification);
            return await query
                .ProjectTo<TResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }
}