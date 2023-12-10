using System.Text;
using BibleDotComScraper.Classes;
using Spectre.Console;
using System.Text.Json;
using BibleDotComScraper.Classes.BibleCom;
using BibleDotComScraper.Services;
using System.IO.Compression;
using LibBibleDotCom.Models;
using System.Security.Cryptography.X509Certificates;
using static BibleDotComScraper.Classes.BibleCom.Translations;
using LibBibleDotCom.CacheServices;
using System;

namespace BibleDotComScraper;

internal static class Program
{
    private static readonly HttpService Http = new();

    public class BallOData
    {
        public List<InfoLanguage> Languages { get; set; } = new();
        public Dictionary<string, LibBibleDotCom.Models.Version> Versions { get; set; } = new();
    }

    static async Task Main()
    {


        LibBibleDotCom.BibleDotComService.SetCacheLifespan(TimeSpan.FromDays(100));

        //BallOData ballOData = new BallOData();
        //List<InfoLanguage> languages = (await LibBibleDotCom.BibleDotComService.GetAllLanguages());
        //ballOData.Languages = languages;
        //for (int i = 0; i < languages.Count; i++)
        //{
        //    System.Diagnostics.Debug.WriteLine(i);
        //    string languageCode = languages[i].Iso639_3;
        //    List<LibBibleDotCom.Models.Version> versions = await LibBibleDotCom.BibleDotComService.GetVersions(languageCode);
        //    foreach (LibBibleDotCom.Models.Version version in versions)
        //    {
        //        //if (ballOData.Versions.ContainsKey(version.LocalAbbreviation))
        //        //{
        //        //    Console.WriteLine($"Can't add {version.LocalAbbreviation}");
        //        //} else
        //        //{
        //        ballOData.Versions.Add($"{languages[i].Tag}|{version.Id}", version);
        //        //}
        //    }
        //}


        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string cachePath = Path.Combine(appData, "LibBibleDotCom");
        FileSystemCache cache = new(cachePath);

        //cache.SetCache("all", ballOData, TimeSpan.FromDays(100));

        BallOData ballOData = cache.GetCached<BallOData>("all");
        int i = 0;

        static string GetFileNameFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(new Uri("http://canbeanything"), url);

            return Path.GetFileName(uri.LocalPath);
        }


        foreach (LibBibleDotCom.Models.Version version in ballOData.Versions.Values)
        {
            i++;
            Console.WriteLine($"{i}: {version.LocalAbbreviation}");
            HttpClient httpClient = new();
            await httpClient.GetByteArrayAsync(version.OfflineInfo.Url).ContinueWith(async (task) =>
            {

                byte[] bytes = await task;
                string fileName = GetFileNameFromUrl(version.OfflineInfo.Url.ToString());
                await File.WriteAllBytesAsync(Path.Combine(cachePath, fileName), bytes);
            });
        }

        return;


        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        AnsiConsole.MarkupLine("[underline red]bible.com[/] download");

        // Get the language that we're going to retrieve.
        int overwriteLine = Console.CursorTop;
        List<Classes.Language> languagesa = GetLanguages();
        Classes.Language language = GetLanguage(languagesa);
        EraseToLineAndPrintStatus(overwriteLine,
            $"[green]{language.Name}[/] selected.");

        // Get the translation that we're going to retrieve.
        overwriteLine = Console.CursorTop;
        List<Translation> translations = GetTranslations(language);
        Translation translation = GetTranslation(translations);
        EraseToLineAndPrintStatus(overwriteLine,
            $"[green]{translation.Name}[/] selected.");

        // Download the content.
        overwriteLine = Console.CursorTop;
        string tempFile = await DownloadOfflineTranslation(translation);
        EraseToLineAndPrintStatus(overwriteLine,
            $"Downloaded [green]{translation.Name}[/] from [red link]http:{translation.Url}[/] to [red link]file://{tempFile}[/].");

