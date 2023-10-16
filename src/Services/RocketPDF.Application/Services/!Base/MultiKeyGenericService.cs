using Microsoft.AspNetCore.Http;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Common;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Services;
using System.Dynamic;

namespace RocketPDF.Application.Services
{
    public interface IMultiKeyGenericService<TEntity> : IMultiKeyGenericService<TEntity, IBaseRequestDto>
        where TEntity : AuditedEntity, new()
    {
    }

    public interface IMultiKeyGenericService<TEntity, TRequest> : IMultiKeyGenericService<TEntity, TRequest, TEntity>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
    {
    }

    public interface IMultiKeyGenericService<TEntity, TRequest, TList> : IMultiKeyGenericService<TEntity, TRequest, TList, TList>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
    {
    }

    public interface IMultiKeyGenericService<TEntity, TRequest, TList, TDetail> : IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TDetail>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
    }

    public interface IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate> : IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate, TDetail>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
    {
    }

    public interface IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate, TUpdate> : IGenericServiceBase<TRequest, TList>
    where TEntity : AuditedEntity, new()
    where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
        where TUpdate : class, new()
    {
        ValueTask<TDetail?> FindAsync(IQueryCollection query);

        ValueTask<TEntityDto?> FindAsync<TEntityDto>(IQueryCollection query) where TEntityDto : class, new();

        ValueTask<bool> CreateAsync(TCreate input);

        ValueTask<bool> CreateAsync<TCreateInput>(TCreateInput input) where TCreateInput : class;

        ValueTask<bool> UpdateAsync(IQueryCollection query, TUpdate input);

        ValueTask<bool> UpdateAsync<TUpdateInput>(IQueryCollection query, TUpdateInput input) where TUpdateInput : class;

        ValueTask<bool> PatchAsync(IQueryCollection query, ExpandoObject input);

        ValueTask<bool> DeleteAsync(IQueryCollection query);
    }

    public class MultiKeyGenericService<TEntity> : MultiKeyGenericService<TEntity, IBaseRequestDto>, IMultiKeyGenericService<TEntity>
        where TEntity : AuditedEntity, new()
    {
        public MultiKeyGenericService(IInjector injector, IMapperRepository<TEntity> repository) : base(injector, repository)
        {
        }
    }

    public class MultiKeyGenericService<TEntity, TRequest> : MultiKeyGenericService<TEntity, TRequest, TEntity>, IMultiKeyGenericService<TEntity, TRequest>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
    {
        public MultiKeyGenericService(IInjector injector, IMapperRepository<TEntity> repository) : base(injector, repository)
        {
        }
    }

    public class MultiKeyGenericService<TEntity, TRequest, TList> : MultiKeyGenericService<TEntity, TRequest, TList, TList>, IMultiKeyGenericService<TEntity, TRequest, TList>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
    {
        public MultiKeyGenericService(IInjector injector, IMapperRepository<TEntity> repository) : base(injector, repository)
        {
        }
    }

    public class MultiKeyGenericService<TEntity, TRequest, TList, TDetail> : MultiKeyGenericService<TEntity, TRequest, TList, TDetail, TDetail>, IMultiKeyGenericService<TEntity, TRequest, TList, TDetail>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
        public MultiKeyGenericService(IInjector injector, IMapperRepository<TEntity> repository) : base(injector, repository)
        {
        }
    }

    public class MultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate> : MultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate, TDetail>, IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
    {
        public MultiKeyGenericService(IInjector injector, IMapperRepository<TEntity> repository) : base(injector, repository)
        {
        }
    }

    public class MultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate, TUpdate> : GenericServiceBase<IMapperRepository<TEntity>, TEntity, TRequest, TList>, IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate, TUpdate>
        where TEntity : AuditedEntity, new()
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
        where TUpdate : class, new()
    {
        public MultiKeyGenericService(IInjector injector, IMapperRepository<TEntity> repository) : base(injector, repository)
        {
        }

        public virtual ValueTask<TDetail?> FindAsync(IQueryCollection query)
        {
            var filters = BuildFilters(query);
            return CacheService.GetOrCreateIfAsync(CacheEnabled, () =>
            {
                return Repository.FindAsync<TDetail>(filters);
            }, CacheKey, CacheKeys.Get, string.Join("_", filters.Values));
        }

        public virtual ValueTask<TEntityDto?> FindAsync<TEntityDto>(IQueryCollection query) where TEntityDto : class, new()
        {
            var filters = BuildFilters(query);
            return CacheService.GetOrCreateIfAsync(CacheEnabled, () =>
            {
                return Repository.FindAsync<TEntityDto>(filters);
            }, CacheKey, CacheKeys.Get, string.Join("_", filters.Values), typeof(TEntityDto).Name);
        }

        public virtual async ValueTask<bool> CreateAsync(TCreate input)
        {
            if (!await ValidateCreateAsync(input))
                throw new BadRequestException();
            var entity = ObjectMapper.Map<TEntity>(input);
            await ExtendCreateAsync(entity);
            Repository.Add(entity);
            return await SaveChangesAsync() > 0;
        }

        public async ValueTask<bool> CreateAsync<TCreateInput>(TCreateInput input) where TCreateInput : class
        {
            var primaryKeys = GetPrimaryKeys(input);
            if (primaryKeys != null && await Repository.AnyAsync(primaryKeys))
                throw new BadRequestException($"Id existed {string.Join(",", primaryKeys.Values)}");
            var entity = ObjectMapper.Map<TEntity>(input);
            Repository.Add(entity);
            return await SaveChangesAsync() > 0;
        }

        protected virtual async Task<bool> ValidateCreateAsync(TCreate input)
        {
            var primaryKeys = GetPrimaryKeys(input);
            if (primaryKeys != null && await Repository.AnyAsync(primaryKeys))
                throw new BadRequestException($"Id existed {string.Join(",", primaryKeys.Values)}");
            return true;
        }

        protected virtual Task ExtendCreateAsync(TEntity input)
        {
            return Task.CompletedTask;
        }

        protected Dictionary<string, object?> GetPrimaryKeys(object input)
        {
            var primaryKeys = Repository.GetPrimaryKeys();
            return input.GetType().GetProperties().Where(p => primaryKeys?.Contains(p.Name) == true).ToDictionary(p => p.Name, p => p.GetValue(input));
        }

        public async ValueTask<bool> UpdateAsync(IQueryCollection query, TUpdate input)
        {
            var filters = BuildFilters(query);
            if (!await ValidateUpdateAsync(query, input))
                throw new BadRequestException();
            var entity = await Repository.FindAsync(filters) ?? throw new NotFoundException("Not found");
            ObjectMapper.Map(input, entity);
            return await SaveChangesAsync() > 0;
        }

        public virtual async ValueTask<bool> UpdateAsync<TUpdateInput>(IQueryCollection query, TUpdateInput input) where TUpdateInput : class
        {
            var filters = BuildFilters(query);
            var entity = await Repository.FindAsync(filters) ?? throw new NotFoundException("Not found");
            ObjectMapper.Map(input, entity);
            return await SaveChangesAsync() > 0;
        }

        protected virtual Task<bool> ValidateUpdateAsync(IQueryCollection query, TUpdate input)
        {
            return Task.FromResult(true);
        }

        public async ValueTask<bool> PatchAsync(IQueryCollection query, ExpandoObject input)
        {
            var filters = BuildFilters(query);
            var entity = await Repository.FindAsync(filters) ?? throw new NotFoundException("Not found");
            ObjectMapper.Map(input.ToDictionary(x => TypeHelper.GetFieldName<TEntity>(x.Key), x => x.Value?.GetTypeValue()), entity);
            return await SaveChangesAsync() > 0;
        }

        public async ValueTask<bool> DeleteAsync(IQueryCollection query)
        {
            var filters = BuildFilters(query);
            var entity = await Repository.FindAsync(filters) ?? throw new NotFoundException("Not found");
            Repository.Remove(entity);
            return await SaveChangesAsync() > 0;
        }

        protected Dictionary<string, object?> BuildFilters(IQueryCollection query)
        {
            var filters = new Dictionary<string, object?>();
            var primaryKeys = Repository.GetPrimaryKeys();
            foreach (var q in query)
            {
                var key = TypeHelper.GetFieldName<TEntity>(q.Key);
                if (primaryKeys?.Contains(key) == true)
                {
                    filters.Add(key, q.Value.FirstOrDefault()?.As<object>());
                }
            }
            return filters;
        }
    }
}