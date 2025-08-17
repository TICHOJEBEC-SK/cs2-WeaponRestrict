<h1 align="center">
  CS2 WeaponRestrict
</h1>

<p align="center">
<i>Loved the tool? Please consider <a href="https://paypal.com/paypalme/playpointsk">donating</a> 💸 to help it improve!</i>
</p>

<p align="center">
<a href="https://www.paypal.com/paypalme/playpointsk"><img src="https://img.shields.io/badge/support-PayPal-blue?logo=PayPal&style=flat-square&label=Donate"/>
</a>
</p>

---

## 📜 About the Plugin

A **Counter-Strike 2 plugin** for **CounterStrikeSharp** that lets you **restrict weapons** by rules.  
If a player picks up or buys a restricted weapon, it will be:
- **Automatically dropped and swapped to knife** (equip restriction)
- **Automatically sold with refund** (purchase restriction)

Supports:
- **Configurable restrictions** (per map, per team, per player count)
- **Dynamic limits** (e.g. *max 1 AWP until 10 players, max 2 AWP after 10 players*)
- **Bypass permissions** (e.g. VIP can ignore restrictions — optional, configurable)
- **Hard-ban system** (limits with `0` or `NoBypassWeapons` cannot be bypassed by anyone)
- **Automatic classnames** from weapon DefIndex
- **Custom chat messages** (with weapon pretty names and placeholders)
- **Weapon price map** for refunds

---

## 🔹 Features

- Restrict **any weapon** via config
- Drop system with **anti-spam protection**
- **Safe ActiveLock** prevents exploits (players can’t force-equip restricted guns)
- Lightweight and crash-safe
- Multi-language friendly (phrases in config)
- **Auto-sell restricted weapons** with money refund
- **Per-map overrides** for rules and hard-bans

---

## 🛠 Installation

**Requirements**
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

**Steps**
1. Build the plugin (`dotnet build -c Release`) or download prebuilt.
2. Copy the DLL and config file to:
   ```
   /game/csgo/addons/counterstrikesharp/plugins/WeaponRestrict/
   ```
3. Start or restart the server.

---

## ⚙️ Configuration

Config is generated on first run:
```json
{
  "ChatPrefix": "[RW]",
  "TypePlayers": 1,
  "TypeWeapons": 1,
  "CountSpectators": false,
  "BypassPermissions": [
    "@vip/restrict",
    "@css/root"
  ],
  "BypassAllowedWhenLimitIsZero": false,
  "NoBypassWeapons": [],
  "Phrases": {
    "Block": "Táto zbraň je obmedzená: {weapon} (limit: {limit}).",
    "BlockTeam": "Táto zbraň je obmedzená: {weapon} (limit: {limit}).",
    "SellRefund": "Obmedzená zbraň {weapon} bola automaticky predaná za {price}$.",
    "SellRemoved": "Obmedzená zbraň {weapon} bola odstránená z tvojho inventára.",
    "WeaponPretty": {...}
  },
  "DefIndexToClass": {...},
  "Rules": {
    "all": {
      "0": {
        "weapon_g3sg1": 0,
        "weapon_scar20": 0
      },
      "4": {
        "weapon_awp": 1
      },
      "5": {
        "weapon_awp": 2
      },
      "7": {
        "weapon_awp": 3
      }
    },
    "de_dust2": {
      "0": {
        "weapon_deagle": 0
      }
    }
  },
  "ConfigVersion": 1
}
```

### 🔧 Options
- **TypePlayers**
    - `1` = count all players
    - `2` = count only teammates
- **TypeWeapons**
    - `1` = restrict globally
    - `2` = restrict per team
- **BypassPermissions** – list of permissions that can ignore restrictions (VIP/root)
- **BypassAllowedWhenLimitIsZero**
    - `false` = VIP/admin **cannot bypass** weapons with limit `0` (hard-ban)
    - `true` = VIP/admin **can still bypass** limit `0`
- **NoBypassWeapons** – explicit list of classnames that **nobody can bypass**, regardless of limit  
  Example: `[ "weapon_awp", "weapon_g3sg1" ]`
- **Rules** – weapon limits per map and player count (`all` applies everywhere)

---

## 📩 Contact
- **Discord:** `tichotm`
