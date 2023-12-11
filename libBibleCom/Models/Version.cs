using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibBibleDotCom.Models;

public abstract record VersionBase
{
    public required uint Id { get; init; }
    public required string Abbreviation { get; init; }
    public required string LocalAbbreviation { get; init; }
    public required string Title { get; init; }
    public required string LocalTitle { get; init; }
    public required bool HasAudio { get; init; }
    public required uint AudioCount { get; init; }
    public required bool HasText { get; init; }
    public required VersionLanguage Language { get; init; }
    public required OnlinePlatformAvailability Platforms { get; init; }
    public required Offline OfflineInfo { get; init; }
    public required uint? MetadataBuild { get; init; }
}

public record Build
{
    public required uint Min { get; init; }
    public required uint Max { get; init; }
}

public record Offline
{
    public required Build Build { get; init; }
    public required Uri Url { get; init; }
    public required PlatformAvailability Platforms { get; init; }
    public required bool AlwaysAllowUpdates { get; init; }
    public required bool AllowRedownload { get; init; }
    public required bool RequireEmailAgreement { get; init; }
    public required uint? AgreementVersion { get; init; }
}

public record Version : VersionBase
{
    public required uint? PublisherId { get; init; }

}

public record TextHtml
{ 
    public required string? Text { get; init; }
    public required string? Html { get; init; }
}

public record Copyright
{
    public required TextHtml Short { get; init; }
    public required TextHtml Long { get; init; }
}

public record ReaderFooter
{
    public required TextHtml Text { get; init; }
    public required Uri? Url { get; init; }
}

public record Publisher
{
    public required uint Id { get; init; }
    public required string EnglishName { get; init; }
    public required string LocalName { get; init; }
    public required Uri? Url { get; init; }
    public required string? Description { get; init; }
}

public enum Canon
{
    OldTestament,
    NewTestament,
    Apocrypha
}

public record Chapter
{
    public required bool TableOfContents { get; init;  }
    public required bool Canonical { get; init; }
    public required string Name { get; init; }
    public required string USFM { get; init; }
}

public record Book
{
    public required bool Text { get; init; }
    public required Canon Canon { get; init; }
    public required string USFM { get; init; }
    public required string Name { get; init; }
    public required string LongName { get; init; }
    public required string Abbreviation { get; init; }
    public required bool Audio { get; init; }
    public required List<Chapter> Chapters { get; init; }
}


public enum CanonCompleteness
{
    Complete,
    Partial,
    No
}


public record VersionMetadata : VersionBase
{
    public required Copyright Copyright { get; init; }
    public required ReaderFooter ReaderFooter { get; init; }
    public required Publisher? Publisher { get; init; }
    public required List<Book> Books { get; init; }
    

    public CanonCompleteness HasOldTestament => 
        Books.Count(b => b.Canon == Canon.OldTestament) == 39 ? CanonCompleteness.Complete :
        Books.Any(b => b.Canon == Canon.OldTestament) ? CanonCompleteness.Partial :
        CanonCompleteness.No;

    public CanonCompleteness HasNewTestament =>
        Books.Count(b => b.Canon == Canon.NewTestament) == 27 ? CanonCompleteness.Complete :
        Books.Any(b => b.Canon == Canon.NewTestament) ? CanonCompleteness.Partial :
        CanonCompleteness.No;

    public bool HasApocrypha => Books.Any(b => b.Canon == Canon.Apocrypha);
    public bool HasSomeOldTestament => Books.Any(b => b.Canon == Canon.OldTestament);
    public bool HasSomeNewTestament => Books.Any(b => b.Canon == Canon.NewTestament);

}