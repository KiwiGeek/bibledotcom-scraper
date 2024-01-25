using OneOf;
using System.Text.RegularExpressions;

namespace LibBibleDotCom.Models;
public class Token
{
    private readonly List<OneOf<string, Token>> _content = [];
    private readonly string _tag;
    private readonly string _class;
    private readonly List<KeyValuePair<string, string>> _attributes = [];

    public string ToXml()
    {

        // for now, this is going to just build an XML string. when we're done, we'll actually make it return a xml document.

        if (!IsFullyUnderstood()) { throw new InvalidDataException("This token is not fully understood"); }
        string content = string.Empty;
        foreach(OneOf<string, Token> item in _content)
        {
            if (item.IsT0) 
            {
                content += item.AsT0;
            }
            else
            {
                string tag = string.Empty;
                if (IsVerse) { tag = "Verse"; }
                if (IsRoot) { tag = "Root"; }
                if (IsVersion) { tag = "Version"; }
                if (IsBook) { tag = "Book"; }
                if (IsChapter) { tag = "Chapter"; }
                if (IsLabel) { tag = "Label"; } 
                if (IsSection) { tag = "Section"; }
                if (IsHeading) { tag = "Heading"; }
                if (IsParagraph) { tag = "Paragraph"; }
                if (IsContent) { tag = "Content"; }
                if (IsNote) { tag = "Note"; }
                if (IsNoteBody) { tag = "NoteBody"; }
                if (IsItalics) { tag = "Italics"; }
                if (!IsNote)            // content we don't care about
                {
                    string attributes = string.Empty;
                    foreach (KeyValuePair<string, string> attrib in _attributes)
                    {
                        attributes += $" {attrib.Key}=\"{attrib.Value}\"";
                    }
                    string tempContent = $"<{tag}{attributes}>{item.AsT1.ToXml()}</{tag}>";
                    // if this tempContent is just an opening and closing tag, we don't need to return it.
                    if (tempContent != $"<{tag}></{tag}>") 
                    { 
                        content = content.Trim() + tempContent.Trim();
                    }
                }

            }
        }
        if (content.StartsWith("<Root>"))
        {
            return content[6..^7];
        }
        return content;
    }

    private bool IsFullyUnderstood()
    {
        // we are fully understood if:
        // - our own tag/class is understood
        // - all of our children are understood.

        bool understood = IsRoot || IsVersion || IsBook || IsChapter || IsLabel || IsSection || IsHeading || IsParagraph
                        || IsVerse || IsContent || IsNote || IsNoteBody || IsItalics;
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
    private bool IsLabel => (_tag == "div" || _tag == "span") && _class.StartsWith("class=\"label");
    private bool IsSection => _tag == "div" && _class == "class=\"s\"";
    private bool IsHeading => _tag == "span" && _class == "class=\"heading\"";
    private bool IsParagraph => _tag == "div" && _class == "class=\"p\"";
    private bool IsVerse => _tag == "span" && _class.StartsWith("class=\"verse");
    private bool IsContent => _tag == "span" && _class.StartsWith("class=\"content");
    private bool IsNote => _tag == "span" && _class.StartsWith("class=\"note");
    private bool IsNoteBody => _tag == "span" && _class.StartsWith("class=\" body");
    private bool IsItalics => _tag == "span" && _class == "class=\"it\"";

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
        else 
            return [];
    }

    public Token(string tag, string @class, string input)
    {

        _tag = tag;
        _class = @class;

        if (IsVersion)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                new Regex(".*data-vid=\\\"(?<id>\\d*)\\\".*data-iso6393=\\\"(?<lang>.*)\\\""),
                ("id", "id"),
                ("lang", "lang")));
        } 
        else if (IsBook)
        {
            _attributes.AddRange(GetAttributesFromRegex(
                new Regex(".*book bk(?<book>[1-3A-Z]*)\\\""),
                ("book", "id")));
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
                string openingTag = input[..(input.IndexOf(">", StringComparison.Ordinal) + 1)];
                string openTagType = openingTag.Contains(' ') ? openingTag[1..openingTag.IndexOf(' ')] : openingTag[1..^1];
                string closingTagToFind = $"</{openTagType}>";
                string openTagToFind = $"<{openTagType}";
                bool done = false;
                int searchIndex = openingTag.Length - 1;
                int tagCounter = 1;

                while (!done)
                {

                    Console.WriteLine($"Looking for {openTagToFind} or {closingTagToFind}");
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
                            Console.WriteLine($"The next tag was an opening Tag, at {openingTagCharIndex}. We are {tagCounter} tags deep.");
                    }
                    else
                    {
                        // presumably, all being well with the world, we found a closing tag. So decrement 
                        // the counter, and see if we're done.
                        tagCounter--;
                        searchIndex = closingTagCharIndex + 1;
                           Console.WriteLine($"The next tag was a closing Tag, at {closingTagCharIndex}. We are {tagCounter} tags deep.");
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
                Token newToken = new (openTagType, classInfo, content);
                _content.Add(newToken);
                input = input[searchIndex..].Trim();

            }


        }


    }

}
