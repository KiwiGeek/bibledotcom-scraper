using Spectre.Console;
using System.Net;

namespace BibleDotComScraper.Services;

class HttpService
{

    private readonly PageCacheService _pageCache = new ();

    private readonly HttpClient _client = new ();

    // todo move this to HttpClient
    public string GetPage(string url, bool ignoreCache = false)
    {

        // see if the page exists in the page cache;
        bool pageInCache = _pageCache.IsPageCached(url);
        if (pageInCache && !ignoreCache)
        {
            return _pageCache.GetPage(url);
        }

        using WebClient client = new ();
        client.Headers["User-Agent"] =
            "Mozilla / 5.0(Windows NT 10.0; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 62.0.3202.9 Safari / 537.36";
        byte[] arr = client.DownloadData(url);
        string result = System.Text.Encoding.Default.GetString(arr);
        _pageCache.AddPage(url, result);
        return result;
    }

    // This methods downloads a file and updates progress
    public async Task Download(string destinationFile, ProgressTask task, string url)
    {
        try
        {
            using HttpResponseMessage response = 
                await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            task.MaxValue(response.Content.Headers.ContentLength ?? 0);
            task.StartTask();

            await using Stream contentStream = await response.Content.ReadAsStreamAsync();
            await using FileStream fileStream = new (destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            byte[] buffer = new byte[8192];
            while (true)
            {
                int read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0) { break; }

                task.Increment(read);

                await fileStream.WriteAsync(buffer, 0, read);
            }
        }
        catch (Exception ex)
        {
            // An error occurred
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex}");
        }
    }
}