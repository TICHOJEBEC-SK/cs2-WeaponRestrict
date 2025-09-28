using System.Collections.Concurrent;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using WeaponRestrict.Services;

namespace WeaponRestrict.Core;

internal sealed class Restrictor : IDisposable
{
    private readonly WeaponRestrict _plugin;
    
    private static readonly ConcurrentDictionary<string, long> Seen = new();
    
    private const int DebounceWindowMs = 1500;

    public Restrictor(WeaponRestrict plugin)
    {
        _plugin = plugin;
        VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Hook(OnWeaponCanAcquire, HookMode.Pre);
    }

    public void Dispose()
    {
        VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Unhook(OnWeaponCanAcquire, HookMode.Pre);
    }

    private static bool SeenRecently(ulong steamId, string weapon, int windowMs)
    {
        var key = $"{steamId}:{weapon}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (Seen.TryGetValue(key, out var last) && (now - last) < windowMs)
            return true;

        Seen[key] = now;
        
        if (Seen.Count > 4096)
        {
            foreach (var kv in Seen)
            {
                if ((now - kv.Value) > (windowMs * 4))
                    Seen.TryRemove(kv.Key, out _);
            }
        }

        return false;
    }

    private HookResult OnWeaponCanAcquire(DynamicHook hook)
    {
        var player = hook.GetParam<CCSPlayer_ItemServices>(0)
            ?.Pawn.Value?.Controller.Value?.As<CCSPlayerController>();

        if (player == null || !player.IsValid || !player.PawnIsAlive)
            return HookResult.Continue;
        
        var econ = hook.GetParam<CEconItemView>(1);
        if (econ == null) return HookResult.Continue;

        var vdata = VirtualFunctions
            .GetCSWeaponDataFromKeyFunc
            .Invoke(-1, econ.ItemDefinitionIndex.ToString());

        if (vdata == null) return HookResult.Continue;
        var classname = vdata.Name;
        if (string.IsNullOrWhiteSpace(classname)) return HookResult.Continue;

        var cfg = _plugin.Config;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null || !pawn.IsValid) return HookResult.Continue;

        var team = (int)pawn.TeamNum;
        var playerCount = WorldQuery.CountPlayers(cfg, team);

        if (!RuleBook.TryGetLimit(cfg, classname, playerCount, team, out var limit))
            return HookResult.Continue;

        var isInNoBypassList = cfg.NoBypassWeapons?.Any(w =>
            w.Equals(classname, StringComparison.OrdinalIgnoreCase)) ?? false;

        var hardBan = (limit == 0 && !cfg.BypassAllowedWhenLimitIsZero) || isInNoBypassList;
        if (!hardBan && Perms.HasBypass(cfg, player))
            return HookResult.Continue;

        var currentCount = WorldQuery.CountWeaponAcrossPlayers(cfg, classname, team);
        var violates = limit == 0 || (limit != -1 && currentCount >= limit);
        if (!violates) return HookResult.Continue;
        
        var steam = player.SteamID;
        var suppressNotify = SeenRecently(steam, classname, DebounceWindowMs);

        if (!suppressNotify)
        {
            var msgTemplate = cfg.TypeWeapons == 2 ? cfg.Phrases.BlockTeam : cfg.Phrases.Block;
            var msg = msgTemplate
                .Replace("{weapon}", cfg.Phrases.Pretty(classname))
                .Replace("{limit}", limit.ToString());
            Notify.Info(player, cfg.ChatPrefix, msg);

            SoundService.TryEmitSafe(player, cfg.BlockSound);
        }
        
        var method = hook.GetParam<AcquireMethod>(2);
        if (method != AcquireMethod.PickUp)
        {
            hook.SetReturn(AcquireResult.AlreadyOwned);
        }
        else
        {
            hook.SetReturn(AcquireResult.InvalidItem);
        }

        return HookResult.Stop;
    }
}
