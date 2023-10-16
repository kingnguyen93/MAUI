using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace RocketPDF.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;

        private readonly CacheSettings settings;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> Keys = new();

        public MemoryCacheService(IMemoryCache memoryCache, IOptions<CacheSettings> settings)
        {
            this.settings = settings.Value;
            this.memoryCache = memoryCache;
        }

        private string GenerateKey(params object[] keys)
        {
            return string.Join("__", keys);
        }

        public TItem? GetOrCreate<TItem>(Func<TItem> factory, params object[] keys)
        {
            if (keys.Length == 0)
            {
                return factory.Invoke();
            }
            else
            {
                string key = GenerateKey(keys);

                if (!Keys.ContainsKey(key))
                {
                    memoryCache.Remove(key);
                }

                return memoryCache.GetOrCreate(key, _ =>
                {
                    var @lock = Keys.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

                    @lock.Wait();

                    try
                    {
                        return memoryCache.GetOrCreate(key, entry =>
                        {
                            entry.SlidingExpiration = TimeSpan.FromMinutes(settings.Ttl);
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(settings.Mttl);
                            entry.RegisterPostEvictionCallback(DependentEvictionCallback, this);
                            return factory.Invoke();
                        });
                    }
                    finally
                    {
                        @lock.Release();
                    }
                });
            }
        }

        public async ValueTask<TItem?> GetOrCreateIfAsync<TItem>(bool condition, Func<ValueTask<TItem>> factory, params object[] keys)
        {
            if (condition)
                return await GetOrCreateAsync(factory, keys);
            return await factory.Invoke();
        }

        public async ValueTask<TItem?> GetOrCreateAsync<TItem>(Func<ValueTask<TItem>> factory, params object[] keys)
        {
            if (keys.Length == 0)
            {
                return await factory.Invoke();
            }
            else
            {
                string key = GenerateKey(keys);

                if (!Keys.ContainsKey(key))
                {
                    memoryCache.Remove(key);
                }

                return await memoryCache.GetOrCreateAsync(key, async _ =>
                {
                    var @lock = Keys.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

                    await @lock.WaitAsync();

                    try
                    {
                        return await memoryCache.GetOrCreateAsync(key, async entry =>
                        {
                            entry.SlidingExpiration = TimeSpan.FromMinutes(settings.Ttl);
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(settings.Mttl);
                            entry.RegisterPostEvictionCallback(DependentEvictionCallback, this);
                            return await factory.Invoke();
                        });
                    }
                    finally
                    {
                        @lock.Release();
                    }
                });
            }
        }

        private void DependentEvictionCallback(object key, object? value, EvictionReason reason, object? state)
        {
            // RemoveStartwith((string)key);
        }

        public void Remove(params object[] keys)
        {
            if (keys.Length == 0)
                return;

            string key = GenerateKey(keys);

            RemoveCache(key);

            foreach (var item in Keys.Where(k => k.Key.StartsWith(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                RemoveCache(item.Key);
            }
        }

        public void RemoveStartwith(params object[] keys)
        {
            string key = GenerateKey(keys);

            foreach (var item in Keys.Where(k => k.Key.StartsWith(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                RemoveCache(item.Key);
            }
        }

        public void RemoveContains(params object[] keys)
        {
            string key = GenerateKey(keys);

            foreach (var item in Keys.Where(k => k.Key.Contains(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                RemoveCache(item.Key);
            }
        }

        public void RemoveAll()
        {
            foreach (var item in Keys)
            {
                RemoveCache(item.Key);
            }
        }

        private void RemoveCache(string key)
        {
            if (Keys.TryRemove(key, out _))
            {
                memoryCache.Remove(key);
            }
        }

        public string[] GetKeys()
        {
            return Keys.Keys.ToArray();
        }

        public IDictionary<string, object?> GetCaches()
        {
            var caches = new Dictionary<string, object?>();
            foreach (var key in Keys.Keys)
            {
                var value = memoryCache.Get(key);
                caches.Add(key, value);
            }
            return caches;
        }
    }
}