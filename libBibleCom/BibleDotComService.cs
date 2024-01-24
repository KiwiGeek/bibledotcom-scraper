using System.Data;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using Version = LibBibleDotCom.Models.Version;

namespace LibBibleDotCom;

public static class BibleDotComService
{

    static TimeSpan _cacheLifespan = TimeSpan.FromDays(1);

    public static void SetCacheLifespan(TimeSpan cacheLifespan)
    {
        _cacheLifespan = cacheLifespan;
    }

    public static async Task<List<InfoLanguage>> GetLanguagesAsync(IProgress<int>? progress = null, CancellationToken? cancellationToken = null)
    {
        // Pulls down the current configuration page from bible.com, which contains a list of all the languages
        string languageHtml = await HttpClientService.GetPage("https://www.bible.com/api/bible/configuration", false, _cacheLifespan);
        cancellationToken?.ThrowIfCancellationRequested();
        progress?.Report(25);

        // Deserialize that page into a BibleDotComLanguages object
        BibleDotComLanguages rawLanguages = JsonSerializer.Deserialize<BibleDotComLanguages>(languageHtml)
            ?? throw new Exception("Failed to deserialize languages");
        cancellationToken?.ThrowIfCancellationRequested();
        progress?.Report(50);

        // some languages will be returned with an null Id, so we need to resolve that.
        // pulling down the versions page for that language (from the tag) will give us the id, which we need to parse
        // out and then use to update the language object.
        // Although it's not currently the case, it's plausible that at some point a language could be returned with no Id which has 
        // multiple translations. In that case, we'd need to pull down all the translations, and double-check them against the 
        // languagues list above to find the Id that's not used. This would probably never happen, but it's better to be safe than sorry.

        List<BibleDotComLanguages.DefaultVersion> languagesWithMissingIds = rawLanguages.response.data.default_versions.Where(l => l.id == null).ToList();
        int i = 0;
        int size = languagesWithMissingIds.Count;
        foreach (BibleDotComLanguages.DefaultVersion rawLanguage in rawLanguages.response.data.default_versions.Where(l => l.id == null))
        {
            List<Version> versions = await GetVersionsAsync(rawLanguage.language_tag);
            foreach (Version version in versions)
            {
                if (rawLanguages.response.data.default_versions.Any(l => l.id == version.Id))
                {
                    continue;
                }

                // this Id wasn't used in the languages list, so we can assume that it's the language that's missing the Id.
                rawLanguage.id = version.Id;
                break;
            }
            i++;
            cancellationToken?.ThrowIfCancellationRequested();
            progress?.Report((int)(50.0 + ((float)i / size * 25.0)));
        }

        return rawLanguages.response.data.default_versions
            .Select(rawLanguage => new InfoLanguage()
            {
                Iso639_1 = rawLanguage.iso_639_1,
                Iso639_3 = rawLanguage.iso_639_3,
                Tag = rawLanguage.language_tag,
                LocalName = rawLanguage.local_name,
                EnglishName = rawLanguage.name,
                TextDirection = rawLanguage.text_direction == "ltr" ? TextDirections.LeftToRight : TextDirections.RightToLeft,
                Id = (uint)rawLanguage.id!,         // we fixed any missing Ids above
                HasAudio = rawLanguage.has_audio,
                HasText = rawLanguage.has_text,
                Font = (string?)rawLanguage.font
            }).ToList();
    }

