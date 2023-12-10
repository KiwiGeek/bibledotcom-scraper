using LibBibleDotCom.Interfaces;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace LibBibleDotCom.CacheServices;
internal class SqliteCache : ICacheService
{

    public SqliteCache() : this("database.db")
    { }

    readonly SqliteConnection _sqlite;

    public SqliteCache(string database)
    {
        _sqlite = new SqliteConnection($"Data Source={database}");

        _sqlite.Open();

        SqliteCommand command = _sqlite.CreateCommand();

        command.CommandText = "CREATE TABLE IF NOT EXISTS Cache (id INTEGER PRIMARY KEY, key TEXT, [type] TEXT, value TEXT, expires DATETIME)";
        command.ExecuteNonQuery();
    }


    public void ClearAll()
    {
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "DELETE FROM Cache";
        command.ExecuteNonQuery();
    }

    public void ClearAllOfType<T>()
    {
        PurgeExpired();
        string type = typeof(T).ToString();
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "DELETE FROM Cache where [type] = @type";
        command.Parameters.AddWithValue("@type", type);
        command.ExecuteScalar();
    }

    public uint Count()
    {
        PurgeExpired();
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Cache";
        uint result = Convert.ToUInt32(command.ExecuteScalar());
        return result;
    }

    public IEnumerable<T> GetAllCachedOfType<T>()
    {
        PurgeExpired();
        string type = typeof(T).ToString();
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "SELECT value FROM Cache where [type] = @type";
        command.Parameters.AddWithValue("@type", type);
        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return JsonSerializer.Deserialize<T>(reader.GetString(0))!;
        }
    }

    public T GetCached<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        PurgeExpired();
        string type = typeof(T).ToString();
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "SELECT value FROM Cache where [type] = @type AND key = @key";
        command.Parameters.AddWithValue("@type", type);
        command.Parameters.AddWithValue("@key", key);
        string result = (string)command.ExecuteScalar()!;
        T value = JsonSerializer.Deserialize<T>(result)!;
        return value;
    }

    public bool IsCached<T>(string key)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        PurgeExpired();
        string type = typeof(T).ToString();
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Cache where [type] = @type AND key = @key";
        command.Parameters.AddWithValue("@type", type);
        command.Parameters.AddWithValue("@key", key);
        long result = (long)command.ExecuteScalar()!;
        return result >= 1;
    }

    public void RemoveCache<T>(string key)
    {
        PurgeExpired();
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        string type = typeof(T).ToString();
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "DELETE FROM Cache where [type] = @type AND key = @key";
        command.Parameters.AddWithValue("@type", type);
        command.Parameters.AddWithValue("@key", key);
        command.ExecuteScalar();
    }

    public void SetCache<T>(string key, T value, TimeSpan? lifespan = null)
    {
        if (string.IsNullOrEmpty(key)) { throw new Exception("Key cannot be null or empty"); }
        if (lifespan.HasValue && lifespan.Value == TimeSpan.Zero) { throw new Exception("Lifespan cannot be zero"); }
        DateTime expiresAt = lifespan != null
            ? DateTime.Now.Add(lifespan.Value)
            : DateTime.MaxValue;
        if (value == null) { throw new Exception("Cannot cache null values"); }
        if (IsCached<T>(key)) { throw new Exception("Item with that key already exists in the cache"); }

        string type = typeof(T).ToString();
        string serializedValue = JsonSerializer.Serialize(value);

        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "INSERT INTO Cache (key, [type], value, expires) VALUES (@key, @type, @value, @expires)";
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@type", type);
        command.Parameters.AddWithValue("@value", serializedValue);
        command.Parameters.AddWithValue("@expires", expiresAt);
        command.ExecuteNonQuery();
        PurgeExpired();
    }

    public void PurgeExpired(DateTime? currentTime = null)
    {
        if (currentTime == null) { currentTime = DateTime.Now; }
        SqliteCommand command = _sqlite.CreateCommand();
        command.CommandText = "DELETE FROM Cache WHERE expires < @now";
        command.Parameters.AddWithValue("@now", currentTime);
        command.ExecuteNonQuery();
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
