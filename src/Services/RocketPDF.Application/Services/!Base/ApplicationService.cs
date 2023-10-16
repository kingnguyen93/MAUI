using Microsoft.AspNetCore.Http;
using RocketPDF.Infrastructure.Mapper;
using RocketPDF.Infrastructure.Services;
using RocketPDF.Infrastructure.Threading;

namespace RocketPDF.Application.Services
{
    public abstract class ApplicationService
    {
        protected HttpContext? HttpContext { get; }
        protected ICacheService CacheService { get; }
        protected ILockService LockService { get; }
        protected IObjectMapper ObjectMapper { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected CancellationToken CancellationToken => CancellationTokenProvider.Token;

        protected ApplicationService(IInjector injector)
        {
            HttpContext = injector.HttpContext;
            CacheService = injector.CacheService;
            LockService = injector.LockService;
            ObjectMapper = injector.ObjectMapper;
            CancellationTokenProvider = injector.CancellationTokenProvider;
        }
    }
}