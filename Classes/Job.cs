using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibleDotComScraper.Enums;

namespace BibleDotComScraper.Classes
{
    class Job
    {
        public Guid Id { get; init; }
        public JobTypes Type { get; init; } 
        public string Url { get; set; }
        //todo: move Url to init only

        public Job( Guid id, JobTypes type, string url)
        {
            Id = id;
            Type = type;
            Url = url;
        }
    }
}