    public static async Task<List<Version>> GetVersionsAsync(string languageTag, IProgress<int>? progress = null, CancellationToken? cancellationToken = null)
    {
        // pull down the versions page for this language
        string versionHtml = await HttpClientService.GetPage($"https://www.bible.com/api/bible/versions?language_tag={languageTag}&type=all", false, _cacheLifespan);
        cancellationToken?.ThrowIfCancellationRequested();
        progress?.Report(33);

        // Deserialize that page into a BibleDotComVersions object
        BibleDotComVersions rawVersions = JsonSerializer.Deserialize<BibleDotComVersions>(versionHtml)
            ?? throw new Exception("Failed to deserialize version");
        cancellationToken?.ThrowIfCancellationRequested();
        progress?.Report(67);


        return rawVersions.response.data.versions
            .Select(rawlanguage => new Version
            {
                Id = rawlanguage.id,
                Abbreviation = rawlanguage.abbreviation,
                LocalAbbreviation = rawlanguage.local_abbreviation,
                Title = rawlanguage.title,
                LocalTitle = rawlanguage.local_title,
                HasAudio = rawlanguage.audio,
                AudioCount = rawlanguage.audio_count,
                HasText = rawlanguage.text,
                Language = new VersionLanguage
                {
                    Iso639_1 = rawlanguage.language.iso_639_1,
                    Iso639_3 = rawlanguage.language.iso_639_3,
                    Tag = rawlanguage.language.language_tag,
                    LocalName = rawlanguage.language.local_name,
                    EnglishName = rawlanguage.language.name,
                    TextDirection = rawlanguage.language.text_direction == "ltr" ? TextDirections.LeftToRight : TextDirections.RightToLeft,
                    SecondaryLanguageTags = rawlanguage.language.secondary_language_tags
                },
                PublisherId = rawlanguage.publisher_id,
                Platforms = new OnlinePlatformAvailability
                {
                    Windows8 = rawlanguage.platforms.win8,
                    WindowsPhone7 = rawlanguage.platforms.wp7,
                    iOS = rawlanguage.platforms.ios,
                    BlackBerry = rawlanguage.platforms.blackberry,
                    Android = rawlanguage.platforms.android,
                    Facebook = rawlanguage.platforms.facebook
                },
                OfflineInfo = new Offline
                {
                    Build = new Build
                    {
                        Min = rawlanguage.offline.build.min,
                        Max = rawlanguage.offline.build.max
                    },
                    Url = new Uri($"http:{rawlanguage.offline.url}"),
                    Platforms = new PlatformAvailability
                    {
                        Windows8 = rawlanguage.offline.platforms.win8,
                        WindowsPhone7 = rawlanguage.offline.platforms.wp7,
                        iOS = rawlanguage.offline.platforms.ios,
                        BlackBerry = rawlanguage.offline.platforms.blackberry,
                        Android = rawlanguage.offline.platforms.android
                    },
                    AlwaysAllowUpdates = rawlanguage.offline.always_allow_updates,
                    AllowRedownload = rawlanguage.offline.allow_redownload,
                    RequireEmailAgreement = rawlanguage.offline.require_email_agreement,
                    AgreementVersion = rawlanguage.offline.agreement_version
                },
                MetadataBuild = rawlanguage.metadata_build
            }).ToList();
    }

