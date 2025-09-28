using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using WeaponRestrict.Config;

namespace WeaponRestrict.Services;

internal static class WorldQuery
{
    public static int CountPlayers(RestrictedWeaponsConfig cfg, int playerTeam)
    {
        int count = 0;
        foreach (var p in Utilities.GetPlayers())
        {
            if (!p.IsValid || p.Connected != PlayerConnectedState.PlayerConnected) continue;

            var pawn = p.PlayerPawn?.Value;
            if (pawn == null || !pawn.IsValid) continue;

            var t = (int)pawn.TeamNum;
            if (!cfg.CountSpectators && t < 2) continue;

            if (cfg.TypePlayers == 1) count++;
            else if (cfg.TypePlayers == 2 && t == playerTeam) count++;
        }
        return count;
    }

    public static int CountWeaponAcrossPlayers(RestrictedWeaponsConfig cfg, string classname, int playerTeam)
    {
        int count = 0;
        foreach (var p in Utilities.GetPlayers())
        {
            if (!p.IsValid || p.Connected != PlayerConnectedState.PlayerConnected) continue;

            var pawn = p.PlayerPawn?.Value;
            if (pawn == null || !pawn.IsValid) continue;

            if (cfg.TypeWeapons == 2 && (int)pawn.TeamNum != playerTeam) continue;

            count += CountOnPlayer(p, classname);
        }
        return count;
    }

    private static int CountOnPlayer(CCSPlayerController player, string classname)
    {
        var pawn = player.PlayerPawn?.Value;
        var ws = pawn?.WeaponServices;
        var list = ws?.MyWeapons;
        if (list == null) return 0;

        int c = 0;
        foreach (var h in list)
        {
            var weap = h.Value;
            if (weap == null || !weap.IsValid) continue;
            var name = weap.DesignerName;
            if (!string.IsNullOrEmpty(name) &&
                name.Equals(classname, StringComparison.OrdinalIgnoreCase))
            {
                c++;
            }
        }
        return c;
    }
}