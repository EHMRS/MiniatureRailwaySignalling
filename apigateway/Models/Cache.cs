namespace WebapiGateway.Models;

using System.Runtime.Caching;

public class Cache<T> : MemoryCache where T : class
{
    private readonly CacheItemPolicy _defaultCacheItemPolicy;

    public Cache() : base(nameof(T))
    {
        _defaultCacheItemPolicy = new CacheItemPolicy();
    }

    public void Set(string cacheKey, T cacheItem)
    {
        base.Set(cacheKey, cacheItem, _defaultCacheItemPolicy);
    }

    public T GetOrAdd(string cacheKey)
    {
        if (TryGet(cacheKey, out var returnData))
        {
            return returnData;
        }

        returnData = Activator.CreateInstance<T>();
        Set(cacheKey, returnData);

        return returnData;
    }

    public bool TryGet(string cacheKey, out T returnItem)
    {
        returnItem = (T) this[cacheKey];
        return returnItem != null;
    }

    public IEnumerable<T> GetAll()
    {
        return this.Cast<T>();
    }
}
