# ValheimBossTrader

**Unlock 964 items at Haldor based on your boss progression.**

Every item sold by the trader is gated behind the boss of its corresponding biome. Kill a boss, unlock the next biome's resources — materials, food, weapons, armor, ammo, consumables, and more. Prices and quantities are sourced directly from [TraderOverhaul](https://github.com/JoeCorrell/TraderOverhaul) and fully configurable.

---

## Features

- **Boss-gated progression** — Items unlock as you defeat bosses, following Valheim's natural biome progression
- **964 items** across all biomes, organized into 7 categories
- **Real-time unlock** — Kill a boss mid-session and items appear immediately, no reload needed
- **Choose your traders** — Configure which merchants sell items (Haldor, Hildir, Bog Witch)
- **Fully configurable** — Adjust prices, stack sizes, toggle categories and merchants via the `.cfg` file
- **Compatible with r2modman and DatHost** — Config is a standard BepInEx `.cfg` file
- **No hard dependencies** — Works standalone, no other mods required

---

## Progression Table

| Boss Defeated | Biome Unlocked | What you can buy |
|---|---|---|
| *(none)* | Meadows | Basic tools, food, seeds, leather gear |
| **Eikthyr** | Black Forest | Copper, Tin, Bronze + Black Forest food |
| **The Elder** | Swamp | Iron, Chitin + fish, soups, potions |
| **Bonemass** | Mountain | Silver, Obsidian + wolf meat, onion dishes |
| **Moder** | Plains | Black Metal, Tar + barley, lox, bread |
| **Yagluth** | Mistlands | Eitr, Carapace + mushrooms, chicken, seeker |
| **The Queen** | Ashlands | Flametal, Black Marble + Ashlands foods |
| **Fader** | Deep North | Deep North fishing bait |

---

## Item Categories

| Category | Count | Toggle Config Key |
|---|---|---|
| Materials (ores, bars, resources) | 61 | `EnableMaterials` |
| Food (raw & cooked) | 139 | `EnableFood` |
| Weapons | 293 | `EnableWeapons` |
| Armor & Shields | 226 | `EnableArmor` |
| Ammo (arrows, bolts, bombs) | 27 | `EnableAmmo` |
| Consumables (meads, potions, bait) | 38 | `EnableConsumables` |
| Misc (seeds, trophies, tools) | 180 | `EnableMisc` |

---

## Installation

### Via r2modman (recommended)
1. Open **r2modman** and select Valheim
2. Search for **ValheimBossTrader** in the online tab
3. Click Install

### Manual
1. Make sure **BepInExPack_Valheim** is installed
2. Copy `ValheimBossTrader.dll` into `Valheim/BepInEx/plugins/`
3. Launch the game

---

## Configuration

The config file is generated on first launch at:

```
BepInEx/config/juliani.mods.valheim.bosstrader.cfg
```

Editable via **r2modman config editor**, **DatHost file manager**, or any text editor.

### Options

**[1 - Prix]**

| Key | Default | Range | Description |
|---|---|---|---|
| `PriceMultiplier` | `1.0` | `0.1 – 10.0` | Multiply all prices. `0.5` = half price, `2.0` = double price |

**[2 - Stacks]**

| Key | Default | Range | Description |
|---|---|---|---|
| `StackMultiplier` | `1.0` | `0.1 – 10.0` | Multiply all stack sizes per purchase |

**[3 - Marchands]**

| Key | Default | Description |
|---|---|---|
| `EnableHaldor` | `true` | Inject items into Haldor's shop |
| `EnableHildir` | `false` | Inject items into Hildir's shop |
| `EnableBogWitch` | `false` | Inject items into the Bog Witch's shop |

**[4 - Catégories]**

| Key | Default | Description |
|---|---|---|
| `EnableMaterials` | `true` | Enable ores, bars, and raw resources |
| `EnableFood` | `true` | Enable raw and cooked food |
| `EnableWeapons` | `true` | Enable weapons |
| `EnableArmor` | `true` | Enable armor, capes, and shields |
| `EnableAmmo` | `true` | Enable arrows, bolts, and bombs |
| `EnableConsumables` | `true` | Enable meads, potions, and fishing bait |
| `EnableMisc` | `true` | Enable seeds, trophies, tools, and other items |

### Example: Resources-only server, Haldor only

```ini
[3 - Marchands]
EnableHaldor   = true
EnableHildir   = false
EnableBogWitch = false

[4 - Catégories]
EnableMaterials   = true
EnableFood        = true
EnableWeapons     = false
EnableArmor       = false
EnableAmmo        = false
EnableConsumables = false
EnableMisc        = false
```

---

## Server-side Usage

This mod **must be installed on the server** for boss-gating to work correctly in multiplayer, since global keys are stored server-side. Clients do not need to install it (the items will appear for all players when they open the shop).

---

## Compatibility

- **BepInExPack_Valheim** 5.4.2200+ required
- Compatible with **Epic Loot** — base item variants only, no rarity conflicts
- Compatible with **Plant Everything**, **Valheim Plus**, and most utility mods
- Not compatible with mods that fully replace the Trader UI (such as TraderOverhaul itself)

---

## Item Data Source

All item definitions, prices, and stack sizes are sourced from [TraderOverhaul by JoeCorrell](https://github.com/JoeCorrell/TraderOverhaul). Only the base (non-rarity) version of each item is used for vanilla compatibility.

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md)

---

## Source Code

[github.com/RenaudJuliani/ValheimBossTrader](https://github.com/RenaudJuliani/ValheimBossTrader)
