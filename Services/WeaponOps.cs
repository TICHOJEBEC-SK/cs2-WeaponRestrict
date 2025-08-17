using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using WeaponRestrict.Core;
using WeaponRestrict.Config;

namespace WeaponRestrict.Services;

internal static class WeaponOps
{
    public static string ResolveClassname(RestrictedWeaponsConfig cfg, int defIndex, string? item)
    {
        if (cfg.DefIndexToClass.TryGetValue(defIndex, out var cls)) return cls;
        if (!string.IsNullOrWhiteSpace(item))
        {
            var s = item.Trim().ToLowerInvariant();
            if (!s.StartsWith("weapon_")) s = "weapon_" + s;
            return s;
        }
        return string.Empty;
    }

    private static void SwitchToKnifeNextFrame(CCSPlayerController player)
    {
        try { Server.NextFrame(() => { try { player?.ExecuteClientCommand("slot3"); } catch { } }); } catch { }
    }
    
    private static void ForceDropByClass(BasePlugin plugin, CCSPlayerController player, string classname)
    {
        ActiveLock.Start(plugin, player, classname, totalSeconds: 0.30f, stepSeconds: 0.06f);

        plugin.AddTimer(0.02f, () =>
        {
            try { if (player.IsValid) player.ExecuteClientCommand("slot3"); } catch { }
        });
        plugin.AddTimer(0.06f, () =>
        {
            try { if (player.IsValid) player.ExecuteClientCommand($"use {classname}"); } catch { }
        });
        plugin.AddTimer(0.12f, () =>
        {
            try { if (player.IsValid) player.ExecuteClientCommand("drop"); } catch { }
        });
        plugin.AddTimer(0.18f, () =>
        {
            try { if (player.IsValid) player.ExecuteClientCommand("slot3"); } catch { }
        });
    }
    

    public static void EnqueueRestricted(BasePlugin plugin, CCSPlayerController player, string classname, RoundSerial round, int roundSerial)
    {
        RestrictQueue.Enqueue(player.SteamID, classname, roundSerial, out var start);
        if (start)
            RunWorker(plugin, player, classname, round);
    }
    
    private static void RunWorker(BasePlugin plugin, CCSPlayerController player, string classname, RoundSerial round)
    {
        void OneTurn()
        {
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            { RestrictQueue.FinishTurn(player?.SteamID ?? 0, classname, true); return; }

            if (!RestrictQueue.TryDequeue(player.SteamID, classname, out var serial, out var isLast))
                return;
            
            SwitchToKnifeNextFrame(player);
            ActiveLock.Start(plugin, player, classname, totalSeconds: 0.60f, stepSeconds: 0.06f);
            
            plugin.AddTimer(0.20f, () =>
            {
                if (round.Current != serial) return;
                if (!player.IsValid) return;
                ForceDropByClass(plugin, player, classname);
            });
            
            plugin.AddTimer(0.80f, () =>
            {
                RestrictQueue.FinishTurn(player.SteamID, classname, isLast);
                if (!isLast)
                    RunWorker(plugin, player, classname, round);
            });
        }

        plugin.AddTimer(0.10f, OneTurn);
    }
}
