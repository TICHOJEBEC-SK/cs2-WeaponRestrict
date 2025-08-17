namespace WeaponRestrict.Config;

internal static class WeaponDefaults
{
    public static Dictionary<int, string> DefaultDefIndexToClass() => new()
    {
        [1] = "weapon_deagle",
        [2] = "weapon_elite",
        [3] = "weapon_fiveseven",
        [4] = "weapon_glock",
        [7] = "weapon_ak47",
        [8] = "weapon_aug",
        [9] = "weapon_awp",
        [10] = "weapon_famas",
        [11] = "weapon_g3sg1",
        [13] = "weapon_galilar",
        [14] = "weapon_m249",
        [16] = "weapon_m4a1",
        [17] = "weapon_mac10",
        [19] = "weapon_p90",
        [23] = "weapon_mp5sd",
        [24] = "weapon_ump45",
        [25] = "weapon_xm1014",
        [26] = "weapon_bizon",
        [27] = "weapon_mag7",
        [28] = "weapon_negev",
        [29] = "weapon_sawedoff",
        [30] = "weapon_tec9",
        [31] = "weapon_taser",
        [32] = "weapon_hkp2000",
        [33] = "weapon_mp7",
        [34] = "weapon_mp9",
        [35] = "weapon_nova",
        [36] = "weapon_p250",
        [38] = "weapon_scar20",
        [39] = "weapon_sg556",
        [40] = "weapon_ssg08",
        [60] = "weapon_m4a1_silencer",
        [61] = "weapon_usp_silencer",
        [63] = "weapon_cz75a",
        [64] = "weapon_revolver",
    };

    public static Dictionary<string, Dictionary<int, Dictionary<string, int>>> DefaultRules()
        => new(StringComparer.OrdinalIgnoreCase)
        {
            ["all"] = new()
            {
                [5]  = new() { ["weapon_awp"] = 1 },
                [10] = new() { ["weapon_awp"] = 2 },
            }
        };

    public static Dictionary<string, string> DefaultWeaponPretty() => new(StringComparer.OrdinalIgnoreCase)
    {
        ["weapon_deagle"] = "Desert Eagle",
        ["weapon_elite"] = "Dual Berettas",
        ["weapon_fiveseven"] = "Five-SeveN",
        ["weapon_glock"] = "Glock-18",
        ["weapon_hkp2000"] = "P2000",
        ["weapon_usp_silencer"] = "USP-S",
        ["weapon_p250"] = "P250",
        ["weapon_cz75a"] = "CZ75-Auto",
        ["weapon_tec9"] = "Tec-9",
        ["weapon_revolver"] = "R8 Revolver",
        ["weapon_taser"] = "Zeus x27",

        ["weapon_mac10"] = "MAC-10",
        ["weapon_mp9"] = "MP9",
        ["weapon_mp7"] = "MP7",
        ["weapon_mp5sd"] = "MP5-SD",
        ["weapon_ump45"] = "UMP-45",
        ["weapon_bizon"] = "PP-Bizon",
        ["weapon_p90"] = "P90",

        ["weapon_ak47"] = "AK-47",
        ["weapon_galilar"] = "Galil AR",
        ["weapon_famas"] = "FAMAS",
        ["weapon_m4a1"] = "M4A4",
        ["weapon_m4a1_silencer"] = "M4A1-S",
        ["weapon_aug"] = "AUG",
        ["weapon_sg556"] = "SG 553",

        ["weapon_xm1014"] = "XM1014",
        ["weapon_nova"] = "Nova",
        ["weapon_sawedoff"] = "Sawed-Off",
        ["weapon_mag7"] = "MAG-7",
        ["weapon_m249"] = "M249",
        ["weapon_negev"] = "Negev",

        ["weapon_ssg08"] = "SSG 08",
        ["weapon_awp"] = "AWP",
        ["weapon_g3sg1"] = "G3SG1",
        ["weapon_scar20"] = "SCAR-20",

        ["weapon_hegrenade"] = "HE Grenade",
        ["weapon_flashbang"] = "Flashbang",
        ["weapon_smokegrenade"] = "Smoke Grenade",
        ["weapon_incgrenade"] = "Incendiary Grenade",
        ["weapon_molotov"] = "Molotov",
        ["weapon_decoy"] = "Decoy Grenade",
    };
    
    public static Dictionary<string, int> DefaultWeaponPrices() => new(StringComparer.OrdinalIgnoreCase)
    {
        ["weapon_glock"] = 200,
        ["weapon_hkp2000"] = 200,
        ["weapon_usp_silencer"] = 200,
        ["weapon_p250"] = 300,
        ["weapon_cz75a"] = 500,
        ["weapon_tec9"] = 500,
        ["weapon_fiveseven"] = 500,
        ["weapon_elite"] = 300,
        ["weapon_deagle"] = 700,
        ["weapon_revolver"] = 600,
        
        ["weapon_mac10"] = 1050,
        ["weapon_mp9"] = 1250,
        ["weapon_mp7"] = 1500,
        ["weapon_mp5sd"] = 1500,
        ["weapon_ump45"] = 1200,
        ["weapon_bizon"] = 1400,
        ["weapon_p90"] = 2350,
        
        ["weapon_galilar"] = 1800,
        ["weapon_famas"]  = 2050,
        ["weapon_ak47"]   = 2700,
        ["weapon_m4a1"]   = 2900,
        ["weapon_m4a1_silencer"] = 2900,
        ["weapon_aug"]    = 3300,
        ["weapon_sg556"]  = 3000,
        
        ["weapon_xm1014"] = 2000,
        ["weapon_nova"]   = 1050,
        ["weapon_sawedoff"]=1100,
        ["weapon_mag7"]   = 1300,
        ["weapon_m249"]   = 4750,
        ["weapon_negev"]  = 1700,
        
        ["weapon_ssg08"] = 1700,
        ["weapon_awp"]   = 4750,
        ["weapon_g3sg1"] = 5000,
        ["weapon_scar20"]= 5000,
        
        ["weapon_taser"] = 200,
        ["weapon_hegrenade"] = 300,
        ["weapon_flashbang"] = 200,
        ["weapon_smokegrenade"] = 300,
        ["weapon_incgrenade"] = 500,
        ["weapon_molotov"] = 500,
        ["weapon_decoy"] = 50
    };
}
