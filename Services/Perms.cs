using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using WeaponRestrict.Config;

namespace WeaponRestrict.Services;

internal static class Perms
{
    public static bool HasBypass(RestrictedWeaponsConfig cfg, CCSPlayerController player)
    {
        if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            return false;

        var list = cfg.BypassPermissions;
        if (list.Count == 0) return false;
        
        foreach (var p in list)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(p) && AdminManager.PlayerHasPermissions(player, p))
                    return true;
            }
            catch
            {
                //
            }
        }
        return false;
    }
}