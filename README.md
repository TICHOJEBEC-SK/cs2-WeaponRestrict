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
If a player picks up or buys a restricted weapon, it will be blocked at the engine level:
- **No pickup** (weapon stays on the ground)
- **No purchase** (money is not spent, only chat message is shown)

Supports:
- **Configurable restrictions** (per map, per team, per player count)
- **Dynamic limits** (e.g. *max 1 AWP until 10 players, max 2 AWP after 10 players*)
- **Bypass permissions** (e.g. VIP can ignore restrictions — optional, configurable)
- **Hard-ban system** (limits with `0` or `NoBypassWeapons` cannot be bypassed by anyone)
- **Automatic classnames** from weapon DefIndex
- **Custom chat messages with colors** (with placeholders and pretty names)
- **BlockSound support** – play a sound when a weapon is restricted

---

## 🔹 Features

- Restrict **any weapon** via config
- Works directly through **CanAcquire hook** (no hacks, no forced drops)
- Lightweight and crash-safe
- Multi-language friendly (phrases in config)
- **Per-map overrides** for rules and hard-bans
- **Configurable chat colors** for prefix and messages
- **Configurable BlockSound** on restriction

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
  "ChatPrefix": "{green}[{default}RW{green}]",
  "TypePlayers": 1,
  "TypeWeapons": 1,
  "CountSpectators": false,
  "BypassPermissions": [
    "@vip/restrict",
    "@css/root"
  ],
  "BypassAllowedWhenLimitIsZero": false,
  "NoBypassWeapons": [],
  "BlockSound": "sounds/example.vsnd",
  "Phrases": {
    "Block": "{default}This weapon is restricted: {lightred}{weapon} {default}(limit: {limit}).",
    "BlockTeam": "{default}This weapon is restricted for your team: {lightred}{weapon} {default}(limit: {limit}).",
    "WeaponPretty": { ... }
  },
  "DefIndexToClass": { ... },
  "Rules": { ... },
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
- **BlockSound** – path to a sound that plays when restriction triggers

---

## 🔢 How Rules Work

The plugin picks the **highest rule key ≤ current player count**.  
This means:
- Keys represent **minimum player thresholds**.  
- If no exact key exists, the nearest lower one is used.  

Example config:
```json
"Rules": {
  "all": {
    "1": { "weapon_deagle": 0 },
    "3": { "weapon_deagle": 2 },
    "4": { "weapon_deagle": 3 }
  }
}
```

### Explanation
- 1 player → uses key `1` → `deagle = 0` (hard-ban)  
- 2 players → still uses key `1` (because `3` > 2) → `deagle = 0`  
- 3 players → key `3` → max 2 deagles  
- 4 players → key `4` → max 3 deagles  
- 5 players → key `4` (nearest ≤ 5) → max 3 deagles  

👉 If you want a weapon to be **unrestricted** for certain player counts, you must **explicitly set `-1`**:  
```json
"Rules": {
  "all": {
    "1": { "weapon_awp": 0 },
    "2": { "weapon_awp": -1 }, // unrestricted for 2 players
    "5": { "weapon_awp": 1 }   // max 1 after 5 players
  }
}
```

- `0` = completely banned  
- `-1` = unlimited  
- `N` = maximum allowed  

---

## 🎨 Chat Colors

You can use the following color tags inside messages and prefixes:

- `{default}`, `{white}`, `{darkred}`, `{green}`, `{lightyellow}`, `{lightblue}`
- `{olive}`, `{lime}`, `{red}`, `{lightpurple}`, `{purple}`, `{grey}`, `{gray}`
- `{yellow}`, `{gold}`, `{silver}`, `{blue}`, `{darkblue}`, `{bluegrey}`
- `{magenta}`, `{lightred}`, `{orange}`

**Example:**
```json
"ChatPrefix": "{blue}A{green}H{red}O{yellow}J"
```

---

## 📩 Contact
- **Discord:** `tichotm`
