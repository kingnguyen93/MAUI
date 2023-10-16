using RocketPDF.Infrastructure.Models;

namespace RocketPDF.Infrastructure.Services
{
    public interface IIdentityService
    {
        UserIdentity? GetUserIdentity();
    }
}