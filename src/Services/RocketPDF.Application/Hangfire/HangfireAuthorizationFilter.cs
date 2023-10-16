using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace RocketPDF.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAsyncAuthorizationFilter
    {
        public Task<bool> AuthorizeAsync([NotNull] DashboardContext context)
        {
            return Task.FromResult(true);
        }
    }
}