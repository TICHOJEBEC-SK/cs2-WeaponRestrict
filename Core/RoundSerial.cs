using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace WeaponRestrict.Core;

internal sealed class RoundSerial
{
    private int _serial = 1;
    public int Current => _serial;

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) { _serial++; return HookResult.Continue; }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd ev, GameEventInfo info) { _serial++; return HookResult.Continue; }
}