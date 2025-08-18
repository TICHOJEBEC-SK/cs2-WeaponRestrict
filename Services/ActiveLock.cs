using System.Collections.Concurrent;
using CounterStrikeSharp.API.Core;

namespace WeaponRestrict.Services;

internal static class ActiveLock
{
    private sealed class Guard { public int TicksLeft; }

    private static readonly ConcurrentDictionary<(ulong steamId, string weapon), Guard> Guards = new();

    public static void Start(BasePlugin plugin, CCSPlayerController player, string classname, float totalSeconds = 1.20f, float stepSeconds = 0.06f)
    {
        if (!player.IsValid) return;

        var key = (player.SteamID, classname);
        var ticks = (int)Math.Ceiling(totalSeconds / stepSeconds);

        Guards.AddOrUpdate(
            key,
            _ => new Guard { TicksLeft = ticks },
            (_, g) => { g.TicksLeft = Math.Max(g.TicksLeft, ticks); return g; }
        );

        RunLoop(plugin, player, classname, stepSeconds);
    }

    private static void RunLoop(BasePlugin plugin, CCSPlayerController player, string classname, float stepSeconds)
    {
        var key = (player.SteamID, classname);
        if (!Guards.TryGetValue(key, out _)) return;

        plugin.AddTimer(stepSeconds, () =>
        {
            try
            {
                if (!Guards.TryGetValue(key, out var g)) return;
                if (g.TicksLeft <= 0 || !player.IsValid ||
                    player.Connected != PlayerConnectedState.PlayerConnected)
                {
                    Guards.TryRemove(key, out _);
                    return;
                }

                var pawn = player.PlayerPawn?.Value;
                var active = pawn?.WeaponServices?.ActiveWeapon?.Value;
                var activeName = active?.DesignerName ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(activeName) &&
                    activeName.Equals(classname, StringComparison.OrdinalIgnoreCase))
                {
                    TryForceDrop(player, classname);
                }

                g.TicksLeft--;
                RunLoop(plugin, player, classname, stepSeconds);
            }
            catch
            {
                Guards.TryRemove(key, out _);
            }
        });
    }

    private static void TryForceDrop(CCSPlayerController p, string classname)
    {
        try
        {
            p.ExecuteClientCommand("slot3");
            p.ExecuteClientCommand($"use {classname}");
            p.ExecuteClientCommand("drop");
            p.ExecuteClientCommand("slot3");
        }
        catch
        {
            //
        }
    }
}
