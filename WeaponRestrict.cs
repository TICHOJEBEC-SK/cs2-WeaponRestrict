using CounterStrikeSharp.API.Core;
using WeaponRestrict.Config;
using WeaponRestrict.Core;

namespace WeaponRestrict;

public class WeaponRestrict : BasePlugin, IPluginConfig<RestrictedWeaponsConfig>
{
    public override string ModuleName => "WeaponRestrict";
    public override string ModuleAuthor => "TICHOJEBEC";
    public override string ModuleVersion => "1.5";
    public override string ModuleDescription => "https://github.com/TICHOJEBEC-SK/cs2-WeaponRestrict";

    public RestrictedWeaponsConfig Config { get; set; } = new();
    private Restrictor? _restrictor;

    public override void Load(bool hotReload)
    {
        _restrictor = new Restrictor(this);
        base.Load(hotReload);
    }

    public override void Unload(bool hotReload)
    {
        _restrictor?.Dispose();
        _restrictor = null;
        base.Unload(hotReload);
    }

    public void OnConfigParsed(RestrictedWeaponsConfig config)
    {
        config.TypePlayers = (config.TypePlayers == 2) ? 2 : 1;
        config.TypeWeapons = (config.TypeWeapons == 2) ? 2 : 1;
        Config = config;
    }
}