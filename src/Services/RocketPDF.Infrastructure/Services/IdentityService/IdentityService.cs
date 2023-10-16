using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RocketPDF.Infrastructure.Models;

namespace RocketPDF.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpContext? httpContext;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContext = httpContextAccessor?.HttpContext;
        }

        public UserIdentity? GetUserIdentity()
        {
            if (httpContext == null)
            {
                return default;
            }
            _ = Guid.TryParse(httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            return new UserIdentity
            {
                UserId = userId,
                UserName = httpContext.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty
            };
        }
    }
}