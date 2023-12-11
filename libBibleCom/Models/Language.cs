using System.Text.Json.Serialization;

namespace LibBibleDotCom.Models;

public abstract record LanguageBase
{
    public required string? Iso639_1 { get; init; }
    public required string Iso639_3 { get; init; }
    public required string Tag { get; init; }
    public required string LocalName { get; init; }
    public required string EnglishName { get; init; }
    public required TextDirections TextDirection { get; init; }

}

public record VersionLanguage : LanguageBase
{
    public object? SecondaryLanguageTags { get; init; }
}

public record InfoLanguage : LanguageBase
{
    public required uint Id { get; init; }
    public required bool HasAudio { get; init; }
    public required bool HasText { get; init; }
    public required string? Font { get; init; }
}