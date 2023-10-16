using System.Collections.Concurrent;

namespace RocketPDF.Infrastructure.Services
{
    public class LockService : ILockService
    {
        public ConcurrentDictionary<object, SemaphoreSlim> Locks { get; }

        public LockService()
        {
            Locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        }

        public Task<IAsyncDisposable> LockAsync(object key)
        {
            return Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1)).LockAsync();
        }
    }
}