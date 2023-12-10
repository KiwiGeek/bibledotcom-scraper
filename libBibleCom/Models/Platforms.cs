using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBibleDotCom.Models;
public record PlatformAvailability
{
    public required bool Windows8 { get; init; }
    public required bool WindowsPhone7 { get; init; }
#pragma warning disable IDE1006 // Naming Styles
    public required bool iOS { get; init; }
#pragma warning restore IDE1006 // Naming Styles
    public required bool BlackBerry { get; init; }
    public required bool Android { get; init; }
}

public record OnlinePlatformAvailability : PlatformAvailability
{
    public required bool Facebook { get; init; }
}