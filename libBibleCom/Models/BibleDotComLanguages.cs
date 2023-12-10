using System.Text.Json.Serialization;

namespace LibBibleDotCom.Models;
public class BibleDotComLanguages
{

    // disabling naming styles warnings since this isn't a class I own
#pragma warning disable IDE1006 // Naming Styles
    public required Response response { get; set; }


    public class Data
    {
        public Stylesheets? stylesheets { get; set; }
        public required List<DefaultVersion> default_versions { get; set; }
        public Totals? totals { get; set; }
        public string? short_url { get; set; }
    }

    public class DefaultVersion
    {
        public required string name { get; set; }
        public required bool has_audio { get; set; }
        public required string language_tag { get; set; }
        public int? total_versions { get; set; }
        public string? iso_639_1 { get; set; }
        public uint? id { get; set; }
        public required string iso_639_3 { get; set; }
        public required bool has_text { get; set; }
        public required string local_name { get; set; }
        public object? font { get; set; }
        public required string text_direction { get; set; }
    }

    public class Response
    {
        public required Data data { get; set; }
        public string? buildtime { get; set; }
        public int? code { get; set; }
    }


    public class Stylesheets
    {
        public string? android { get; set; }
        public string? ios { get; set; }
    }

    public class Totals
    {
        public int? languages { get; set; }
        public int? versions { get; set; }
    }
}

#pragma warning restore IDE1006 // Naming Styles