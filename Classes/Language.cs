using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleDotComScraper.Classes
{
    internal record Language(string Name, string Code, string Iso639, string LocalName, int TranslationCount) 
    {
        
    }
}
