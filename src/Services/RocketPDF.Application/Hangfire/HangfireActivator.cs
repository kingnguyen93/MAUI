using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace RocketPDF.Hangfire
{
    public class HangfireActivator : JobActivator
    {
        private readonly IServiceProvider serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override object ActivateJob(Type type)
        {
            return serviceProvider.CreateScope().ServiceProvider.GetRequiredService(type);
        }
    }
}