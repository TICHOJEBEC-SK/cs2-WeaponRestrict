using CounterStrikeSharp.API.Core;
using WeaponRestrict.Services;

namespace WeaponRestrict.Core;

internal sealed class Restrictor : IDisposable
{
    private readonly WeaponRestrict _plugin;
    private readonly DelayedWork _debounce;
    private readonly RoundSerial _round;

    public Restrictor(WeaponRestrict plugin, RoundSerial round)
    {
        _plugin = plugin;
        _round = round;
        _debounce = new DelayedWork(plugin, delayMs: 220);
    }

    public void Dispose() => _debounce.Dispose();

    public HookResult OnItemEquip(EventItemEquip ev, GameEventInfo info)
    {
        var player = ev.Userid;
        if (player == null || !player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            return HookResult.Continue;

        var defIndex = (int)(ev?.Defindex ?? 0);
        var itemName = ev?.Item;

        _debounce.Schedule(player, () => ProcessEquipDeferred(player, defIndex, itemName));
        return HookResult.Continue;
    }

    private void ProcessEquipDeferred(CCSPlayerController player, int defIndex, string? item)
    {
        if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected) return;

        var cfg = _plugin.Config;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null || !pawn.IsValid) return;

        var team = (int)pawn.TeamNum;
        var classname = WeaponOps.ResolveClassname(cfg, defIndex, item);
        if (string.IsNullOrWhiteSpace(classname)) return;
        
        var playerCount = WorldQuery.CountPlayers(cfg, team);
        if (!RuleBook.TryGetLimit(cfg, classname, playerCount, team, out var limit)) return;
        
        bool isInNoBypassList = (cfg.NoBypassWeapons?.Any(w => w.Equals(classname, StringComparison.OrdinalIgnoreCase)) ?? false);
        bool hardBan = (limit == 0 && !cfg.BypassAllowedWhenLimitIsZero) || isInNoBypassList;
        
        if (!hardBan && Perms.HasBypass(cfg, player))
            return;
        
        var currentCount = WorldQuery.CountWeaponAcrossPlayers(cfg, classname, team);
        if (limit == -1 || currentCount <= limit) return;
        
        var msgTemplate = (cfg.TypeWeapons == 2 ? cfg.Phrases.BlockTeam : cfg.Phrases.Block);
        var msg = msgTemplate
            .Replace("{weapon}", cfg.Phrases.Pretty(classname))
            .Replace("{limit}", limit.ToString());
        Notify.Info(player, cfg.ChatPrefix, msg);

        var roundSerial = _round.Current;
        
        ActiveLock.Start(_plugin, player, classname);
        _plugin.AddTimer(0.05f, () =>
        {
            WeaponOps.EnqueueRestricted(_plugin, player, classname, _round, roundSerial);
        });
    }
}