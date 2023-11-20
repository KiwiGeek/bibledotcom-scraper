namespace BibleDotComScraper.Classes.BibleCom;

internal class Languages
{

    public Response response { get; set; }

    public class Data
    {
        public Stylesheets stylesheets { get; set; }
        public List<DefaultVersion> default_versions { get; set; }
        public Totals totals { get; set; }
        public string short_url { get; set; }
    }

    public class DefaultVersion
    {
        public string name { get; set; }
        public bool has_audio { get; set; }
        public string language_tag { get; set; }
        public int total_versions { get; set; }
        public string iso_639_1 { get; set; }
        public int? id { get; set; }
        public string iso_639_3 { get; set; }
        public bool has_text { get; set; }
        public string local_name { get; set; }
        public object font { get; set; }
        public string text_direction { get; set; }
    }

    public class Response
    {
        public Data data { get; set; }
        public string buildtime { get; set; }
        public int code { get; set; }
    }


    public class Stylesheets
    {
        public string android { get; set; }
        public string ios { get; set; }
    }

    public class Totals
    {
        public int languages { get; set; }
        public int versions { get; set; }
    }
}