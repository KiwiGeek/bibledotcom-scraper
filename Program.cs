﻿using LibBibleDotCom.Models;
using BDC = LibBibleDotCom;
using System.IO;
using System.IO.Compression;

namespace BibleDotComScraper;

internal static class Program
{

    public class BagOfData
    {
        public List<InfoLanguage> Languages = [];
        public List<BDC.Models.Version> Translations = [];
        public List<VersionMetadata> Metadata = [];
    }

    static async Task Main()
    {
        //BDC.BibleDotComService.SetCacheLifespan(TimeSpan.FromDays(100));
        //string b = await BDC.BibleDotComService.GetDecodedVersion("eng", "NKJV");
        //Console.WriteLine(b);
        //Console.ReadKey();

        string sourceFile = "C:\\Users\\jpenman\\AppData\\Roaming\\LibBibleDotCom\\114-19.zip";
        await using FileStream file = File.OpenRead(sourceFile);
        using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
        {
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                await using Stream stream = entry.Open();
                StreamReader sr = new StreamReader(stream);
                string chapterName = entry.Name;
                Console.WriteLine(chapterName);
                string chapterContent = sr.ReadToEnd();
                Console.WriteLine("Plaintext");
                Console.WriteLine(chapterContent);
                Token t = BDC.BibleDotComService.Tokenize(chapterContent);
                Console.WriteLine("Tokenized");
                Console.WriteLine(t.ToXml());
                Console.ReadKey();
                Console.Clear();
            }
        }

        //  var t = await bdc.BibleDotComService.Tokenize("This is a test");
        //  var t2 = await bdc.BibleDotComService.Tokenize("This is a test<b>Hellotestworld</b>");
        //  var t1 = await bdc.BibleDotComService.Tokenize("<b>Hello<b>test</b>world</b>");
        //var t3 = await BDC.BibleDotComService.Tokenize("<div class=\"version vid114 iso6393eng\" data-vid=\"114\" data-iso6393=\"eng\">\n\t<div class=\"book bk1JN\">\n\t\t<div class=\"chapter ch1 frag37090\" data-usfm=\"1JN.1\">\n\t\t\t<div class=\"label\">1</div>\n\t\t\t<div class=\"s\">\n\t\t\t\t<span class=\"heading\">What Was Heard, Seen, and Touched</span>\n\t\t\t</div>\n\t\t</div>\n\t</div>\n</div>");
        Token t4 = BDC.BibleDotComService.Tokenize("<div class=\"version vid114 iso6393eng\" data-vid=\"114\" data-iso6393=\"eng\"> <div class=\"book bk1JN\"> <div class=\"chapter ch1 frag37090\" data-usfm=\"1JN.1\"><div class=\"label\">1</div><div class=\"s\"><span class=\"heading\">What Was Heard, Seen, and Touched</span></div> <div class=\"p\"><span class=\"verse v1\" data-usfm=\"1JN.1.1\"><span class=\"label\">1</span><span class=\"content\">That </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 1:1); 1 John 2:13, 14</span></span><span class=\"content\">which was from the beginning, which we have heard, which we have </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Luke 1:2; John 1:14</span></span><span class=\"content\">seen with our eyes, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Pet. 1:16</span></span><span class=\"content\">which we have looked upon, and </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Luke 24:39; John 20:27</span></span><span class=\"content\">our hands have handled, concerning the </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 1:1, 4, 14)</span></span><span class=\"content\">Word of life— </span></span><span class=\"verse v2\" data-usfm=\"1JN.1.2\"><span class=\"label\">2</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 1:4; (1 John 3:5, 8; 5:20)</span></span><span class=\"content\">the life </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Rom. 16:26; 1 Tim. 3:16</span></span><span class=\"content\">was manifested, and we have seen, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 21:24</span></span><span class=\"content\">and bear witness, and declare to you that eternal life which was </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 1:1, 18; 16:28)</span></span><span class=\"content\">with the Father and was manifested to us— </span></span><span class=\"verse v3\" data-usfm=\"1JN.1.3\"><span class=\"label\">3</span><span class=\"content\">that which we have seen and heard we declare to you, that you also may have fellowship with us; and truly our fellowship </span><span class=\"it\"><span class=\"content\">is</span></span><span class=\"content\"> </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 17:21; 1 Cor. 1:9; 1 John 2:24</span></span><span class=\"content\">with the Father and with His Son Jesus Christ. </span></span><span class=\"verse v4\" data-usfm=\"1JN.1.4\"><span class=\"label\">4</span><span class=\"content\">And these things we write to you </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 15:11; 16:24; 1 Pet. 1:8</span></span><span class=\"content\">that your joy may be full.</span></span></div> <div class=\"s\"><span class=\"heading\">Fellowship with Him and One Another</span></div> <div class=\"p\"><span class=\"verse v5\" data-usfm=\"1JN.1.5\"><span class=\"label\">5</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 1:19; 1 John 3:11</span></span><span class=\"content\">This is the message which we have heard from Him and declare to you, that </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(1 Tim. 6:16); James 1:17</span></span><span class=\"content\">God is light and in Him is no darkness at all. </span></span><span class=\"verse v6\" data-usfm=\"1JN.1.6\"><span class=\"label\">6</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(John 8:12); 2 Cor. 6:14; (1 John 2:9–11)</span></span><span class=\"content\">If we say that we have fellowship with Him, and walk in darkness, we lie and do not practice the truth. </span></span><span class=\"verse v7\" data-usfm=\"1JN.1.7\"><span class=\"label\">7</span><span class=\"content\">But if we </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Is. 2:5</span></span><span class=\"content\">walk in the light as He is in the light, we have fellowship with one another, and </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(1 Cor. 6:11)</span></span><span class=\"content\">the blood of Jesus Christ His Son cleanses us from all sin.</span></span></div> <div class=\"p\"><span class=\"verse v8\" data-usfm=\"1JN.1.8\"><span class=\"label\">8</span><span class=\"content\">If we say that we have no sin, we deceive ourselves, and the truth is not in us. </span></span><span class=\"verse v9\" data-usfm=\"1JN.1.9\"><span class=\"label\">9</span><span class=\"content\">If we </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Ps. 32:5; Prov. 28:13</span></span><span class=\"content\">confess our sins, He is </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">(Rom. 3:24–26)</span></span><span class=\"content\">faithful and just to forgive us </span><span class=\"it\"><span class=\"content\">our</span></span><span class=\"content\"> sins and to </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Ps. 51:2</span></span><span class=\"content\">cleanse us from all unrighteousness. </span></span><span class=\"verse v10\" data-usfm=\"1JN.1.10\"><span class=\"label\">10</span><span class=\"content\">If we say that we have not sinned, we </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">John 3:33; 1 John 5:10</span></span><span class=\"content\">make Him a liar, and His word is not in us.</span></span></div></div></div></div>");

        Console.WriteLine(t4.ToXml());
    }
}