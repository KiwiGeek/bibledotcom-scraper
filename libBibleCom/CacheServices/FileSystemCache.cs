using LibBibleDotCom.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibBibleDotCom.CacheServices;
public class FileSystemCache : ICacheService
{

    private string CachePath { get; }

    public FileSystemCache() : this(System.IO.Path.Combine(Path.GetTempPath(), "cache"))
    { }


    public FileSystemCache(string path)
    {
        // ensure that the folder exists
        CachePath = path;
        Directory.CreateDirectory(CachePath);
    }

    public void ClearAll()
    {
        foreach (string directory in Directory.GetDirectories(CachePath))
        {
            Directory.Delete(directory, true);
        }
    }

    public void ClearAllOfType<T>()
    {
        string type = typeof(T).ToString();
        string typePath = Path.Combine(CachePath, type);

        Directory.Delete(typePath, true);
    }

    public uint Count()
    {
        PurgeExpired();
        int count = 0;
        foreach (string directory in Directory.GetDirectories(CachePath))
        {
            count += Directory.GetFiles(directory).Length;
        }
        return Convert.ToUInt32(count);
    }

    public IEnumerable<T> GetAllCachedOfType<T>()
    {
        PurgeExpired();
        string type = typeof(T).ToString();
        string typePath = Path.Combine(CachePath, type);
        if (!Directory.Exists(typePath))
        {
            yield break;
        }
        else
        {
            foreach (string file in Directory.GetFiles(typePath))
            {
                string serializedValue = File.ReadAllText(file);
                yield return JsonSerializer.Deserialize<T>(serializedValue)!;
            }
        }

    }

    public T GetCached<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        PurgeExpired();
        string type = typeof(T).ToString();
        string typePath = Path.Combine(CachePath, type);
        if (!Directory.Exists(typePath)) { throw new Exception("Key does not exist in cache"); }
        string[] files = Directory.GetFiles(typePath);
        string? file = files.FirstOrDefault(f => f.EndsWith($".{key}")) 
            ?? throw new Exception("Key does not exist in cache");
        string fileSerialized = File.ReadAllText(file);
        return JsonSerializer.Deserialize<T>(fileSerialized)!;
    }

    public bool IsCached<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        PurgeExpired();
        string type = typeof(T).ToString();
        string typePath = Path.Combine(CachePath, type);
        if (!Directory.Exists(typePath)) { return false; }
        string[] files = Directory.GetFiles(typePath);
        return files.Any(f => f.EndsWith($".{key}"));
    }

    public void PurgeExpired(DateTime? currentTime = null)
    {
        if (currentTime == null) { currentTime = DateTime.Now; }

        foreach (string directory in Directory.GetDirectories(CachePath))
        {

            foreach (string file in Directory.GetFiles(directory))
            {
                string[] parts = Path.GetFileName(file).Split('.');
                DateTime expiresAt = DateTime.ParseExact(parts[0], "yyyyMMddHHmmss", null);
                if (expiresAt < currentTime)
                {
                    File.Delete(file);
                }
            }


        }
    }

    public void RemoveCache<T>(string key)
    {
        PurgeExpired();
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        string type = typeof(T).ToString();
        string typePath = Path.Combine(CachePath, type);

        foreach (string file in Directory.GetFiles(typePath))
        {
            string[] parts = Path.GetFileName(file).Split('.');
            if (parts[1] == key)
            {
                File.Delete(file);
            }
        }

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

        string type = typeof(T).ToString();
        string serializedValue = JsonSerializer.Serialize(value);
        string typePath = Path.Combine(CachePath, type);

        if (!Directory.Exists(typePath))
        {
            Directory.CreateDirectory(typePath);
        }

        string fileName = $"{expiresAt:yyyyMMddHHmmss}.{key}";
        File.WriteAllText(Path.Combine(typePath, fileName), serializedValue);

        PurgeExpired();
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

    public void UpdateCache<T>(string key, T value, TimeSpan? lifespan)
    {
        PurgeExpired();
        RemoveCache<T>(key);
        SetCache<T>(key, value, lifespan);
    }
}
