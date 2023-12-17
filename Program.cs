using System.Text;
using BibleDotComScraper.Classes;
using Spectre.Console;
using System.Text.Json;
using BibleDotComScraper.Classes.BibleCom;
using BibleDotComScraper.Services;
using System.IO.Compression;
using LibBibleDotCom.Models;
using bdc = LibBibleDotCom;
using System.Text.Json.Serialization;

namespace BibleDotComScraper;

internal static class Program
{
    private static readonly HttpService Http = new();

    public class BagOfData
    {
        public List<InfoLanguage> Languages = new();
        public List<bdc.Models.Version> Translations = new();
        public List<VersionMetadata> Metadata = new();
    }


    static async Task Main()
    {


        //bdc.BibleDotComService.SetCacheLifespan(TimeSpan.FromDays(100));
        //var b = await bdc.BibleDotComService.GetDecodedVersion("eng", "NKJV");

        //  var t = await bdc.BibleDotComService.Tokenize("This is a test");
        //  var t2 = await bdc.BibleDotComService.Tokenize("This is a test<b>Hellotestworld</b>");
        //  var t1 = await bdc.BibleDotComService.Tokenize("<b>Hello<b>test</b>world</b>");
        // var t3 = await bdc.BibleDotComService.Tokenize("<div class=\"version vid114 iso6393eng\" data-vid=\"114\" data-iso6393=\"eng\"><div class=\"book bk1JN\"></div></div>");
        var t4 = await bdc.BibleDotComService.Tokenize("<div class=\"version vid114 iso6393eng\" data-vid=\"114\" data-iso6393=\"eng\"> <div class=\"book bk1JN\"> <div class=\"chapter ch1 frag37090\" data-usfm=\"1JN.1\"><div class=\"label\">1</div><div class=\"s\"><span class=\"heading\">What Was Heard, Seen, and Touched</span></div> <div class=\"p\"><span class=\"verse v1\" data-usfm=\"1JN.1.1\"><span class=\"label\">1</span><span class=\"content\">That </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 1:1); 1 John 2:13, 14</span></span><span class=\"content\">which was from the beginning, which we have heard, which we have </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Luke 1:2; John 1:14</span></span><span class=\"content\">seen with our eyes, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Pet. 1:16</span></span><span class=\"content\">which we have looked upon, and </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Luke 24:39; John 20:27</span></span><span class=\"content\">our hands have handled, concerning the </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 1:1, 4, 14)</span></span><span class=\"content\">Word of life— </span></span><span class=\"verse v2\" data-usfm=\"1JN.1.2\"><span class=\"label\">2</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 1:4; (1 John 3:5, 8; 5:20)</span></span><span class=\"content\">the life </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Rom. 16:26; 1 Tim. 3:16</span></span><span class=\"content\">was manifested, and we have seen, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 21:24</span></span><span class=\"content\">and bear witness, and declare to you that eternal life which was </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 1:1, 18; 16:28)</span></span><span class=\"content\">with the Father and was manifested to us— </span></span><span class=\"verse v3\" data-usfm=\"1JN.1.3\"><span class=\"label\">3</span><span class=\"content\">that which we have seen and heard we declare to you, that you also may have fellowship with us; and truly our fellowship </span><span class=\"it\"><span class=\"content\">is</span></span><span class=\"content\"> </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 17:21; 1 Cor. 1:9; 1 John 2:24</span></span><span class=\"content\">with the Father and with His Son Jesus Christ. </span></span><span class=\"verse v4\" data-usfm=\"1JN.1.4\"><span class=\"label\">4</span><span class=\"content\">And these things we write to you </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 15:11; 16:24; 1 Pet. 1:8</span></span><span class=\"content\">that your joy may be full.</span></span></div> <div class=\"s\"><span class=\"heading\">Fellowship with Him and One Another</span></div> <div class=\"p\"><span class=\"verse v5\" data-usfm=\"1JN.1.5\"><span class=\"label\">5</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 1:19; 1 John 3:11</span></span><span class=\"content\">This is the message which we have heard from Him and declare to you, that </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(1 Tim. 6:16); James 1:17</span></span><span class=\"content\">God is light and in Him is no darkness at all. </span></span><span class=\"verse v6\" data-usfm=\"1JN.1.6\"><span class=\"label\">6</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 8:12); 2 Cor. 6:14; (1 John 2:9–11)</span></span><span class=\"content\">If we say that we have fellowship with Him, and walk in darkness, we lie and do not practice the truth. </span></span><span class=\"verse v7\" data-usfm=\"1JN.1.7\"><span class=\"label\">7</span><span class=\"content\">But if we </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Is. 2:5</span></span><span class=\"content\">walk in the light as He is in the light, we have fellowship with one another, and </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(1 Cor. 6:11)</span></span><span class=\"content\">the blood of Jesus Christ His Son cleanses us from all sin.</span></span></div> <div class=\"p\"><span class=\"verse v8\" data-usfm=\"1JN.1.8\"><span class=\"label\">8</span><span class=\"content\">If we say that we have no sin, we deceive ourselves, and the truth is not in us. </span></span><span class=\"verse v9\" data-usfm=\"1JN.1.9\"><span class=\"label\">9</span><span class=\"content\">If we </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Ps. 32:5; Prov. 28:13</span></span><span class=\"content\">confess our sins, He is </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(Rom. 3:24–26)</span></span><span class=\"content\">faithful and just to forgive us </span><span class=\"it\"><span class=\"content\">our</span></span><span class=\"content\"> sins and to </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Ps. 51:2</span></span><span class=\"content\">cleanse us from all unrighteousness. </span></span><span class=\"verse v10\" data-usfm=\"1JN.1.10\"><span class=\"label\">10</span><span class=\"content\">If we say that we have not sinned, we </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 3:33; 1 John 5:10</span></span><span class=\"content\">make Him a liar, and His word is not in us.</span></span></div></div></div></div>");

        Console.WriteLine(t4.ToXml());
        return;


        //Console.OutputEncoding = Encoding.UTF8;
        //Console.InputEncoding = Encoding.UTF8;
        //AnsiConsole.MarkupLine("[underline red]bible.com[/] download");

        //// Get the language that we're going to retrieve.
        //int overwriteLine = Console.CursorTop;
        //List<Classes.Language> languagesa = GetLanguages();
        //Classes.Language language = GetLanguage(languagesa);
        //EraseToLineAndPrintStatus(overwriteLine,
        //    $"[green]{language.Name}[/] selected.");

        //// Get the translation that we're going to retrieve.
        //overwriteLine = Console.CursorTop;
        //List<Translation> translations = GetTranslations(language);
        //Translation translation = GetTranslation(translations);
        //EraseToLineAndPrintStatus(overwriteLine,
        //    $"[green]{translation.Name}[/] selected.");

        //// Download the content.
        //overwriteLine = Console.CursorTop;
        //string tempFile = await DownloadOfflineTranslation(translation);
        //EraseToLineAndPrintStatus(overwriteLine,
        //    $"Downloaded [green]{translation.Name}[/] from [red link]http:{translation.Url}[/] to [red link]file://{tempFile}[/].");

        //// Extract and decode source files
        //overwriteLine = Console.CursorTop;
        //string tempPath = await ExtractAndDecodeFiles(tempFile);
        //EraseToLineAndPrintStatus(overwriteLine,
        //    $"Extracted and Decoded [green]{translation.Name}[/] to [red link]file://{tempPath}[/].");

        //// Delete the temporary file
        //File.Delete(tempFile);

        //// Get the meta data from Bible.com for this archive

        //// Parse the files and build the data

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