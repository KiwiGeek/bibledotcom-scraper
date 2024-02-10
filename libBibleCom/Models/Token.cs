using OneOf;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;

namespace LibBibleDotCom.Models;
public partial class Token
{
    private readonly List<OneOf<string, Token>> _content = [];
    private readonly string _tag;
    private readonly string _class;
    private readonly List<KeyValuePair<string, string>> _attributes = [];

    [GeneratedRegex(@".*data-vid=\""(?<id>\d+)\"".*data-iso6393=\""(?<lang>.{3})\""", RegexOptions.IgnoreCase)]
    private static partial Regex VersionRegex();

    [GeneratedRegex(@".*book bk(?<book>[1-3A-Z]+)\""", RegexOptions.IgnoreCase)]
    private static partial Regex BookRegex();

    [GeneratedRegex(@".*data-usfm=""(?<usfm>(?<book>.*)\.(?<chapter>\d+))""", RegexOptions.IgnoreCase)]
    private static partial Regex ChapterRegex();

    [GeneratedRegex(@"data-usfm=""(?<usfm>(?<book>[A-Z0-9]+).(?<chapter>\d+).(?<verse>\d+))""", RegexOptions.IgnoreCase)]
    private static partial Regex VerseRegex();

    [GeneratedRegex(@"class=""q(?<level>\d)""", RegexOptions.IgnoreCase)]
    private static partial Regex QuoteBlockRegex();


    public string ToXml()
    {
        // for now, this is going to just build an XML string. when we're done, we'll actually make it return a xml document.

        if (!IsFullyUnderstood()) { throw new InvalidDataException("This token is not fully understood"); }

        string tag = string.Empty;
        if (IsVerse) { tag = "Verse"; }
        if (IsRoot) { tag = "Root"; }
        if (IsVersion) { tag = "Version"; }
        if (IsBook) { tag = "Book"; }
        if (IsChapter) { tag = "Chapter"; }
        if (IsChapterLabel) { tag = "ChapterHeading"; }
        if (IsVerseLabel) { tag = "VerseLabel"; }
        if (IsSection) { tag = "Section"; }
        if (IsHeading) { tag = "Heading"; }
        if (IsParagraph) { tag = "Paragraph"; }
        if (IsContent) { tag = "Content"; }
        if (IsNote) { tag = "Note"; }
        if (IsNoteBody) { tag = "NoteBody"; }
        if (IsItalics) { tag = "Italics"; }
        if (IsSmallCaps) { tag = "SmallCaps"; }
        if (IsQuoteBlock) { tag = "QuoteBlock"; }
        string attributes = string.Empty;
        foreach (KeyValuePair<string, string> attrib in _attributes)
        {
            attributes += $" {attrib.Key}=\"{attrib.Value}\"";
        }
        string openingTag = $"<{tag}{attributes}>";
        string contentTag = string.Empty;
        string closingTag = $"</{tag}>";
        foreach (OneOf<string, Token> item in _content)
        {
            if (item.IsT0)
            {
                contentTag += item.AsT0.Trim();
            }
            else
            {
                contentTag += item.AsT1.ToXml().Trim();
            }
        }
        string content = $"{openingTag}{contentTag}{closingTag}";

        // if we're not the root element, we don't need to do any clean-up work, so just return the results.
        if (!content.StartsWith("<Root>")) { return content; }

        // Mid-parsing clean-up
        content = PurgeNotes(content);
        content = ProcessSectionHeaders(content);
        content = ProcessContentNodes(content);
        content = ProcessQuoteBlocks(content);
        content = ProcessPartials(content);
        

        return content[6..^7];
    }

    private string PurgeNotes(string content)
    {
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(content);

        XmlNodeList? noteNodes = xmlDoc.SelectNodes("//Note");
        if (noteNodes == null) { return xmlDoc.OuterXml; }
        foreach (XmlNode noteNode in noteNodes)
        {
            noteNode.ParentNode?.RemoveChild(noteNode);
        }
        return xmlDoc.OuterXml;
    }

    private string ProcessSectionHeaders(string content)
    {
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(content);

        XmlNodeList? sectionNodes = xmlDoc.SelectNodes("//Section");
        if (sectionNodes == null) { return xmlDoc.OuterXml; }
        foreach (XmlNode sectionNode in sectionNodes)
        {
            XmlNode? headingNode = sectionNode.SelectSingleNode("Heading");
            if (headingNode == null) { continue; }

            // Create the new SectionHeader Node
            
            XmlElement sectionHeaderNode = xmlDoc.CreateElement("SectionHeading");
            sectionHeaderNode.InnerXml = headingNode.InnerXml;

            // replace the existing nodes.
            sectionNode.ParentNode!.ReplaceChild(sectionHeaderNode, sectionNode);
        }
        return xmlDoc.OuterXml;
    }

