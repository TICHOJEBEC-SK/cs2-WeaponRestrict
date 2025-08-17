using System.Collections.Concurrent;

namespace WeaponRestrict.Core;

internal static class RestrictQueue
{
    private sealed class Q
    {
        public int Pending;
        public bool Running;
        public int SerialAtStart;
    }

    private static readonly ConcurrentDictionary<(ulong steam, string weapon), Q> Queues = new();

    internal static void Enqueue(ulong steamId, string weapon, int roundSerial, out bool shouldStart)
    {
        var key = (steamId, weapon);
        var q = Queues.GetOrAdd(key, _ => new Q());
        q.Pending++;
        q.SerialAtStart = roundSerial;
        shouldStart = !q.Running;
        if (shouldStart) q.Running = true;
    }

    internal static bool TryDequeue(ulong steamId, string weapon, out int serial, out bool isLast)
    {
        var key = (steamId, weapon);
        serial = 0;
        isLast = false;

        if (!Queues.TryGetValue(key, out var q)) return false;

        serial = q.SerialAtStart;
        if (q.Pending <= 0)
        {
            q.Running = false;
            Queues.TryRemove(key, out _);
            return false;
        }

        q.Pending--;
        isLast = (q.Pending == 0);
        return true;
    }

    internal static void FinishTurn(ulong steamId, string weapon, bool isLast)
    {
        var key = (steamId, weapon);
        if (!Queues.TryGetValue(key, out var q)) return;

        if (!isLast) return;
        q.Running = false;
        Queues.TryRemove(key, out _);
    }
}