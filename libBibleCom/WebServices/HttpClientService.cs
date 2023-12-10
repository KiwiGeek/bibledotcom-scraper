using LibBibleDotCom.CacheServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBibleDotCom.HttpServices;
internal static class HttpClientService
{

    static readonly SqliteCache _sqliteCacheService;

    static HttpClientService()
    {

        // build the root path for our caches
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string cachePath = Path.Combine(appData, "LibBibleDotCom");
        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }
        string cacheFile = Path.Combine(cachePath, "cache.db");

        _sqliteCacheService = new SqliteCache(cacheFile);
    }


    public static async Task<string> GetPage(string url, bool ignoreCache = false, TimeSpan? cacheLifespan = null)
    {

        if (!ignoreCache)
        {
            if (_sqliteCacheService.IsCached<string>(url)) {
                return _sqliteCacheService.GetCached<string>(url);
            }
        }

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("X-Youversion-App-Platform", "web");
        client.DefaultRequestHeaders.Add("X-Youversion-App-Version", "3");
        client.DefaultRequestHeaders.Add("X-Youversion-App-Client", "youversion");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0");
        
        string pageResult = await client.GetStringAsync(url);
        _sqliteCacheService.SetCache(url, pageResult, cacheLifespan);
        return pageResult;
    }
}
