namespace LibBibleDotCom.Models;

internal class BibleDotComVersions
{

    // disabling naming styles warnings since this isn't a class I own
#pragma warning disable IDE1006 // Naming Styles
    public required Response response { get; set; }


    public class Build
    {
        public required uint max { get; set; }
        public required uint min { get; set; }
    }

    public class Data
    {
        public required List<Version> versions { get; set; }
        public Totals? totals { get; set; }
    }

    public class Language
    {
        public required string name { get; set; }
        public required string language_tag { get; set; }
        public required string? iso_639_1 { get; set; }
        public required string iso_639_3 { get; set; }
        public required string local_name { get; set; }
        public string? text_direction { get; set; }
        public object? secondary_language_tags { get; set; }
    }

    public class Offline
    {
        public required bool require_email_agreement { get; set; }
        public required string url { get; set; }
        public required bool always_allow_updates { get; set; }
        public uint? agreement_version { get; set; }
        public required Platforms platforms { get; set; }
        public required Build build { get; set; }
        public required bool allow_redownload { get; set; }
    }

    public class Platforms
    {
        public required bool win8 { get; set; }
        public required bool wp7 { get; set; }
        public required bool ios { get; set; }
        public required bool blackberry { get; set; }
        public required bool android { get; set; }
        public bool facebook { get; set; }
    }

    public class Response
    {
        public required Data data { get; set; }
        public string? buildtime { get; set; }
        public int? code { get; set; }
    }

    public class Totals
    {
        public int? languages { get; set; }
        public int? versions { get; set; }
    }

    public class Version
    {
        public string? vrs { get; set; }
        public required string local_title { get; set; }
        public required Language language { get; set; }
        public required string title { get; set; }
        public required Offline offline { get; set; }
        public required bool text { get; set; }
        public uint? metadata_build { get; set; }
        public required string abbreviation { get; set; }
        public required Platforms platforms { get; set; }
        public required uint audio_count { get; set; }
        public required string local_abbreviation { get; set; }
        public required bool audio { get; set; }
        public required uint? publisher_id { get; set; }
        public required uint id { get; set; }
    }


}
#pragma warning restore IDE1006 // Naming Styles