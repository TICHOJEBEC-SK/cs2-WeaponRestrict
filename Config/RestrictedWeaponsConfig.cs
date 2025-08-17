using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace WeaponRestrict.Config;

public class RestrictedWeaponsConfig : BasePluginConfig
{
    [JsonPropertyName("ChatPrefix")] public string ChatPrefix { get; set; } = "[RW]";
    [JsonPropertyName("TypePlayers")] public int TypePlayers { get; set; } = 1;
    [JsonPropertyName("TypeWeapons")] public int TypeWeapons { get; set; } = 1;
    [JsonPropertyName("CountSpectators")] public bool CountSpectators { get; set; } = false;
    [JsonPropertyName("BypassPermissions")] public List<string> BypassPermissions { get; set; } = new() { "@vip/restrict", "@css/root" };
    [JsonPropertyName("BypassAllowedWhenLimitIsZero")] public bool BypassAllowedWhenLimitIsZero { get; set; } = false;
    [JsonPropertyName("NoBypassWeapons")] public List<string> NoBypassWeapons { get; set; } = new();
    [JsonPropertyName("Phrases")] public PhrasesSection Phrases { get; set; } = new();
    [JsonPropertyName("DefIndexToClass")] public Dictionary<int, string> DefIndexToClass { get; set; } = WeaponDefaults.DefaultDefIndexToClass();
    [JsonPropertyName("Rules")] public Dictionary<string, Dictionary<int, Dictionary<string, int>>> Rules { get; set; } = WeaponDefaults.DefaultRules();

    public Dictionary<int, Dictionary<string, int>> ResolveRulesForMap(string? map)
    {
        map ??= string.Empty;
        if (Rules.TryGetValue(map, out var specific)) return specific;
        if (Rules.TryGetValue("all", out var all)) return all;
        return new();
    }
}

public class PhrasesSection
{
    [JsonPropertyName("Block")] 
    public string Block { get; set; } = "This weapon is restricted: {weapon} (limit: {limit}).";

    [JsonPropertyName("BlockTeam")] 
    public string BlockTeam { get; set; } = "This weapon is restricted for your team: {weapon} (limit: {limit}).";

    [JsonPropertyName("SellRefund")]
    public string SellRefund { get; set; } = "Restricted {weapon} was auto-sold for {price}$.";

    [JsonPropertyName("SellRemoved")]
    public string SellRemoved { get; set; } = "Restricted {weapon} was removed from your inventory.";

    [JsonPropertyName("WeaponPretty")] 
    public Dictionary<string, string> WeaponPretty { get; set; } = WeaponDefaults.DefaultWeaponPretty();

    public string Pretty(string classname)
    {
        if (WeaponPretty.TryGetValue(classname, out var p) && !string.IsNullOrWhiteSpace(p)) 
            return p;
        var raw = classname.StartsWith("weapon_") ? classname.Substring("weapon_".Length) : classname;
        return raw.ToUpperInvariant();
    }
}

