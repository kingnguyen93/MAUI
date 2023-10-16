namespace RocketPDF.Infrastructure.Services
{
    public interface ICacheService
    {
        TItem? GetOrCreate<TItem>(Func<TItem> factory, params object[] keys);

        ValueTask<TItem?> GetOrCreateIfAsync<TItem>(bool condition, Func<ValueTask<TItem>> factory, params object[] keys);

        ValueTask<TItem?> GetOrCreateAsync<TItem>(Func<ValueTask<TItem>> factory, params object[] keys);

        void Remove(params object[] keys);

        void RemoveStartwith(params object[] keys);

        void RemoveContains(params object[] keys);

        void RemoveAll();

        string[] GetKeys();

        IDictionary<string, object?> GetCaches();
    }
}