    public static async Task<VersionMetadata> GetVersionMetadataAsync(uint id, IProgress<int>? progress = null, CancellationToken? cancellationToken = null)
    {

        // pull down the versions page for this language
        string versionHtml = await HttpClientService.GetPage($"https://www.bible.com/api/bible/version/{id}", false, _cacheLifespan);
        cancellationToken?.ThrowIfCancellationRequested();
        progress?.Report(33);

        // Deserialize that page into a BibleDotComVersions object
        BibleDotComVersion rawVersion = JsonSerializer.Deserialize<BibleDotComVersion>(versionHtml)
            ?? throw new Exception("Failed to deserialize version");
        cancellationToken?.ThrowIfCancellationRequested();
        progress?.Report(67);

        return new VersionMetadata()
        {
            Id = rawVersion.id,
            Abbreviation = rawVersion.abbreviation,
            LocalAbbreviation = rawVersion.local_abbreviation,
            Title = rawVersion.title,
            LocalTitle = rawVersion.local_title,
            HasAudio = rawVersion.audio,
            AudioCount = rawVersion.audio_count,
            HasText = rawVersion.text,
            Language = new VersionLanguage
            {
                Iso639_1 = rawVersion.language.iso_639_1,
                Iso639_3 = rawVersion.language.iso_639_3,
                Tag = rawVersion.language.language_tag,
                LocalName = rawVersion.language.local_name,
                EnglishName = rawVersion.language.name,
                TextDirection = rawVersion.language.text_direction == "ltr" ? TextDirections.LeftToRight : TextDirections.RightToLeft,
                SecondaryLanguageTags = rawVersion.language.secondary_language_tags
            },
            Platforms = new OnlinePlatformAvailability
            {
                Windows8 = rawVersion.platforms.win8,
                WindowsPhone7 = rawVersion.platforms.wp7,
                iOS = rawVersion.platforms.ios,
                BlackBerry = rawVersion.platforms.blackberry,
                Android = rawVersion.platforms.android,
                Facebook = rawVersion.platforms.facebook
            },
            OfflineInfo = new Offline
            {
                Build = new Build
                {
                    Min = rawVersion.offline.build.min,
                    Max = rawVersion.offline.build.max
                },
                Url = new Uri($"http:{rawVersion.offline.url}"),
                Platforms = new PlatformAvailability
                {
                    Windows8 = rawVersion.offline.platforms.win8,
                    WindowsPhone7 = rawVersion.offline.platforms.wp7,
                    iOS = rawVersion.offline.platforms.ios,
                    BlackBerry = rawVersion.offline.platforms.blackberry,
                    Android = rawVersion.offline.platforms.android
                },
                AlwaysAllowUpdates = rawVersion.offline.always_allow_updates,
                AllowRedownload = rawVersion.offline.allow_redownload,
                RequireEmailAgreement = rawVersion.offline.require_email_agreement,
                AgreementVersion = rawVersion.offline.agreement_version
            },
            MetadataBuild = rawVersion.metadata_build,
            Copyright = new Copyright
            {
                Short = new TextHtml
                {
                    Text = rawVersion.copyright_short.text,
                    Html = rawVersion.copyright_short.html
                },
                Long = new TextHtml
                {
                    Text = rawVersion.copyright_long.text,
                    Html = rawVersion.copyright_long.html
                }
            },
            ReaderFooter = new ReaderFooter
            {
                Text = new TextHtml
                {
                    Text = rawVersion.reader_footer.text,
                    Html = rawVersion.reader_footer.html
                },
                Url = rawVersion.reader_footer_url == null ? null : new Uri($"http://{rawVersion.reader_footer_url}")
            },
            Publisher = rawVersion.publisher == null ? null :
                new Publisher
                {
                    Id = rawVersion.publisher.id,
                    EnglishName = rawVersion.publisher.name,
                    LocalName = rawVersion.publisher.local_name,
                    Url = rawVersion.publisher.url == null ? null : new Uri(rawVersion.publisher.url),
                    Description = rawVersion.publisher.description
                },
            Books = rawVersion.books.ToList().Select(b => new Book
            {
                Text = b.text,
                Canon = b.canon switch { "ot" => Canon.OldTestament, "nt" => Canon.NewTestament, "ap" => Canon.Apocrypha, _ => throw new NotImplementedException() },
                USFM = b.usfm,
                Abbreviation = b.abbreviation,
                Name = b.human,
                LongName = b.human_long,
                Audio = b.audio,
                Chapters = b.chapters.ToList().Select(c => new Chapter
                {
                    TableOfContents = c.toc,
                    Canonical = c.canonical,
                    Name = c.human,
                    USFM = c.usfm
                }).ToList()
            }).ToList()
        };

    }

