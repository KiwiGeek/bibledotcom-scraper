using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBibleDotCom.Models;
public record Version
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
    public required uint? PublisherId { get; init; }

    public required OnlinePlatformAvailability Platforms { get; init; }

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

    public required Offline OfflineInfo { get; init; }
    public required uint? MetadataBuild { get; init; }
}
