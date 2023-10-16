using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RocketPDF.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected Guid UserId => Guid.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        protected string? UserName => User?.FindFirstValue(ClaimTypes.Name);
    }
}