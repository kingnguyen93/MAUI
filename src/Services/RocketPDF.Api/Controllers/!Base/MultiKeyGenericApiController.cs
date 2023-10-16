using Microsoft.AspNetCore.Mvc;
using RocketPDF.Application.Services;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Models;
using System.Dynamic;

namespace RocketPDF.Api.Controllers
{
    public abstract class MultiKeyGenericApiController<TEntity> : MultiKeyGenericApiController<TEntity, BaseRequestDto, TEntity>
        where TEntity : AuditedEntity, new()
    {
    }

    public abstract class MultiKeyGenericApiController<TEntity, TRequest> : MultiKeyGenericApiController<TEntity, TRequest, TEntity>
        where TEntity : AuditedEntity, new()
        where TRequest : BaseRequestDto
    {
    }

    public abstract class MultiKeyGenericApiController<TEntity, TRequest, TList> : MultiKeyGenericApiController<TEntity, TRequest, TList, TList>
        where TEntity : AuditedEntity, new()
        where TRequest : BaseRequestDto
        where TList : class, new()
    {
    }

    public abstract class MultiKeyGenericApiController<TEntity, TRequest, TList, TDetail> : MultiKeyGenericApiController<IMultiKeyGenericService<TEntity, TRequest, TList, TDetail>, TEntity, TRequest, TList, TDetail>
        where TEntity : AuditedEntity, new()
        where TRequest : BaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
    }

    public abstract class MultiKeyGenericApiController<TService, TEntity, TRequest, TList, TDetail> : MultiKeyGenericApiController<TService, TEntity, TRequest, TList, TDetail, TDetail>
        where TService : IMultiKeyGenericService<TEntity, TRequest, TList, TDetail>
        where TEntity : AuditedEntity, new()
        where TRequest : BaseRequestDto
        where TList : class, new()
        where TDetail : class, new()
    {
    }

    public abstract class MultiKeyGenericApiController<TService, TEntity, TRequest, TList, TDetail, TCreate> : ApiControllerBase
        where TService : IMultiKeyGenericService<TEntity, TRequest, TList, TDetail, TCreate>
        where TEntity : AuditedEntity, new()
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

        [HttpGet("find")]
        public virtual async Task<IActionResult> Find([FromServices] TService service)
        {
            var result = await service.FindAsync(HttpContext.Request.Query);
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

        [HttpPut]
        public virtual async Task<IActionResult> Update([FromServices] TService service, TDetail input)
        {
            await service.UpdateAsync(HttpContext.Request.Query, input);
            return Ok(ApiResponseWrapper.Ok(true));
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromServices] TService service, ExpandoObject input)
        {
            await service.PatchAsync(HttpContext.Request.Query, input);
            return Ok(ApiResponseWrapper.Ok(true));
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromServices] TService service)
        {
            if (await service.DeleteAsync(HttpContext.Request.Query))
                return Ok(ApiResponseWrapper.Ok(true));
            return BadRequest(ApiResponseWrapper.BadRequest(false));
        }
    }
}