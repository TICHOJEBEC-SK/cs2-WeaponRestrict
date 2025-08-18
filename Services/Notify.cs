using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace WeaponRestrict.Services;

internal static class Notify
{
    private static readonly Dictionary<string, char> ColorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["default"] = ChatColors.Default,
        ["white"] = ChatColors.White,
        ["darkred"] = ChatColors.DarkRed,
        ["green"] = ChatColors.Green,
        ["lightyellow"] = ChatColors.LightYellow,
        ["lightblue"] = ChatColors.LightBlue,
        ["olive"] = ChatColors.Olive,
        ["lime"] = ChatColors.Lime,
        ["red"] = ChatColors.Red,
        ["lightpurple"] = ChatColors.LightPurple,
        ["purple"] = ChatColors.Purple,
        ["grey"] = ChatColors.Grey,
        ["gray"] = ChatColors.Grey, // alias
        ["yellow"] = ChatColors.Yellow,
        ["gold"] = ChatColors.Gold,
        ["silver"] = ChatColors.Silver,
        ["blue"] = ChatColors.Blue,
        ["darkblue"] = ChatColors.DarkBlue,
        ["bluegrey"] = ChatColors.BlueGrey,
        ["magenta"] = ChatColors.Magenta,
        ["lightred"] = ChatColors.LightRed,
        ["orange"] = ChatColors.Orange
    };

    private static string ApplyColors(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var t = text;
        
        foreach (var kv in ColorMap)
            t = t.Replace("{" + kv.Key + "}", kv.Value.ToString());
        
        return t;
    }

    public static void Info(CCSPlayerController p, string prefix, string msg)
    {
        var composed = $" {ApplyColors(prefix)} {ApplyColors(msg)}{ChatColors.Default}";
        p.PrintToChat(composed);
    }
}