    private string ProcessContentNodes(string content)
    {
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(content);

        while (xmlDoc.SelectSingleNode("//Content") != null) 
        {
            XmlNode node = xmlDoc.SelectSingleNode("//Content")!;

            // find the parent node of node
            XmlNode parentNode = node.ParentNode!;

            // find our node in the parent's node.
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode != node) { continue; }
                XmlNode newText = xmlDoc.CreateTextNode(childNode.InnerText + " ");
                parentNode.ReplaceChild(newText, childNode);
                break;
            }
        }
        return xmlDoc.OuterXml;
    }
    
    private string ProcessQuoteBlocks(string content)
    {
        /*
         * We are going to assume that the "Q" in the styling refers to a quote block level, and not anything to do with
         * whether it's a poetic line (as the documentation seems to imply).
         *
         * Currently, QuoteBlock verses look like this: (As a child of a chapter node):
         * <QuoteBlock level="1">
         *   <Verse id="1CH.12.18" book="1CH" chapter="12" verse="18">" &lt;Italics&gt;We &lt;/Italics&gt; &lt;Italics&gt;are &lt;/Italics&gt;yours, O David; </Verse>
         * </Poetic>
         * <QuoteBlock level="2">
         *   <Verse id="1CH.12.18" book="1CH" chapter="12" verse="18">We &lt;Italics&gt;are &lt;/Italics&gt;on your side, O son of Jesse! </Verse>
         * </Poetic>
         * <QuoteBlock level="2">
         *   <Verse id="1CH.12.18" book="1CH" chapter="12" verse="18">Peace, peace to you, </Verse>
         * </Poetic>
         * <QuoteBlock level="2">
         *   <Verse id="1CH.12.18" book="1CH" chapter="12" verse="18">And peace to your helpers! </Verse>
         * </Poetic>
         * <QuoteBlock level="2">
         *   <Verse id="1CH.12.18" book="1CH" chapter="12" verse="18">For your God helps you." </Verse>
         * </Poetic>
         *
         * We want to turn them into the following:
         * 
         * <QuoteBlock id="1CH.12.18" book="1CH" chapter="12" verse="18" level="1">
         *   &lt;Italics&gt;We &lt;/Italics&gt; &lt;Italics&gt;are &lt;/Italics&gt;yours, O David;</QuoteBlock>
         * <QuoteBlock id="1CH.12.18" book="1CH" chapter="12" verse="18" level="2">
         *   We &lt;Italics&gt;are &lt;/Italics&gt;on your side, O son of Jesse! </QuoteBlock>
         * <QuoteBlock id="1CH.12.18" book="1CH" chapter="12" verse="18" level="2">
         *   Peace, peace to you, </QuoteBlock>
         * <QuoteBlock id="1CH.12.18" book="1CH" chapter="12" verse="18" level="2">
         *   And peace to your helpers! </QuoteBlock>
         * <QuoteBlock id="1CH.12.18" book="1CH" chapter="12" verse="18" level="2">
         *   For your God helps you." </QuoteBlock>
         * 
         * Even more problematic, Isaiah 37:22 (problematic from a solving this problem perspective, not from a verse
         * context perspective):
         *
         * <Paragraph>
         *  	...
         *  	<Verse id="ISA.37.22" book="ISA" chapter="37" verse="22">
         * 		<VerseLabel>22</VerseLabel>this &lt;Italics&gt;is &lt;/Italics&gt;the word which the
         *        &lt;SmallCaps&gt;Lord &lt;/SmallCaps&gt;has spoken concerning him: 
         *  	</Verse>
         * </Paragraph>
         * <QuoteBlock level="1">
         *   <Verse id="ISA.37.22" book="ISA" chapter="37" verse="22">“The virgin, the daughter of Zion, </Verse>
         * </QuoteBlock>
         * <QuoteBlock level="2">
         *   <Verse id="ISA.37.22" book="ISA" chapter="37" verse="22">Has despised you, laughed you to scorn; </Verse>
         * </QuoteBlock>
         * <QuoteBlock level="2">
         *   <Verse id="ISA.37.22" book="ISA" chapter="37" verse="22">The daughter of Jerusalem </Verse>
         * </QuoteBlock>
         * <QuoteBlock level="2">
         *   <Verse id="ISA.37.22" book="ISA" chapter="37" verse="22">Has shaken &lt;Italics&gt;her
         *      &lt;/Italics&gt;head behind your back! </Verse>
         * </QuoteBlock>
         * 
         * In this case we have a verse which is spread across two distinct visual nodes. We can't put the poetic lines
         * within the Paragraph above, because that's not how it looks in the Bible, and we can't move the plain text
         * verse to the Poetic verse node, because that's a display block, rather than an inline block. So we have to
         * wrestle with the unique fact that we're trying to handle displaying the verse by itself, as well as inline
         * with the remainder of the verse. 
         * 
         * <Paragraph>
         *   ...
         *   <Verse id="ISA.37.22" book="ISA" chapter="37" verse="22">
         *     <VerseLabel>22</VerseLabel>this <Italics>is</Italics> the word which the <SmallCaps>Lord</SmallCaps> has
         *       spoken concerning him:
         *   </Verse>
         * </Paragraph>
         * <QuoteBlock level="1" id="ISA.37.22" book="ISA" chapter="37" verse="22">“The virgin, the daughter of Zion, </QuoteBlock>
         * <QuoteBlock level="2" id="ISA.37.22" book="ISA" chapter="37" verse="22">Has despised you, laughed you to scorn; </QuoteBlock>
         * <QuoteBlock level="2" id="ISA.37.22" book="ISA" chapter="37" verse="22">The daughter of Jerusalem </QuoteBlock>
         * <QuoteBlock level="2" id="ISA.37.22" book="ISA" chapter="37" verse="22">Has shaken <Italics>her</Italics> head behind your back! </Indent>
         *
         */

        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(content);

        while (xmlDoc.SelectSingleNode("//QuoteBlock/Verse") != null)
        {
            XmlNode node = xmlDoc.SelectSingleNode("//QuoteBlock/Verse")!;

            // find the parent node of node
            XmlNode parentNode = node.ParentNode!;

            // We want to move all the attributes from our node to our parent node
            foreach (XmlAttribute a in node.Attributes)
            {
                XmlAttribute attr = xmlDoc.CreateAttribute(a.Name);
                attr.Value = a.Value;
                parentNode.Attributes.Append(attr);
            }

            // bomb out if we have more than one child node.
            Debug.Assert(parentNode.ChildNodes.Count == 1);
            
            XmlNode childNode = parentNode.ChildNodes.Item(0)!;
           // XmlNode newText = childNode;   //  (childNode.InnerXml + " ");
            parentNode.ReplaceChild(childNode, node);
        }
        return xmlDoc.OuterXml;
    }

    private string ProcessPartials(string content)
    {
        return content;
    }

    private bool IsFullyUnderstood()
    {
        // we are fully understood if:
        // - our own tag/class is understood
        // - all of our children are understood.

        bool understood = IsRoot || IsVersion || IsBook || IsChapter || IsChapterLabel || IsVerseLabel || IsSection || IsHeading || IsParagraph
                        || IsVerse || IsContent || IsNote || IsNoteBody || IsItalics || IsSmallCaps || IsQuoteBlock;
        if (!understood)
        {
            Console.WriteLine($"I don't understand {_tag} {_class}");
            return false;
        }

        foreach (OneOf<string, Token> item in _content)
        {
            if (item.IsT0) { continue; }
            if (!item.AsT1.IsFullyUnderstood()) { return false; }
        }
        return true;

    }

    private bool IsRoot => _tag == "root";
    private bool IsVersion => _tag == "div" && _class.StartsWith("class=\"version");
    private bool IsBook => _tag == "div" && _class.StartsWith("class=\"book");
    private bool IsChapter => _tag == "div" && _class.StartsWith("class=\"chapter");
    private bool IsChapterLabel => _tag == "div" && _class.StartsWith("class=\"label");
    private bool IsVerseLabel => _tag == "span" && _class.StartsWith("class=\"label");
    private bool IsSection => _tag == "div" && _class == "class=\"s\"";
    private bool IsHeading => _tag == "span" && _class == "class=\"heading\"";
    private bool IsParagraph => _tag == "div" && _class == "class=\"p\"";
    private bool IsVerse => _tag == "span" && _class.StartsWith("class=\"verse");
    private bool IsContent => _tag == "span" && _class.StartsWith("class=\"content");
    private bool IsNote => _tag == "span" && _class.StartsWith("class=\"note");
    private bool IsNoteBody => _tag == "span" && _class.StartsWith("class=\" body");
    private bool IsItalics => _tag == "span" && _class == "class=\"it\"";
    private bool IsSmallCaps => _tag == "span" && _class == "class=\"sc\"";
    private bool IsQuoteBlock => _tag == "div" && QuoteBlockRegex().IsMatch(_class);

    private List<KeyValuePair<string, string>> GetAttributesFromRegex(Regex regex, params (string source, string dest)[] mappings)
    {
        List<KeyValuePair<string, string>> results = [];
        Match match = regex.Match(_class);
        if (match.Success)
        {
            GroupCollection groups = match.Groups;
            foreach ((string s, string d) in mappings)
            {
                results.Add(new KeyValuePair<string, string>(d, groups[s].Value));
            }
            return results;
        }

        return [];
    }

    public Token(string tag, string @class, string input)
    {

        _tag = tag;
        _class = @class;

        if (IsVersion)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                VersionRegex(),
                ("id", "id"),
                ("lang", "lang")));
        }
        else if (IsBook)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                BookRegex(),
                ("book", "id")));
        }
        else if (IsChapter)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                ChapterRegex(),
                ("usfm", "id"),
                ("book", "book"),
                ("chapter", "chapter")));
        }
        else if (IsVerse)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                VerseRegex(),
                ("usfm", "id"),
                ("book", "book"),
                ("chapter", "chapter"),
                ("verse", "verse")));
        } 
        else if (IsQuoteBlock)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                QuoteBlockRegex(),
                ("level", "level")));
        }

        while (input.Length > 0)
        {

            // determine if the next token is a tag or text
            if (input[0] != '<')
            {
                // it's an immediate. In which case we want to find the next "<",
                // and everything before that is text we want to tokenize.
                int nextTagIndex = input.IndexOf('<');
                if (nextTagIndex == -1)
                {
                    // there are no more tags, so the rest of the input is text
                    _content.Add(input);
                    input = "";
                }
                else
                {
                    // there is a tag, so the text is everything up to that tag
                    _content.Add(input[..nextTagIndex]);
                    input = input[nextTagIndex..];
                }
            }
            else
            {
                // it's a tag
                // temporarily, let's just drop that first character.
                string openingTag = input[..(input.IndexOf('>', StringComparison.Ordinal) + 1)];
                string openTagType = openingTag.Contains(' ') ? openingTag[1..openingTag.IndexOf(' ')] : openingTag[1..^1];
                string closingTagToFind = $"</{openTagType}>";
                string openTagToFind = $"<{openTagType}";
                bool done = false;
                int searchIndex = openingTag.Length - 1;
                int tagCounter = 1;

                while (!done)
                {

                    //Console.WriteLine($"Looking for {openTagToFind} or {closingTagToFind}");
                    int openingTagCharIndex = input.IndexOf(openTagToFind, searchIndex, StringComparison.Ordinal);
                    int closingTagCharIndex = input.IndexOf(closingTagToFind, searchIndex, StringComparison.Ordinal);

                    // we're now in one of three states.
                    // we found an opening tag, but no closing tag. This would be bad
                    // we found both tags, but the opening tag is first.
                    // we found a closing tag.
                    if (openingTagCharIndex >= 0 && closingTagCharIndex == -1)
                    {
                        // there's no closing tag left anywhere in the string. It's malformed, so 
                        // we say screw it and @ on out of there.
                        throw new InvalidDataException("Did not find a closing tag. We should panic");
                    }
                    else if (openingTagCharIndex < closingTagCharIndex && openingTagCharIndex != -1)
                    {
                        // the next tag that we found is an opening tag, so increment the counter, and move
                        // the index on past this tag.
                        tagCounter++;
                        searchIndex = openingTagCharIndex + 1;
                        //Console.WriteLine($"The next tag was an opening Tag, at {openingTagCharIndex}. We are {tagCounter} tags deep.");
                    }
                    else
                    {
                        // presumably, all being well with the world, we found a closing tag. So decrement 
                        // the counter, and see if we're done.
                        tagCounter--;
                        searchIndex = closingTagCharIndex + 1;
                        //Console.WriteLine($"The next tag was a closing Tag, at {closingTagCharIndex}. We are {tagCounter} tags deep.");
                        if (tagCounter == 0)
                        {
                            done = true;
                            searchIndex += closingTagToFind.Length - 1;
                        }
                    }

                }

                // we have the opening tag, and the closing tag, and everything in between.
                // so let's go ahead and tokenize it.
                string classInfo = input[openTagType.Length + 1] == '>' ? string.Empty : input[(openTagType.Length + 2)..input.IndexOf('>')];
                string content = input[(openingTag.Length)..(searchIndex - closingTagToFind.Length)];
                Token newToken = new(openTagType, classInfo, content);
                _content.Add(newToken);
                input = input[searchIndex..].Trim();

            }


        }


    }

}
