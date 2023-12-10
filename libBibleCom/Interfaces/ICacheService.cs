
namespace LibBibleDotCom.Interfaces;
public interface ICacheService
{
    bool IsCached<T>(string key);
    IEnumerable<T> GetAllCachedOfType<T>();
    void SetCache<T>(string key, T value, TimeSpan? lifespan);
    T GetCached<T>(string key);
    bool TryGetCached<T>(string key, out T? value);
    void RemoveCache<T>(string key);
    void ClearAllOfType<T>();
    void ClearAll();
    void UpdateCache<T>(string key, T value, TimeSpan? lifespan);
    uint Count();
    void PurgeExpired(DateTime? currentTime = null);
}
