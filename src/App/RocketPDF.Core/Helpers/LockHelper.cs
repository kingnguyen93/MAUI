using System.Collections.Concurrent;
using RocketPDF.Core.Extensions;

namespace RocketPDF.Core.Helpers
{
    public static class LockHelper
    {
        private static ConcurrentDictionary<object, SemaphoreSlim> Locks { get; } = new ConcurrentDictionary<object, SemaphoreSlim>();

        public static Task<IAsyncDisposable> LockAsync(object key)
        {
            return Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1)).LockAsync();
        }
    }
}