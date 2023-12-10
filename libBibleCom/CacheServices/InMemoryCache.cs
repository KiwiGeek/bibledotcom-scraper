using LibBibleDotCom.Interfaces;

namespace LibBibleDotCom.CacheServices;

internal class InMemoryCache : ICacheService
{
    private class CacheItem<T>
    {
        public required Type ItemType { get; set; }
        public required string Key { get; set; }
        public required T Value { get; set; }
        public required DateTime Expires { get; set; }
    }

    private readonly List<CacheItem<object>> cache = [];

    public void PurgeExpired(DateTime? currentTime = null)
    {
        if (currentTime == null) { currentTime = DateTime.Now; }
        cache.RemoveAll(x => x.Expires < currentTime);
    }

    public bool IsCached<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        PurgeExpired();
        return cache.Any(x => x.Key == key && x.ItemType == typeof(T));
    }

    public IEnumerable<T> GetAllCachedOfType<T>()
    {
        PurgeExpired();
        return cache.Where(x => x.ItemType == typeof(T)).Select(x => (T)x.Value);
    }

    public void SetCache<T>(string key, T value, TimeSpan? lifespan)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        if (lifespan.HasValue && lifespan.Value == TimeSpan.Zero) { throw new Exception("Lifespan cannot be zero"); }
        DateTime expiresAt = lifespan != null 
            ? DateTime.Now.Add(lifespan.Value) 
            : DateTime.MaxValue;
        if (IsCached<T>(key)) { throw new Exception("Item with that key already exists in the cache"); }
        if (value == null) { throw new Exception("Cannot cache null values"); }

        cache.Add(new CacheItem<object>()
        {
            ItemType = typeof(T),
            Key = key,
            Value = value,
            Expires = expiresAt
        });

        PurgeExpired();
    }

    public T GetCached<T>(string key)
    {
        return string.IsNullOrEmpty(key)
            ? throw new Exception("Key cannot be null or empty")
            : (T)cache.Single(x => x.Key == key && x.ItemType == typeof(T)).Value 
                ?? throw new Exception("An Item with that key and type does not exist in the cache");
    }
    public bool TryGetCached<T>(string key, out T? value)
    {
        try
        {
            value = GetCached<T>(key);
            return true;
        }
        catch (Exception)
        {
            value = default;
            return false;
        }
    }

    public void RemoveCache<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        PurgeExpired();
        cache.RemoveAll(x => x.Key == key && x.ItemType == typeof(T));
    }

    public void ClearAllOfType<T>()
    {
        PurgeExpired();
        cache.RemoveAll(x => x.ItemType == typeof(T));
    }

    public void ClearAll()
    {
        cache.Clear();
    }

    public void UpdateCache<T>(string key, T value, TimeSpan? lifespan)
    {
        RemoveCache<T>(key);
        SetCache<T>(key, value, lifespan);
    }

    public uint Count()
    {
        PurgeExpired();
        return (uint)cache.Count;
    }

}
