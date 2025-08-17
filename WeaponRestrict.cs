using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Extensions;
using WeaponRestrict.Config;
using WeaponRestrict.Core;

namespace WeaponRestrict;

public class WeaponRestrict : BasePlugin, IPluginConfig<RestrictedWeaponsConfig>
{
    public override string ModuleName => "WeaponRestrict";
    public override string ModuleAuthor => "TICHOJEBEC";
    public override string ModuleVersion => "1.2";
    public override string ModuleDescription => "https://github.com/TICHOJEBEC-SK/cs2-WeaponRestrict";

    public RestrictedWeaponsConfig Config { get; set; } = new();

    private Restrictor? _restrictor;
    private readonly RoundSerial _round = new();

    public void OnConfigParsed(RestrictedWeaponsConfig config)
    {
        config.TypePlayers = (config.TypePlayers == 2) ? 2 : 1;
        config.TypeWeapons = (config.TypeWeapons == 2) ? 2 : 1;
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        _restrictor = new Restrictor(this, _round);

        RegisterEventHandler<EventItemEquip>(_restrictor.OnItemEquip);
        RegisterEventHandler<EventItemPurchase>(_restrictor.OnItemPurchase);
        RegisterEventHandler<EventRoundStart>(_round.OnRoundStart);
        RegisterEventHandler<EventRoundEnd>(_round.OnRoundEnd);
    }


    public override void Unload(bool hotReload)
    {
        _restrictor?.Dispose();
        _restrictor = null;
    }
    
    [ConsoleCommand("rw_reload_config", "Reload RestrictedWeapons config")]
    [RequiresPermissions("@css/root")]
    public void CmdReload(CCSPlayerController? player, CommandInfo info)
    {
        Config.Reload();
        info.ReplyToCommand($"{Config.ChatPrefix} Reloaded.");
    }
    
    [ConsoleCommand("rw_reset_config", "Reset RestrictedWeapons config to defaults and save")]
    [RequiresPermissions("@css/root")]
    public void CmdReset(CCSPlayerController? player, CommandInfo info)
    {
        Config = new RestrictedWeaponsConfig();
        Config.Update();
        info.ReplyToCommand($"{Config.ChatPrefix} Reset to defaults.");
    }
}
