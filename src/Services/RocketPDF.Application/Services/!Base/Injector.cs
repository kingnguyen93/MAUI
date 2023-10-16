using Microsoft.AspNetCore.Http;
using RocketPDF.Infrastructure.Mapper;
using RocketPDF.Infrastructure.Services;
using RocketPDF.Infrastructure.Threading;

namespace RocketPDF.Application.Services
{
    public interface IInjector
    {
        HttpContext? HttpContext { get; }
        ICacheService CacheService { get; }
        ILockService LockService { get; }
        IObjectMapper ObjectMapper { get; }
        ICancellationTokenProvider CancellationTokenProvider { get; }
    }

    public class Injector : IInjector
    {
        public HttpContext? HttpContext { get; }
        public ICacheService CacheService { get; }
        public ILockService LockService { get; }
        public IObjectMapper ObjectMapper { get; }
        public ICancellationTokenProvider CancellationTokenProvider { get; }

        public Injector(IHttpContextAccessor httpContextAccessor, ICacheService cacheService, ILockService lockService,
            IObjectMapper objectMapper, ICancellationTokenProvider cancellationTokenProvider)
        {
            HttpContext = httpContextAccessor?.HttpContext;
            CacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            LockService = lockService ?? throw new ArgumentNullException(nameof(lockService));
            ObjectMapper = objectMapper ?? throw new ArgumentNullException(nameof(objectMapper));
            CancellationTokenProvider = cancellationTokenProvider ?? throw new ArgumentNullException(nameof(cancellationTokenProvider));
        }
    }
}