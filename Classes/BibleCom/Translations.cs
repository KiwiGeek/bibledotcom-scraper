using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleDotComScraper.Classes.BibleCom
{
    internal class Translations
    {

        public Response response { get; set; }
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Build
        {
            public int max { get; set; }
            public int min { get; set; }
        }

        public class Data
        {
            public List<Version> versions { get; set; }
            public Totals totals { get; set; }
        }

        public class Language
        {
            public string name { get; set; }
            public string language_tag { get; set; }
            public string iso_639_1 { get; set; }
            public string iso_639_3 { get; set; }
            public string local_name { get; set; }
            public string text_direction { get; set; }
            public object secondary_language_tags { get; set; }
        }

        public class Offline
        {
            public bool require_email_agreement { get; set; }
            public string url { get; set; }
            public bool always_allow_updates { get; set; }
            public int? agreement_version { get; set; }
            public Platforms platforms { get; set; }
            public Build build { get; set; }
            public bool allow_redownload { get; set; }
        }

        public class Platforms
        {
            public bool win8 { get; set; }
            public bool wp7 { get; set; }
            public bool ios { get; set; }
            public bool blackberry { get; set; }
            public bool android { get; set; }
            public bool facebook { get; set; }
        }

        public class Response
        {
            public Data data { get; set; }
            public string buildtime { get; set; }
            public int code { get; set; }
        }

        public class Totals
        {
            public int languages { get; set; }
            public int versions { get; set; }
        }

        public class Version
        {
            public string vrs { get; set; }
            public string local_title { get; set; }
            public Language language { get; set; }
            public string title { get; set; }
            public Offline offline { get; set; }
            public bool text { get; set; }
            public int metadata_build { get; set; }
            public string abbreviation { get; set; }
            public Platforms platforms { get; set; }
            public int audio_count { get; set; }
            public string local_abbreviation { get; set; }
            public bool audio { get; set; }
            public int? publisher_id { get; set; }
            public int id { get; set; }
        }


    }
}
