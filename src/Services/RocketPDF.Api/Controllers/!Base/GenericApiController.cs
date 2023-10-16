using Microsoft.AspNetCore.Mvc;
using RocketPDF.Application.Services;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Models;
using System.Dynamic;

namespace RocketPDF.Api.Controllers
{
    public abstract class GenericApiController<TEntity, TKey> : GenericApiController<TEntity, TKey, BaseRequestDto, TEntity>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
    {
    }

    public abstract class GenericApiController<TEntity, TKey, TRequest> : GenericApiController<TEntity, TKey, TRequest, TEntity>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : BaseRequestDto
    {
    }

    public abstract class GenericApiController<TEntity, TKey, TRequest, TList> : GenericApiController<TEntity, TKey, TRequest, TList, TList>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : BaseRequestDto
        where TList : class, new()
    {
    }

    public abstract class GenericApiController<TEntity, TKey, TRequest, TList, TDetail> : GenericApiController<IGenericService<TEntity, TKey, TRequest, TList, TDetail>, TEntity, TKey, TRequest, TList, TDetail>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : BaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
    }

    public abstract class GenericApiController<TService, TEntity, TKey, TRequest, TList, TDetail> : GenericApiController<TService, TEntity, TKey, TRequest, TList, TDetail, TDetail>
        where TService : IGenericService<TEntity, TKey, TRequest, TList, TDetail>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : BaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
    }

    public abstract class GenericApiController<TService, TEntity, TKey, TRequest, TList, TDetail, TCreate> : ApiControllerBase
        where TService : IGenericService<TEntity, TKey, TRequest, TList, TDetail, TCreate>
        where TEntity : AuditedEntity<TKey>, new()
        where TKey : IComparable<TKey>
        where TRequest : BaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
        where TCreate : class, new()
    {
        [HttpGet]
        public virtual async Task<IActionResult> List([FromServices] TService service, [FromQuery] TRequest request)
        {
            return Ok(ApiResponseWrapper.Ok(new
            {
                Total = await service.CountAsync(request),
                Items = await service.ListAsync(request)
            }));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get([FromServices] TService service, [FromRoute] TKey id)
        {
            var result = await service.GetAsync(id);
            if (result == null)
                return NotFound(ApiResponseWrapper.NotFound());
            return Ok(ApiResponseWrapper.Ok(result));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromServices] TService service, TCreate input)
        {
            if (await service.CreateAsync(input))
                return Ok(ApiResponseWrapper.Ok(true));
            return BadRequest(ApiResponseWrapper.BadRequest(false));
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromServices] TService service, [FromRoute] TKey id, TDetail input)
        {
            await service.UpdateAsync(id, input);
            return Ok(ApiResponseWrapper.Ok(true));
        }

        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> Patch([FromServices] TService service, [FromRoute] TKey id, ExpandoObject input)
        {
            await service.PatchAsync(id, input);
            return Ok(ApiResponseWrapper.Ok(true));
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromServices] TService service, [FromRoute] TKey id)
        {
            if (await service.DeleteAsync(id))
                return Ok(ApiResponseWrapper.Ok(true));
            return BadRequest(ApiResponseWrapper.BadRequest(false));
        }
    }
}