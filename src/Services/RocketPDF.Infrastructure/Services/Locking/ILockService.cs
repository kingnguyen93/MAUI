namespace RocketPDF.Infrastructure.Services
{
    public interface ILockService
    {
        Task<IAsyncDisposable> LockAsync(object key);
    }
}