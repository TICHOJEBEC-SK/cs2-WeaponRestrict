using System.Collections.Concurrent;
using CounterStrikeSharp.API.Core;

namespace WeaponRestrict.Services;

internal static class SoundService
{
    private static readonly ConcurrentDictionary<string, byte> Warned = new();

    private static bool LooksLikeDangerousScene(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return true;

        var hasSlash = s.Contains('/') || s.Contains('\\');
        var lower = s.ToLowerInvariant();
        var looksLikeFile = lower.EndsWith(".vsnd") || lower.EndsWith(".vsnd_c") || lower.Contains("/");

        if (!hasSlash && !looksLikeFile) return true;

        if (!hasSlash && s.Contains('.') && !looksLikeFile) return true;

        return false;
    }

    public static void TryEmitSafe(CCSPlayerController player, string? sound)
    {
        if (string.IsNullOrWhiteSpace(sound)) return;

        var s = sound.Trim();
        if (LooksLikeDangerousScene(s))
        {
            if (Warned.TryAdd(s, 1))
                Console.WriteLine($"[WeaponRestrict] Skipping suspicious RestrictedSound '{s}'. Provide valid sound event/path (e.g., weapons/.../*.vsnd_c).");
            return;
        }

        try
        {
            player.PlayerPawn?.Value?.EmitSound(s);
        }
        catch { /**/ }
    }
}