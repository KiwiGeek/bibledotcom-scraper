using System.Text.Json;
using static LibBibleDotCom.Models.Version;
using Version = LibBibleDotCom.Models.Version;

namespace LibBibleDotCom;

public static class BibleDotComService
{

    static TimeSpan _cacheLifespan = TimeSpan.FromDays(1);

    public static void SetCacheLifespan(TimeSpan cacheLifespan)
    {
        _cacheLifespan = cacheLifespan;
    }

    public static async Task<List<InfoLanguage>> GetAllLanguages()
    {
        // Pulls down the current configuration page from bible.com, which contains a list of all the languages
        string languageHtml = await HttpClientService.GetPage("https://www.bible.com/api/bible/configuration", false, _cacheLifespan);

        // Deserialize that page into a BibleDotComLanguages object
        BibleDotComLanguages? rawLanguages = JsonSerializer.Deserialize<BibleDotComLanguages>(languageHtml)
            ?? throw new Exception("Failed to deserialize languages");

        // some languages will be returned with an null Id, so we need to resolve that.
        // pulling down the versions page for that language (from the tag) will give us the id, which we need to parse
        // out and then use to update the language object.
        // Although it's not currently the case, it's plausible that at some point a language could be returned with no Id which has 
        // multiple translations. In that case, we'd need to pull down all the translations, and double-check them against the 
        // languagues list above to find the Id that's not used. This would probably never happen, but it's better to be safe than sorry.
        foreach (BibleDotComLanguages.DefaultVersion rawLanguage in rawLanguages.response.data.default_versions.Where(l => l.id == null))
        {

            List<Version> versions = await GetVersions(rawLanguage.language_tag);
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

    public static async Task<List<Version>> GetVersions(string languageTag)
    {
        // pull down the versions page for this language
        string versionHtml = await HttpClientService.GetPage($"https://www.bible.com/api/bible/versions?language_tag={languageTag}&type=all", false, _cacheLifespan);

        // Deserialize that page into a BibleDotComVersions object
        BibleDotComVersions? rawVersions = JsonSerializer.Deserialize<BibleDotComVersions>(versionHtml)
            ?? throw new Exception("Failed to deserialize version");

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

    public static async Task<Version> GetVersion(string languageTag, uint id)
    {
        return (await GetVersions(languageTag)).First(v => v.Id == id); 
    }


}
