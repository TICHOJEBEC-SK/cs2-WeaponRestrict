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
If a player picks up or buys a restricted weapon, it will be **automatically dropped and swapped to knife** — without server crashes and without money refunds.  

Supports:
- **Configurable restrictions** (per map, per team, per player count)
- **Dynamic limits** (e.g. *max 1 AWP until 10 players, max 2 AWP after 10 players*)
- **Bypass permissions** (e.g. VIP can ignore restrictions)
- **Automatic classnames** from weapon DefIndex
- **Custom chat messages** (with weapon pretty names and colors)

---

## 🔹 Features

- Restrict **any weapon** via config  
- Drop system with **anti-spam protection**  
- **Safe ActiveLock** prevents exploits (players can’t force-equip restricted guns)  
- Lightweight and crash-safe  
- Multi-language friendly (phrases in config)  

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
  "BypassPermissions": [ "@vip/restrict", "@css/root" ],
  "Phrases": {
    "Block": "This weapon is restricted: {weapon} (limit: {limit}).",
    "BlockTeam": "This weapon is restricted for your team: {weapon} (limit: {limit})."
  },
  "DefIndexToClass": { ... },
  "Rules": {
    "all": {
      "5": { "weapon_awp": 1 },
      "10": { "weapon_awp": 2 }
    }
  }
}
```

- **TypePlayers**
  - `1` = count all players  
  - `2` = count only teammates
- **TypeWeapons**
  - `1` = restrict globally  
  - `2` = restrict per team
- **BypassPermissions** – list of permissions that can ignore restrictions (VIP/root)
- **Rules** – weapon limits per map and player count

---

## 📩 Contact
- **Discord:** `tichotm`