    // Get translation
    public static async Task<string> GetDecodedVersion(uint id, uint? revision = null, string? destinationFilePath = null, IProgress<int>? progress = null, CancellationToken? cancellationToken = null)
    {
        // first, we need to get the version metadata, so we can get the url for the translation
        VersionMetadata versionMetadata = await GetVersionMetadataAsync(id, null, cancellationToken);
        cancellationToken?.ThrowIfCancellationRequested();

        // extract the maximum revision number
        // the minimum revision number seems to always be the same as the maximum revision number, and doesn't seem to actually
        // represent what the real minimum revision number is. So we'll ignore it, and instead just use the supplied revision.
        // if that file returns a 404, then we know it wasn't actually available.
        uint maxRevision = versionMetadata.OfflineInfo.Build.Max;
        revision ??= maxRevision;
        if (revision == uint.MinValue) { throw new Exception("Invalid revision number"); }
        if (revision > maxRevision) { throw new Exception("Revision number too high"); }
        cancellationToken?.ThrowIfCancellationRequested();

        // build the url for the translation
        // we want to take the portion of the string that's "/{id}-{maxRevision}.zip" and replace it with "/{id}-{revision}.zip"
        // so we'll split the string on the last dash, and then replace the last part with the new revision number
        string unadjustedUrl = versionMetadata.OfflineInfo.Url.ToString();
        string stringToFind = $"/{id}-{maxRevision}.zip";
        string replacementString = $"/{id}-{revision}.zip";
        string adjustedUrl = unadjustedUrl.Replace(stringToFind, replacementString);
        Uri translationUri = new (adjustedUrl);
        cancellationToken?.ThrowIfCancellationRequested();

        // pull down the translation
        string temporaryFileDestination = Path.GetTempFileName();
        await HttpClientService.GetDownload(temporaryFileDestination, translationUri);
        cancellationToken?.ThrowIfCancellationRequested();


        // unzip, decode, and then rezip to the final file
        string tempDestinationString = Path.GetTempFileName()[..^4];
        string finalFilename = tempDestinationString + ".zip";
        string tempPath = Path.Combine(Path.GetTempPath(), tempDestinationString);
        Directory.CreateDirectory(tempPath);

        using ZipArchive zipSource = ZipFile.OpenRead(temporaryFileDestination);
        uint filesInArchive = (uint)zipSource.Entries.Count;

        foreach (ZipArchiveEntry entry in zipSource.Entries)
        {
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
            cancellationToken?.ThrowIfCancellationRequested();
        }

        string finalFileDestination;
        if (destinationFilePath != null)
        {
            finalFileDestination = destinationFilePath;
        }
        else
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string cachePath = Path.Combine(appData, "LibBibleDotCom");
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            finalFileDestination = Path.Combine(cachePath, translationUri.Segments.Last());

        }


        if (File.Exists(finalFileDestination))
        {
            File.Delete(finalFileDestination);
        }
        ZipFile.CreateFromDirectory(tempPath, finalFileDestination);
        zipSource.Dispose();
        Directory.Delete(tempPath, true);
        File.Delete(temporaryFileDestination);
        cancellationToken?.ThrowIfCancellationRequested();

        return finalFileDestination;

        static string DecodeYves(IReadOnlyList<byte> arrayOfByte)
        {
            int i = 0;
            List<byte> byteArray = [];
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

    }

    public static async Task<string> GetDecodedVersion(string languageTag, string translation, string? destinationFilePath = null, IProgress<int>? progress = null, CancellationToken? cancellationToken = null)
    {
        // pull down the translations of that language
        List<Version> versions = await GetVersionsAsync(languageTag, progress, cancellationToken);
        cancellationToken?.ThrowIfCancellationRequested();

        // find the translation we're looking for
        uint id = versions.FirstOrDefault(v => v.Abbreviation == translation
                                            || v.LocalAbbreviation == translation
                                            || v.LocalTitle == translation
                                            || v.Title == translation)?.Id ?? throw new Exception("Translation not found");
        cancellationToken?.ThrowIfCancellationRequested();

        // get the translation
        return await GetDecodedVersion((uint)id, null, destinationFilePath, progress, cancellationToken);
    }

    public static async Task<Token> Tokenize(string input)
    {
        return new Token("root", string.Empty, input);
    }

    // Extract text from translation

    // Get translation audio
    
    


    // bundle content into translation bundle
    public static async Task<string> CreateTranslationBundle()
    {
        throw new NotImplementedException();
    }

  
}