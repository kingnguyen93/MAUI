using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Common;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Services;
using System.Dynamic;

namespace RocketPDF.Application.Services
{
    public interface IGenericService<TEntity, TKey> : IGenericService<TEntity, TKey, IBaseRequestDto>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
    {
    }

    public interface IGenericService<TEntity, TKey, TRequest> : IGenericService<TEntity, TKey, TRequest, TEntity>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
    {
    }

    public interface IGenericService<TEntity, TKey, TRequest, TList> : IGenericService<TEntity, TKey, TRequest, TList, TList>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
    {
    }

    public interface IGenericService<TEntity, TKey, TRequest, TList, TDetail> : IGenericService<TEntity, TKey, TRequest, TList, TDetail, TDetail>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
    }

    public interface IGenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate> : IGenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate, TDetail>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
    {
    }

    public interface IGenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate, TUpdate> : IGenericServiceBase<TRequest, TList>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
        where TUpdate : class, new()
    {
        ValueTask<TDetail?> GetAsync(TKey id);

        ValueTask<TEntityDto?> GetAsync<TEntityDto>(TKey id) where TEntityDto : class, new();

        ValueTask<bool> CreateAsync(TCreate input);

        ValueTask<bool> CreateAsync<TCreateInput>(TCreateInput input) where TCreateInput : class;

        ValueTask<bool> UpdateAsync(TKey id, TUpdate input);

        ValueTask<bool> UpdateAsync<TUpdateInput>(TKey id, TUpdateInput input) where TUpdateInput : class;

        ValueTask<bool> PatchAsync(TKey id, ExpandoObject input);

        ValueTask<bool> DeleteAsync(TKey id);
    }

    public class GenericService<TEntity, TKey> : GenericService<TEntity, TKey, IBaseRequestDto>, IGenericService<TEntity, TKey>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
    {
        public GenericService(IInjector injector, IMapperRepository<TEntity, TKey> repository) : base(injector, repository)
        {
        }
    }

    public class GenericService<TEntity, TKey, TRequest> : GenericService<TEntity, TKey, TRequest, TEntity>, IGenericService<TEntity, TKey, TRequest>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
    {
        public GenericService(IInjector injector, IMapperRepository<TEntity, TKey> repository) : base(injector, repository)
        {
        }
    }

    public class GenericService<TEntity, TKey, TRequest, TList> : GenericService<TEntity, TKey, TRequest, TList, TList>, IGenericService<TEntity, TKey, TRequest, TList>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
    {
        public GenericService(IInjector injector, IMapperRepository<TEntity, TKey> repository) : base(injector, repository)
        {
        }
    }

    public class GenericService<TEntity, TKey, TRequest, TList, TDetail> : GenericService<TEntity, TKey, TRequest, TList, TDetail, TDetail>, IGenericService<TEntity, TKey, TRequest, TList, TDetail>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
        public GenericService(IInjector injector, IMapperRepository<TEntity, TKey> repository) : base(injector, repository)
        {
        }
    }

    public class GenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate> : GenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate, TDetail>, IGenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
    {
        public GenericService(IInjector injector, IMapperRepository<TEntity, TKey> repository) : base(injector, repository)
        {
        }
    }

    public class GenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate, TUpdate> : GenericServiceBase<IMapperRepository<TEntity, TKey>, TEntity, TRequest, TList>, IGenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate, TUpdate>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : IBaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
        where TUpdate : class, new()
    {
        public GenericService(IInjector injector, IMapperRepository<TEntity, TKey> repository) : base(injector, repository)
        {
        }

        public virtual ValueTask<TDetail?> GetAsync(TKey id) => CacheService.GetOrCreateIfAsync(CacheEnabled, () =>
        {
            return Repository.GetAsync<TDetail>(id);
        }, CacheKey, CacheKeys.Get, id);

        public virtual ValueTask<TEntityDto?> GetAsync<TEntityDto>(TKey id) where TEntityDto : class, new() => CacheService.GetOrCreateIfAsync(CacheEnabled, () =>
        {
            return Repository.GetAsync<TEntityDto>(id);
        }, CacheKey, CacheKeys.Get, id, typeof(TEntityDto).Name);

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
            var entity = ObjectMapper.Map<TEntity>(input);
            Repository.Add(entity);
            return await SaveChangesAsync() > 0;
        }

        protected virtual Task<bool> ValidateCreateAsync(TCreate input)
        {
            return Task.FromResult(true);
        }

        protected virtual Task ExtendCreateAsync(TEntity input)
        {
            return Task.CompletedTask;
        }

        public virtual async ValueTask<bool> UpdateAsync(TKey id, TUpdate input)
        {
            if (!await ValidateUpdateAsync(id, input))
                throw new BadRequestException();
            var entity = await Repository.FindAsync(x => x.Id.Equals(id)) ?? throw new NotFoundException("Not found");
            ObjectMapper.Map(input, entity);
            return await SaveChangesAsync() > 0;
        }

        public virtual async ValueTask<bool> UpdateAsync<TUpdateInput>(TKey id, TUpdateInput input) where TUpdateInput : class
        {
            var entity = await Repository.FindAsync(x => x.Id.Equals(id)) ?? throw new NotFoundException("Not found");
            ObjectMapper.Map(input, entity);
            return await SaveChangesAsync() > 0;
        }

        protected virtual Task<bool> ValidateUpdateAsync(TKey id, TUpdate input)
        {
            return Task.FromResult(true);
        }

        public virtual async ValueTask<bool> PatchAsync(TKey id, ExpandoObject input)
        {
            var entity = await Repository.FindAsync(x => x.Id.Equals(id)) ?? throw new NotFoundException("Not found");
            ObjectMapper.Map(input.ToDictionary(x => TypeHelper.GetFieldName<TEntity>(x.Key), x => x.Value?.GetTypeValue()), entity);
            return await SaveChangesAsync() > 0;
        }

        public virtual async ValueTask<bool> DeleteAsync(TKey id)
        {
            var entity = await Repository.FindAsync(x => x.Id.Equals(id)) ?? throw new NotFoundException("Not found");
            Repository.Remove(entity);
            return await SaveChangesAsync() > 0;
        }
    }
}