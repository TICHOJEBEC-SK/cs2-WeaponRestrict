using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace WeaponRestrict.Services;

internal static class Notify
{
    public static void Info(CCSPlayerController p, string prefix, string msg)
        => p.PrintToChat($" {ChatColors.Red}{prefix}{ChatColors.White} {msg}");
}