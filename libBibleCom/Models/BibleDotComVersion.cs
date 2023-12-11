namespace LibBibleDotCom.Models;

internal class BibleDotComVersion
{

    // disabling naming styles warnings since this isn't a class I own
#pragma warning disable IDE1006 // Naming Styles
    public class Book
    {
        public bool text { get; set; }
        public string canon { get; set; }
        public List<Chapter> chapters { get; set; }
        public string usfm { get; set; }
        public string abbreviation { get; set; }
        public string human { get; set; }
        public bool audio { get; set; }
        public string human_long { get; set; }
    }

    public class Build
    {
        public uint min { get; set; }
        public uint max { get; set; }
    }

    public class Chapter
    {
        public bool toc { get; set; }
        public bool canonical { get; set; }
        public string human { get; set; }
        public string usfm { get; set; }
    }

    public class CopyrightLong
    {
        public required string? text { get; set; }
        public required string? html { get; set; }
    }

    public class CopyrightShort
    {
        public required string? text { get; set; }
        public required string? html { get; set; }
    }

    public class Language
    {
        public string iso_639_1 { get; set; }
        public string iso_639_3 { get; set; }
        public string name { get; set; }
        public string local_name { get; set; }
        public string text_direction { get; set; }

        public string language_tag { get; set; }
        public object? secondary_language_tags { get; set; }
    }

    public class Offline
    {
        public Build build { get; set; }
        public string url { get; set; }
        public Platforms platforms { get; set; }
        public bool always_allow_updates { get; set; }
        public bool allow_redownload { get; set; }
        public bool require_email_agreement { get; set; }
        public uint? agreement_version { get; set; }
    }

    public class Platforms
    {
        public bool win8 { get; set; }
        public bool wp7 { get; set; }
        public bool ios { get; set; }
        public bool facebook { get; set; }
        public bool blackberry { get; set; }
        public bool android { get; set; }
    }

    public class Publisher
    {
        public uint id { get; set; }
        public string name { get; set; }
        public string local_name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
    }

    public class ReaderFooter
    {
        public string? text { get; set; }
        public string? html { get; set; }
    }

    public uint id { get; set; }
    public string abbreviation { get; set; }
    public string local_abbreviation { get; set; }
    public string title { get; set; }
    public string local_title { get; set; }
    public bool audio { get; set; }
    public uint audio_count { get; set; }
    public bool text { get; set; }
    public Language language { get; set; }
    public CopyrightShort copyright_short { get; set; }
    public CopyrightLong copyright_long { get; set; }
    public ReaderFooter reader_footer { get; set; }
    public string? reader_footer_url { get; set; }
    public Publisher? publisher { get; set; }
    public Platforms platforms { get; set; }
    public Offline offline { get; set; }
    public uint metadata_build { get; set; }
    public List<Book> books { get; set; }
    public string vrs { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles