using CounterStrikeSharp.API;
using WeaponRestrict.Config;

namespace WeaponRestrict.Core;

internal static class RuleBook
{
    public static bool TryGetLimit(RestrictedWeaponsConfig cfg, string classname, int playerCount, int team, out int limit)
    {
        limit = 0;
        var rulesForMap = cfg.ResolveRulesForMap(Server.MapName);
        if (rulesForMap.Count == 0) return false;

        var best = rulesForMap.Keys.Where(k => k <= playerCount).DefaultIfEmpty(-1).Max();
        if (best < 0) return false;

        var limits = rulesForMap[best];
        return limits.TryGetValue(classname, out limit);
    }
}