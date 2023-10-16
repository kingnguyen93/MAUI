using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RocketPDF.Infrastructure.Services;

namespace RocketPDF.EntityFrameworkCore
{
    public class NpgsqlScopedFactory : IDbContextFactory<NpgsqlContext>
    {
        private readonly IDbContextFactory<NpgsqlContext> _pooledFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;

        public NpgsqlScopedFactory(IDbContextFactory<NpgsqlContext> pooledFactory,
            IHttpContextAccessor httpContextAccessor,
            IIdentityService identityService)
        {
            _pooledFactory = pooledFactory;
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
        }

        public NpgsqlContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            context.HttpContext = _httpContextAccessor?.HttpContext;
            context.IdentityService = _identityService;
            return context;
        }
    }
}