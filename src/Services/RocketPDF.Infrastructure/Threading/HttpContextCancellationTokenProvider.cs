using Microsoft.AspNetCore.Http;

namespace RocketPDF.Infrastructure.Threading
{
    public class HttpContextCancellationTokenProvider : ICancellationTokenProvider
    {
        public CancellationToken Token => _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCancellationTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}