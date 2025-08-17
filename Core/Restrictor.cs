using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using WeaponRestrict.Config;
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
        _debounce = new DelayedWork(plugin, 220);
    }

    public void Dispose()
    {
        _debounce.Dispose();
    }

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

        var isInNoBypassList = cfg.NoBypassWeapons?.Any(w => w.Equals(classname, StringComparison.OrdinalIgnoreCase)) ??
                               false;
        var hardBan = (limit == 0 && !cfg.BypassAllowedWhenLimitIsZero) || isInNoBypassList;

        if (!hardBan && Perms.HasBypass(cfg, player))
            return;

        var currentCount = WorldQuery.CountWeaponAcrossPlayers(cfg, classname, team);
        if (limit == -1 || currentCount <= limit) return;

        var msgTemplate = cfg.TypeWeapons == 2 ? cfg.Phrases.BlockTeam : cfg.Phrases.Block;
        var msg = msgTemplate
            .Replace("{weapon}", cfg.Phrases.Pretty(classname))
            .Replace("{limit}", limit.ToString());
        Notify.Info(player, cfg.ChatPrefix, msg);
        
        var roundSerial = _round.Current;
        ActiveLock.Start(_plugin, player, classname);
        _plugin.AddTimer(0.05f,
            () => { WeaponOps.EnqueueRestricted(_plugin, player, classname, _round, roundSerial); });
    }
    
    public HookResult OnItemPurchase(EventItemPurchase ev, GameEventInfo info)
    {
        var player = ev.Userid;
        if (player == null || !player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            return HookResult.Continue;
        
        var raw = ev.Weapon;
        var classname = string.IsNullOrWhiteSpace(raw)
            ? string.Empty
            : raw.StartsWith("weapon_", StringComparison.OrdinalIgnoreCase)
                ? raw.Trim()
                : "weapon_" + raw.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(classname))
            return HookResult.Continue;

        TryAutoSellIfRestricted(player, classname);
        return HookResult.Continue;
    }


    private void TryAutoSellIfRestricted(CCSPlayerController player, string classname)
    {
        if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected) return;

        var cfg = _plugin.Config;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null || !pawn.IsValid) return;

        var team = (int)pawn.TeamNum;

        var rulesForMap = cfg.ResolveRulesForMap(Server.MapName);
        if (rulesForMap.Count == 0) return;

        var playerCount = WorldQuery.CountPlayers(cfg, team);
        if (!RuleBook.TryGetLimit(cfg, classname, playerCount, team, out var limit)) return;

        var isInNoBypassList = cfg.NoBypassWeapons?.Any(w => w.Equals(classname, StringComparison.OrdinalIgnoreCase)) ??
                               false;
        var hardBan = (limit == 0 && !cfg.BypassAllowedWhenLimitIsZero) || isInNoBypassList;

        if (!hardBan && Perms.HasBypass(cfg, player)) return;

        var currentCount = WorldQuery.CountWeaponAcrossPlayers(cfg, classname, team);
        var violates = limit == 0 || (limit != -1 && currentCount >= limit);
        if (!violates) return;

        var price = 0;
        if (WeaponDefaults.DefaultWeaponPrices().TryGetValue(classname, out var p))
            price = Math.Max(0, p);

        RemoveWeaponByClass(player, classname);
        TrySwitchToKnife(player);

        if (price > 0)
        {
            TryAddMoney(player, price);
            var msg = cfg.Phrases.SellRefund
                .Replace("{weapon}", cfg.Phrases.Pretty(classname))
                .Replace("{price}", price.ToString());
            Notify.Info(player, cfg.ChatPrefix, msg);
        }
        else
        {
            var msg = cfg.Phrases.SellRemoved
                .Replace("{weapon}", cfg.Phrases.Pretty(classname));
            Notify.Info(player, cfg.ChatPrefix, msg);
        }
    }


    private static void RemoveWeaponByClass(CCSPlayerController player, string classname)
    {
        try
        {
            var ws = player.PlayerPawn?.Value?.WeaponServices;
            var list = ws?.MyWeapons;
            if (list == null || list.Count == 0) return;

            foreach (var kv in list.ToArray())
            {
                var weap = kv.Value;
                if (weap == null || !weap.IsValid) continue;
                if (string.Equals(weap.DesignerName, classname, StringComparison.OrdinalIgnoreCase)) weap.Remove();
            }
        }
        catch
        {
        }
    }

    private static void TrySwitchToKnife(CCSPlayerController player)
    {
        try
        {
            player?.ExecuteClientCommand("slot3");
        }
        catch
        {
        }
    }

    private static void TryAddMoney(CCSPlayerController player, int amount)
    {
        try
        {
            var money = player.InGameMoneyServices?.Account ?? 0;
            var target = Math.Clamp(money + amount, 0, 16000);
            if (player.InGameMoneyServices != null)
                player.InGameMoneyServices.Account = target;
        }
        catch
        {
        }
    }
}