        // Extract and decode source files
        overwriteLine = Console.CursorTop;
        string tempPath = await ExtractAndDecodeFiles(tempFile);
        EraseToLineAndPrintStatus(overwriteLine,
            $"Extracted and Decoded [green]{translation.Name}[/] to [red link]file://{tempPath}[/].");

        // Delete the temporary file
        File.Delete(tempFile);

        // Get the meta data from Bible.com for this archive

        // Parse the files and build the data

    }

    private static async Task<string> ExtractAndDecodeFiles(string tempFile)
    {
        using ZipArchive archive = ZipFile.OpenRead(tempFile);
        string tempPath = GetTemporaryDirectory();

        uint filesInArchive = (uint)archive.Entries.Count;

        await AnsiConsole.Progress()
            .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new RemainingTimeColumn(), new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                ProgressTask task1 = ctx.AddTask("Extracting/Decoding", true, filesInArchive);
                int i = 0;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    i++;
                    task1.Value = i;

                    await using Stream stream = entry.Open();
                    int bytesToRead = (int)entry.Length;
                    byte[] buffer = new byte[bytesToRead];
                    _ = await stream.ReadAsync(buffer.AsMemory(0, bytesToRead));
                    string decodedContent = DecodeYves(buffer);

                    // get the path from the archive entry
                    string entryPath = entry.FullName.Split('/')[0];
                    string extractPath = Path.Combine(tempPath, entryPath);
                    Directory.CreateDirectory(extractPath);
                    string fileName = entry.FullName.Split('/')[1].Replace("yves", "html");
                    string fullPath = Path.Combine(extractPath, fileName);
                    await File.WriteAllTextAsync(fullPath, decodedContent);
                }
            });

        return tempPath;

        static string DecodeYves(IReadOnlyList<byte> arrayOfByte)
        {
            int i = 0;
            List<byte> byteArray = new();
            while (i < arrayOfByte.Count)
            {
                if (arrayOfByte.Count > i + 1)
                {
                    byteArray.Add((byte)(((0xFF & arrayOfByte[i + 1]) >> 5) | ((0xFF & arrayOfByte[i + 1]) << 3)));
                    byteArray.Add((byte)(((0xFF & arrayOfByte[i]) >> 5) | ((0xFF & arrayOfByte[i]) << 3)));
                }
                else
                {
                    byteArray.Add((byte)(((0xFF & arrayOfByte[i]) >> 5) | ((0xFF & arrayOfByte[i]) << 3)));
                }
                i += 2;
            }
            return Encoding.UTF8.GetString(byteArray.ToArray());
        }

        static string GetTemporaryDirectory()
        {
            string tempFolder = Path.GetTempFileName().Split('.')[0];
            File.Delete(tempFolder);
            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }
    }

    private static void EraseToLineAndPrintStatus(int lineToRevertTo, string lineToPrint)
    {
        while (Console.CursorTop > lineToRevertTo + 1)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            ClearCurrentConsoleLine();
        }

        AnsiConsole.MarkupLine(lineToPrint);
        return;

        static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }

    private static async Task<string> DownloadOfflineTranslation(Translation translation)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"Downloading [green]{translation.Name}[/] from [red link]http:{translation.Url}[/]");

        string temporaryFilePath = Path.GetTempFileName().Replace(".tmp", ".zip");

        await AnsiConsole.Progress()
            .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new RemainingTimeColumn(), new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                await Http.Download(
                    temporaryFilePath,
                    ctx.AddTask(translation.Name, new ProgressTaskSettings()),
                    "http:" + translation.Url);
            });

        return temporaryFilePath;
    }

    private static List<Translation> GetTranslations(Classes.Language language)
    {

        List<Translation>? result = null;
        AnsiConsole.Status()
            .Start($"Retrieving list of translations available for [green]{language.Name}[/]", _ =>
            {
                string translationHtml = Http.GetPage($"https://www.bible.com/api/bible/versions?language_tag={language.Code}&type=all");
                Translations? rawTranslations = JsonSerializer.Deserialize<Translations>(translationHtml);
                result = rawTranslations!.response.data.versions.Select(f => new Translation(f.abbreviation, f.local_title, f.offline.url)).ToList();
            });

        if (result == null)
        {
            throw new InvalidDataException("We couldn't get a list of translations available in that language.");
        }
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"We retrieved [green]{result.Count}[/] translations available in [green]{language.Name}[/]!");
        AnsiConsole.WriteLine();

        return result;
    }

    private static Translation GetTranslation(List<Translation> translations)
    {

        if (translations.Count == 1) { return translations.First(); }

        SelectionPrompt<string> selectionPrompt = new()
        {
            Title = "Which translation would you like to download?",
            PageSize = 15,
            MoreChoicesText = "[grey](Move up and down to reveal more translations)[/]"
        };
        foreach (Translation translation in translations)
        {
            selectionPrompt.AddChoice($"[green]{translation.Code}[/]: {translation.Name}");
        }
        string b = AnsiConsole.Prompt(selectionPrompt)[7..];
        string desiredTranslationCode = b[..b.IndexOf('[', StringComparison.Ordinal)];
        return translations.Single(f => f.Code == desiredTranslationCode);
    }

    private static List<Classes.Language> GetLanguages()
    {
        List<Classes.Language>? result = null;

        AnsiConsole.Status()
            .Start("Retrieving available languages ...", _ =>
            {
                string languageHtml = Http.GetPage("https://www.bible.com/api/bible/configuration");
                Languages? rawLanguages = JsonSerializer.Deserialize<Languages>(languageHtml);
                result = rawLanguages!.response.data.default_versions.Select(c =>
                    new Classes.Language(c.name, c.language_tag, c.iso_639_3, c.local_name, c.total_versions)).ToList();
            });
        if (result == null)
        {
            throw new InvalidDataException("We couldn't get a list of languages.");
        }
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"We found [green]{result.Count}[/] languages!");
        AnsiConsole.WriteLine();

        return result;
    }

    private static Classes.Language GetLanguage(List<Classes.Language> languages)
    {
        bool done = false;
        List<Classes.Language> possibleMatches = new();
        while (!done)
        {
            string whatTheySaid = AnsiConsole.Ask<string>("What is the [green]language[/] you want to download?", "english").ToLower();

            // we have a string. We want to see now how many possible matches that has
            possibleMatches = languages.Where(f => f.Iso639.Equals(whatTheySaid, StringComparison.CurrentCultureIgnoreCase))
                .Union(languages.Where(f => f.Name.StartsWith(whatTheySaid, StringComparison.CurrentCultureIgnoreCase)))
                .Union(languages.Where(f => f.LocalName.StartsWith(whatTheySaid, StringComparison.CurrentCultureIgnoreCase)))
                .OrderBy(f => f.Name)
                .ToList();

            if (possibleMatches.Count == 1)
            {
                return possibleMatches.Single();
            }
            if (possibleMatches.Count != 0)
            {
                done = true;
            }
            else
            {
                AnsiConsole.MarkupLine("[red underline]Error:[/] We couldn't locate any languages that match that. Please try again!");
            }
        }

        AnsiConsole.MarkupLine($"We found [green]{possibleMatches.Count}[/] matches");

        SelectionPrompt<string> selectionPrompt = new()
        {
            Title = "Which language would you like to download?",
            PageSize = 15,
            MoreChoicesText = "[grey](Move up and down to reveal more languages)[/]"
        };
        foreach (Classes.Language code in possibleMatches)
        {
            selectionPrompt.AddChoice($"[green]{code.Code}[/]: {code.Name} [red]({code.TranslationCount})[/]");
        }

        string selection = AnsiConsole.Prompt(selectionPrompt);
        selection = selection[7..(selection.IndexOf(':') - 3)];

        return languages.Single(f => f.Code == selection);

    }

}