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

        //string sourceFile = "C:\\Users\\jpenman\\AppData\\Roaming\\LibBibleDotCom\\114-19.zip";
        //await using FileStream file = File.OpenRead(sourceFile);
        //using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
        //{
        //    foreach (ZipArchiveEntry entry in zip.Entries)
        //    {
        //        await using Stream stream = entry.Open();
        //        StreamReader sr = new StreamReader(stream);
        //        string chapterName = entry.Name;
        //        Console.WriteLine(chapterName);
        //        string chapterContent = sr.ReadToEnd();
        //        Console.WriteLine("Plaintext");
        //        Console.WriteLine(chapterContent);
        //        Token t = BDC.BibleDotComService.Tokenize(chapterContent);
        //        Console.WriteLine("Tokenized");
        //        Console.WriteLine(t.ToXml());
        //        Console.ReadKey();
        //        Console.Clear();
        //    }
        //}

        //  var t = await bdc.BibleDotComService.Tokenize("This is a test");
        //  var t2 = await bdc.BibleDotComService.Tokenize("This is a test<b>Hellotestworld</b>");
        //  var t1 = await bdc.BibleDotComService.Tokenize("<b>Hello<b>test</b>world</b>");
        //var t3 = await BDC.BibleDotComService.Tokenize("<div class=\"version vid114 iso6393eng\" data-vid=\"114\" data-iso6393=\"eng\">\n\t<div class=\"book bk1JN\">\n\t\t<div class=\"chapter ch1 frag37090\" data-usfm=\"1JN.1\">\n\t\t\t<div class=\"label\">1</div>\n\t\t\t<div class=\"s\">\n\t\t\t\t<span class=\"heading\">What Was Heard, Seen, and Touched</span>\n\t\t\t</div>\n\t\t</div>\n\t</div>\n</div>");
        Token t4 = BDC.BibleDotComService.Tokenize("<div class=\"version vid114 iso6393eng\" data-vid=\"114\" data-iso6393=\"eng\"><div class=\"book bk1CH\"><div class=\"chapter ch12 frag6131\" data-usfm=\"1CH.12\"><div class=\"label\">12</div><div class=\"s\"><span class=\"heading\">The Growth of David's Army</span></div><div class=\"p\"><span class=\"verse v1\" data-usfm=\"1CH.12.1\"><span class=\"label\">1</span><span class=\"content\">Now </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 27:2</span></span><span class=\"content\">these </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> the men who came to David at </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 27:6</span></span><span class=\"content\">Ziklag while he was still a fugitive from Saul the son of Kish; and they </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> among the mighty men, helpers in the war, </span></span><span class=\"verse v2\" data-usfm=\"1CH.12.2\"><span class=\"label\">2</span><span class=\"content\">armed with bows, using both the right hand and </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Judg. 3:15; 20:16</span></span><span class=\"content\">the left in </span><span class=\"it\"><span class=\"content\">hurling</span></span><span class=\"content\"> stones and </span><span class=\"it\"><span class=\"content\">shooting</span></span><span class=\"content\"> arrows with the bow. </span><span class=\"it\"><span class=\"content\">They</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> of Benjamin, Saul's brethren.</span></span></div><div class=\"p\"><span class=\"verse v3\" data-usfm=\"1CH.12.3\"><span class=\"label\">3</span><span class=\"content\">The chief </span><span class=\"it\"><span class=\"content\">was</span></span><span class=\"content\"> Ahiezer, then Joash, the sons of Shemaah the Gibeathite; Jeziel and Pelet the sons of Azmaveth; Berachah, and Jehu the Anathothite; </span></span><span class=\"verse v4\" data-usfm=\"1CH.12.4\"><span class=\"label\">4</span><span class=\"content\">Ishmaiah the Gibeonite, a mighty man among the thirty, and over the thirty; Jeremiah, Jahaziel, Johanan, and Jozabad the Gederathite; </span></span><span class=\"verse v5\" data-usfm=\"1CH.12.5\"><span class=\"label\">5</span><span class=\"content\">Eluzai, Jerimoth, Bealiah, Shemariah, and Shephatiah the Haruphite; </span></span><span class=\"verse v6\" data-usfm=\"1CH.12.6\"><span class=\"label\">6</span><span class=\"content\">Elkanah, Jisshiah, Azarel, Joezer, and Jashobeam, the Korahites; </span></span><span class=\"verse v7\" data-usfm=\"1CH.12.7\"><span class=\"label\">7</span><span class=\"content\">and Joelah and Zebadiah the sons of Jeroham of Gedor.</span></span></div><div class=\"p\"><span class=\"verse v8\" data-usfm=\"1CH.12.8\"><span class=\"label\">8</span><span class=\"it\"><span class=\"content\">Some</span></span><span class=\"content\"> Gadites joined David at the stronghold in the wilderness, mighty men of valor, men trained for battle, who could handle shield and spear, whose faces </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">like</span></span><span class=\"content\"> the faces of lions, and </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\">  </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Sam. 2:18</span></span><span class=\"content\">as swift as gazelles on the mountains: </span></span><span class=\"verse v9\" data-usfm=\"1CH.12.9\"><span class=\"label\">9</span><span class=\"content\">Ezer the first, Obadiah the second, Eliab the third, </span></span><span class=\"verse v10\" data-usfm=\"1CH.12.10\"><span class=\"label\">10</span><span class=\"content\">Mishmannah the fourth, Jeremiah the fifth, </span></span><span class=\"verse v11\" data-usfm=\"1CH.12.11\"><span class=\"label\">11</span><span class=\"content\">Attai the sixth, Eliel the seventh, </span></span><span class=\"verse v12\" data-usfm=\"1CH.12.12\"><span class=\"label\">12</span><span class=\"content\">Johanan the eighth, Elzabad the ninth, </span></span><span class=\"verse v13\" data-usfm=\"1CH.12.13\"><span class=\"label\">13</span><span class=\"content\">Jeremiah the tenth, and Machbanai the eleventh. </span></span><span class=\"verse v14\" data-usfm=\"1CH.12.14\"><span class=\"label\">14</span><span class=\"content\">These </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> from the sons of Gad, captains of the army; the least was over a hundred, and the greatest was over a </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 18:13</span></span><span class=\"content\">thousand. </span></span><span class=\"verse v15\" data-usfm=\"1CH.12.15\"><span class=\"label\">15</span><span class=\"content\">These </span><span class=\"it\"><span class=\"content\">are</span></span><span class=\"content\"> the ones who crossed the Jordan in the first month, when it had overflowed all its </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Josh. 3:15; 4:18, 19</span></span><span class=\"content\">banks; and they put to flight all </span><span class=\"it\"><span class=\"content\">those</span></span><span class=\"content\"> in the valleys, to the east and to the west.</span></span></div><div class=\"p\"><span class=\"verse v16\" data-usfm=\"1CH.12.16\"><span class=\"label\">16</span><span class=\"content\">Then some of the sons of Benjamin and Judah came to David at the stronghold. </span></span><span class=\"verse v17\" data-usfm=\"1CH.12.17\"><span class=\"label\">17</span><span class=\"content\">And David went out to meet them, and answered and said to them, \"If you have come peaceably to me to help me, my heart will be united with you; but if to betray me to my enemies, since </span><span class=\"it\"><span class=\"content\">there</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">is</span></span><span class=\"content\"> no wrong in my hands, may the God of our fathers look and bring judgment.\" </span></span><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"label\">18</span><span class=\"content\">Then the Spirit came upon </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Sam. 17:25</span></span><span class=\"content\">Amasai, chief of the captains, </span><span class=\"it\"><span class=\"content\">and</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">he</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">said:</span></span></span></div><div class=\"q1\"><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"content\">\"</span><span class=\"it\"><span class=\"content\">We</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">are</span></span><span class=\"content\"> yours, O David;</span></span></div><div class=\"q2\"><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"content\">We </span><span class=\"it\"><span class=\"content\">are</span></span><span class=\"content\"> on your side, O son of Jesse!</span></span></div><div class=\"q2\"><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"content\">Peace, peace to you,</span></span></div><div class=\"q2\"><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"content\">And peace to your helpers!</span></span></div><div class=\"q2\"><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"content\">For your God helps you.\"</span></span></div><div class=\"p\"><span class=\"verse v18\" data-usfm=\"1CH.12.18\"><span class=\"content\">So David received them, and made them captains of the troop.</span></span></div><div class=\"p\"><span class=\"verse v19\" data-usfm=\"1CH.12.19\"><span class=\"label\">19</span><span class=\"content\">And </span><span class=\"it\"><span class=\"content\">some</span></span><span class=\"content\"> from Manasseh defected to David </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 29:2</span></span><span class=\"content\">when he was going with the Philistines to battle against Saul; but they did not help them, for the lords of the Philistines sent him away by agreement, saying, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 29:4</span></span><span class=\"content\">\"He may defect to his master Saul </span><span class=\"it\"><span class=\"content\">and</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">endanger</span></span><span class=\"content\"> our heads.\" </span></span><span class=\"verse v20\" data-usfm=\"1CH.12.20\"><span class=\"label\">20</span><span class=\"content\">When he went to Ziklag, those of Manasseh who defected to him were Adnah, Jozabad, Jediael, Michael, Jozabad, Elihu, and Zillethai, captains of the thousands who </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> from Manasseh. </span></span><span class=\"verse v21\" data-usfm=\"1CH.12.21\"><span class=\"label\">21</span><span class=\"content\">And they helped David against </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 30:1, 9, 10</span></span><span class=\"content\">the bands </span><span class=\"it\"><span class=\"content\">of</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">raiders,</span></span><span class=\"content\"> for they </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> all mighty men of valor, and they were captains in the army. </span></span><span class=\"verse v22\" data-usfm=\"1CH.12.22\"><span class=\"label\">22</span><span class=\"content\">For at </span><span class=\"it\"><span class=\"content\">that</span></span><span class=\"content\"> time they came to David day by day to help him, until </span><span class=\"it\"><span class=\"content\">it</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">was</span></span><span class=\"content\"> a great army, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Gen. 32:2; Josh. 5:13-15</span></span><span class=\"content\">like the army of God.</span></span></div><div class=\"s\"><span class=\"heading\">David's Army at Hebron</span></div><div class=\"p\"><span class=\"verse v23\" data-usfm=\"1CH.12.23\"><span class=\"label\">23</span><span class=\"content\">Now these </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> the numbers of the divisions </span><span class=\"it\"><span class=\"content\">that</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> equipped for war, </span><span class=\"it\"><span class=\"content\">and</span></span><span class=\"content\">  </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Sam. 2:1-4</span></span><span class=\"content\">came to David at </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Chr. 11:1</span></span><span class=\"content\">Hebron to </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Chr. 10:14</span></span><span class=\"content\">turn </span><span class=\"it\"><span class=\"content\">over</span></span><span class=\"content\"> the kingdom of Saul to him, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">1 Sam. 16:1-4</span></span><span class=\"content\">according to the word of the </span><span class=\"sc\"><span class=\"content\">Lord</span></span><span class=\"content\">: </span></span><span class=\"verse v24\" data-usfm=\"1CH.12.24\"><span class=\"label\">24</span><span class=\"content\">of the sons of Judah bearing shield and spear, six thousand eight hundred armed for war; </span></span><span class=\"verse v25\" data-usfm=\"1CH.12.25\"><span class=\"label\">25</span><span class=\"content\">of the sons of Simeon, mighty men of valor fit for war, seven thousand one hundred; </span></span><span class=\"verse v26\" data-usfm=\"1CH.12.26\"><span class=\"label\">26</span><span class=\"content\">of the sons of Levi four thousand six hundred; </span></span><span class=\"verse v27\" data-usfm=\"1CH.12.27\"><span class=\"label\">27</span><span class=\"content\">Jehoiada, the leader of the Aaronites, and with him three thousand seven hundred; </span></span><span class=\"verse v28\" data-usfm=\"1CH.12.28\"><span class=\"label\">28</span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Sam. 8:17; 1 Chr. 6:8, 53</span></span><span class=\"content\">Zadok, a young man, a valiant warrior, and from his father's house twenty-two captains; </span></span><span class=\"verse v29\" data-usfm=\"1CH.12.29\"><span class=\"label\">29</span><span class=\"content\">of the sons of Benjamin, relatives of Saul, three thousand (until then </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Sam. 2:8, 9</span></span><span class=\"content\">the greatest part of them had remained loyal to the house of Saul); </span></span><span class=\"verse v30\" data-usfm=\"1CH.12.30\"><span class=\"label\">30</span><span class=\"content\">of the sons of Ephraim twenty thousand eight hundred, mighty men of valor, famous men throughout their father's house; </span></span><span class=\"verse v31\" data-usfm=\"1CH.12.31\"><span class=\"label\">31</span><span class=\"content\">of the half-tribe of Manasseh eighteen thousand, who were designated by name to come and make David king; </span></span><span class=\"verse v32\" data-usfm=\"1CH.12.32\"><span class=\"label\">32</span><span class=\"content\">of the sons of Issachar </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Esth. 1:13</span></span><span class=\"content\">who had understanding of the times, to know what Israel ought to do, their chiefs were two hundred; and all their brethren were at their command; </span></span><span class=\"verse v33\" data-usfm=\"1CH.12.33\"><span class=\"label\">33</span><span class=\"content\">of Zebulun there were fifty thousand who went out to battle, expert in war with all weapons of war, </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">Ps. 12:2; (James 1:8)</span></span><span class=\"content\">stouthearted men who could keep ranks; </span></span><span class=\"verse v34\" data-usfm=\"1CH.12.34\"><span class=\"label\">34</span><span class=\"content\">of Naphtali one thousand captains, and with them thirty-seven thousand with shield and spear; </span></span><span class=\"verse v35\" data-usfm=\"1CH.12.35\"><span class=\"label\">35</span><span class=\"content\">of the Danites who could keep battle formation, twenty-eight thousand six hundred; </span></span><span class=\"verse v36\" data-usfm=\"1CH.12.36\"><span class=\"label\">36</span><span class=\"content\">of Asher, those who could go out to war, able to keep battle formation, forty thousand; </span></span><span class=\"verse v37\" data-usfm=\"1CH.12.37\"><span class=\"label\">37</span><span class=\"content\">of the Reubenites and the Gadites and the half-tribe of Manasseh, from the other side of the Jordan, one hundred and twenty thousand armed for battle with every </span><span class=\"it\"><span class=\"content\">kind</span></span><span class=\"content\"> of weapon of war.</span></span></div><div class=\"p\"><span class=\"verse v38\" data-usfm=\"1CH.12.38\"><span class=\"label\">38</span><span class=\"content\">All these men of war, who could keep ranks, came to Hebron with a loyal heart, to make David king over all Israel; and all the rest of Israel </span><span class=\"it\"><span class=\"content\">were</span></span><span class=\"content\"> of </span><span class=\"note x\"><span class=\"label\">#</span><span class=\" body\">2 Chr. 30:12</span></span><span class=\"content\">one mind to make David king. </span></span><span class=\"verse v39\" data-usfm=\"1CH.12.39\"><span class=\"label\">39</span><span class=\"content\">And they were there with David three days, eating and drinking, for their brethren had prepared for them. </span></span><span class=\"verse v40\" data-usfm=\"1CH.12.40\"><span class=\"label\">40</span><span class=\"content\">Moreover those who were near to them, from as far away as Issachar and Zebulun and Naphtali, were bringing food on donkeys and camels, on mules and oxen-provisions of flour and cakes of figs and cakes of raisins, wine and oil and oxen and sheep abundantly, for </span><span class=\"it\"><span class=\"content\">there</span></span><span class=\"content\">  </span><span class=\"it\"><span class=\"content\">was</span></span><span class=\"content\"> joy in Israel.</span></span></div></div></div></div>");

        Console.WriteLine(t4.ToXml());
    }
}