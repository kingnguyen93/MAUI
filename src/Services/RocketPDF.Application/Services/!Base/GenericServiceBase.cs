using Microsoft.EntityFrameworkCore;
using RocketPDF.Domain.Entities;
using RocketPDF.EntityFrameworkCore.Extensions;
using RocketPDF.Infrastructure.Common;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Services;

namespace RocketPDF.Application.Services
{
    public interface IGenericServiceBase<TRequest, TList>
        where TRequest : IBaseRequestDto
        where TList : class, new()
    {
        ValueTask<IReadOnlyList<TList>?> ListAllAsync();

        ValueTask<IReadOnlyList<TEntityDto>?> ListAllAsync<TEntityDto>() where TEntityDto : class, new();

        ValueTask<IReadOnlyList<TList>?> ListAsync(TRequest request);

        ValueTask<IReadOnlyList<TEntityDto>?> ListAsync<TEntityDto>(TRequest request) where TEntityDto : class, new();

        ValueTask<IReadOnlyList<TList>?> SearchAsync(TRequest request);

        ValueTask<IReadOnlyList<TEntityDto>?> SearchAsync<TEntityDto>(TRequest request) where TEntityDto : class, new();

        ValueTask<long> CountAsync();

        ValueTask<long> CountAsync(TRequest request);
    }

    public abstract class GenericServiceBase<TRepository, TEntity, TRequest, TList> : ApplicationService, IGenericServiceBase<TRequest, TList>
        where TRepository : IQueryableRepository<TEntity>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
    {
        protected virtual bool CacheEnabled { get; }

        protected virtual bool CacheTotalOnly { get; }

        protected virtual bool CacheByUser { get; }

        protected virtual string CacheKey => typeof(TEntity).Name;

        protected virtual IEnumerable<string>? ChildEntities { get; }

        protected virtual IEnumerable<string>? RelatedEntities { get; }

        protected TRepository Repository { get; }

        protected IUnitOfWork UnitOfWork => Repository.UnitOfWork;

        protected IQueryable<TEntity> QueryWithTracking => Repository.GetQueryable();

        protected IQueryable<TEntity> QueryNoTracking => Repository.GetQueryable().AsNoTracking();

        protected GenericServiceBase(IInjector injector, TRepository repository) : base(injector)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        protected virtual IQueryable<TEntity> CreateFilteredQuery(TRequest request)
        {
            return QueryNoTracking;
        }

        protected virtual IQueryable<TEntity> FilterByDate(IQueryable<TEntity> query, TRequest request)
        {
            if (request.FromDate.HasValue)
            {
                query = query.Where(c => c.CreatedDate >= request.FromDate);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(c => c.CreatedDate <= request.ToDate);
            }
            return query;
        }

        protected virtual IQueryable<TEntity> OrderQuery(IQueryable<TEntity> query, TRequest request)
        {
            return query.OrderByRequestOrNewest(request);
        }

        protected virtual IQueryable<TEntity> PagingQuery(IQueryable<TEntity> query, TRequest request)
        {
            return query.SkipTake(request);
        }

        public virtual async ValueTask<IReadOnlyList<TList>?> ListAllAsync() => await CacheService.GetOrCreateIfAsync(CacheEnabled, async () =>
        {
            var query = QueryNoTracking;
            query = query.OrderByNewest();
            return await ObjectMapper.ProjectTo<TList>(query).ToListAsync();
        }, CacheKey, CacheKeys.List);

        public virtual async ValueTask<IReadOnlyList<TEntityDto>?> ListAllAsync<TEntityDto>() where TEntityDto : class, new() => await CacheService.GetOrCreateIfAsync(CacheEnabled, async () =>
        {
            var query = QueryNoTracking.OrderByNewest();
            return await ObjectMapper.ProjectTo<TEntityDto>(query).ToListAsync();
        }, CacheKey, CacheKeys.List, typeof(TEntityDto).Name);

        public virtual async ValueTask<IReadOnlyList<TList>?> ListAsync(TRequest request) => await CacheService.GetOrCreateIfAsync(CacheEnabled, async () =>
        {
            var query = CreateFilteredQuery(request);
            query = FilterByDate(query, request);
            query = OrderQuery(query, request);
            query = PagingQuery(query, request);
            return await ObjectMapper.ProjectTo<TList>(query).ToListAsync();
        }, CacheKey, CacheKeys.List, request.ToCacheKeys());

        public virtual async ValueTask<IReadOnlyList<TEntityDto>?> ListAsync<TEntityDto>(TRequest request) where TEntityDto : class, new() => await CacheService.GetOrCreateIfAsync(CacheEnabled, async () =>
        {
            var query = CreateFilteredQuery(request);
            query = FilterByDate(query, request);
            query = OrderQuery(query, request);
            query = PagingQuery(query, request);
            return await ObjectMapper.ProjectTo<TEntityDto>(query).ToListAsync();
        }, CacheKey, CacheKeys.List, typeof(TEntityDto).Name, request.ToCacheKeys());

        public virtual async ValueTask<IReadOnlyList<TList>?> SearchAsync(TRequest request) => await CacheService.GetOrCreateIfAsync(CacheEnabled, async () =>
        {
            var query = CreateFilteredQuery(request);
            query = FilterByDate(query, request);
            query = OrderQuery(query, request);
            query = PagingQuery(query, request);
            return await ObjectMapper.ProjectTo<TList>(query).ToListAsync();
        }, CacheKey, CacheKeys.LookUp, request.ToCacheKeys());

        public virtual async ValueTask<IReadOnlyList<TEntityDto>?> SearchAsync<TEntityDto>(TRequest request) where TEntityDto : class, new() => await CacheService.GetOrCreateIfAsync(CacheEnabled, async () =>
        {
            var query = CreateFilteredQuery(request);
            query = FilterByDate(query, request);
            query = OrderQuery(query, request);
            query = PagingQuery(query, request);
            return await ObjectMapper.ProjectTo<TEntityDto>(query).ToListAsync();
        }, CacheKey, CacheKeys.LookUp, typeof(TEntityDto).Name, request.ToCacheKeys());

        public virtual async ValueTask<long> CountAsync() => await CacheService.GetOrCreateIfAsync(CacheEnabled || CacheTotalOnly, async () =>
        {
            var query = QueryNoTracking;
            return await query.LongCountAsync();
        }, CacheKey, CacheKeys.Count);

        public virtual async ValueTask<long> CountAsync(TRequest request) => await CacheService.GetOrCreateIfAsync(CacheEnabled || CacheTotalOnly, async () =>
        {
            var query = CreateFilteredQuery(request);
            query = FilterByDate(query, request);
            return await query.LongCountAsync();
        }, CacheKey, CacheKeys.Count, request.ToCacheKeys(nameof(IBaseRequestDto.PageIndex), nameof(IBaseRequestDto.PageSize)));

        public async Task<int> SaveChangesAsync()
        {
            var result = await UnitOfWork.SaveChangesAsync(CancellationToken);
            if (result > 0)
            {
                if (CacheEnabled)
                {
                    RemoveCache();
                }
                else if (CacheTotalOnly)
                {
                    RemoveCacheTotal();
                }
            }
            return result;
        }

        protected virtual void RemoveCache()
        {
            CacheService.Remove(CacheKey);
            if (RelatedEntities != null)
            {
                foreach (var relatedEntity in RelatedEntities)
                {
                    CacheService.Remove(relatedEntity);
                }
            }
        }

        protected virtual void RemoveCacheTotal()
        {
            CacheService.Remove(CacheKey, CacheKeys.Count);
        }
    }
}