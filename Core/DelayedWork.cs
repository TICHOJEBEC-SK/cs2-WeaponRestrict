using System.Collections.Concurrent;
using CounterStrikeSharp.API.Core;

namespace WeaponRestrict.Core;

internal sealed class DelayedWork : IDisposable
{
    private readonly BasePlugin _plugin;
    private readonly int _delayMs;

    public DelayedWork(BasePlugin plugin, int delayMs) { _plugin = plugin; _delayMs = delayMs; }

    private readonly ConcurrentDictionary<ulong, int> _tokens = new();
    private static readonly ConcurrentDictionary<string, long> Seen = new();

    public void Schedule(CCSPlayerController player, Action work)
    {
        var sid = player.SteamID;
        var my = _tokens.AddOrUpdate(sid, 1, (_, v) => v + 1);

        _plugin.AddTimer(_delayMs / 1000f, () =>
        {
            if (!_tokens.TryGetValue(sid, out var current) || current != my) return;
            if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected) return;

            try { work(); }
            catch
            {
                //
            }
            finally
            {
                _tokens.TryGetValue(sid, out var chk);
                if (chk == my) _tokens.TryRemove(sid, out _);
            }
        });
    }

    public static bool SeenRecently(ulong steamId, string weapon, int windowMs)
    {
        var key = $"{steamId}:{weapon}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (Seen.TryGetValue(key, out var last) && (now - last) < windowMs) return true;
        Seen[key] = now;

        if (Seen.Count > 2048)
        {
            foreach (var kv in Seen)
                if ((now - kv.Value) > 3000)
                    Seen.TryRemove(kv.Key, out _);
        }
        return false;
    }

    public void Dispose() => _tokens.Clear();
}