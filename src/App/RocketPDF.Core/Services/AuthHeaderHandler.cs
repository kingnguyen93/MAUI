using System.Net.Http.Headers;

namespace RocketPDF.Core.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserStore.Value.AccessToken);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}