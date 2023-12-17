using OneOf;

namespace LibBibleDotCom.Models;
public class Token
{
    public List<OneOf<string, Token>> Content = new();
    public string Tag { get; set; } = "";
    public string Class { get; set; } = "";

    public string ToXml()
    {
        if (!IsFullyUnderstood()) { throw new InvalidDataException("This token is not fully understood"); }
        string content = string.Empty;
        foreach(OneOf<string, Token> item in Content)
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
                    string tempContent = $"<{tag}>{item.AsT1.ToXml()}</{tag}>";
                    // if this tempContent is just an opening and closeing tag, we don't need to reutnrin it.
                    if (tempContent != $"<{tag}></{tag}>") 
                    { 
                        content += tempContent;
                    }
                }

            }
        }
        return content;
    }

    public bool IsFullyUnderstood()
    {
        // we are fully understood if:
        // a) our own tag/class is understood
        // and b) all of our children are understood.

        bool understood = IsRoot || IsVersion || IsBook || IsChapter || IsLabel || IsSection || IsHeading || IsParagraph
                        || IsVerse || IsContent || IsNote || IsNoteBody || IsItalics;
        if (!understood) 
        { 
            Console.WriteLine($"I don't understand {Tag} {Class}");
            return false; 
        }

        foreach (OneOf<string, Token> item in Content)
        {
            if (item.IsT0) { continue; }
            if (!item.AsT1.IsFullyUnderstood()) { return false; }
        }
        return true;

    }

    public bool IsRoot => Tag == "root";
    public bool IsVersion => Tag == "div" && Class.StartsWith("class=\"version");
    public bool IsBook => Tag == "div" && Class.StartsWith("class=\"book");
    public bool IsChapter => Tag == "div" && Class.StartsWith("class=\"chapter");
    public bool IsLabel => (Tag == "div" || Tag == "span") && Class.StartsWith("class=\"label");
    public bool IsSection => Tag == "div" && Class == "class=\"s\"";
    public bool IsHeading => Tag == "span" && Class == "class=\"heading\"";
    public bool IsParagraph => Tag == "div" && Class == "class=\"p\"";
    public bool IsVerse => Tag == "span" && Class.StartsWith("class=\"verse");
    public bool IsContent => Tag == "span" && Class.StartsWith("class=\"content");
    public bool IsNote => Tag == "span" && Class.StartsWith("class=\"note");
    public bool IsNoteBody => Tag == "span" && Class.StartsWith("class=\" body");
    public bool IsItalics => Tag == "span" && Class == "class=\"it\"";

    public Token(string tag, string @class, string input)
    {

        Tag = tag;
        Class = @class;

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
                    Content.Add(input);
                    input = "";
                }
                else
                {
                    // there is a tag, so the text is everything up to that tag
                    Content.Add(input[..nextTagIndex]);
                    input = input[nextTagIndex..];
                }
            }
            else
            {
                // it's a tag
                // temporarily, let's jsut drop that first character.
                string openingTag = input[0..(input.IndexOf(">") + 1)];
                string openTagType = openingTag.Contains(' ') ? openingTag[1..openingTag.IndexOf(' ')] : openingTag[1..^1];
                string closingTagToFind = $"</{openTagType}>";
                string openTagToFind = $"<{openTagType}";
                bool done = false;
                int searchIndex = openingTag.Length - 1;
                int tagCounter = 1;

                while (!done)
                {

                    //Console.WriteLine($"Looking for {openTagToFind} or {closingTagToFind}");
                    int openingTagCharIndex = input.IndexOf(openTagToFind, searchIndex);
                    int closingTagCharIndex = input.IndexOf(closingTagToFind, searchIndex);

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
                        //    Console.WriteLine($"The next tag was an opening Tag, at {openingTagCharIndex}. We are {tagCounter} tags deep.");
                    }
                    else
                    {
                        // presumably, all being well with the world, we found a closing tag. So decrement 
                        // the counter, and see if we're done.
                        tagCounter--;
                        searchIndex = closingTagCharIndex + 1;
                        //   Console.WriteLine($"The next tag was a closing Tag, at {closingTagCharIndex}. We are {tagCounter} tags deep.");
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
                Token newToken = new Token(openTagType, classInfo, content);
                Content.Add(newToken);
                input = input[searchIndex..].Trim();

            }


        }


    }

}
