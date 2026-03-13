# Changelog

## 1.0.4
- Fix: Bank and category filter UI backgrounds disappearing after a few seconds — textures were being garbage collected by Unity. All textures are now stored as MonoBehaviour fields to prevent collection.

## 1.0.3
- New: **Bank system** — deposit and withdraw coins at Haldor. Balance saved per character in `BepInEx/config/BossTrader_Bank/`. Bank funds are automatically used when buying items (inventory is topped up transparently before each purchase).
- New: **Category filter panel** — IMGUI panel displayed next to the merchant, with buttons for each category (Matériaux, Nourriture, Armes, Armures, Munitions, Consommables, Divers). Clicking a category filters the item list instantly; clicking again returns to "Tout".
- New: **Haldor dark-wood UI theme** — Bank and category filter panels use a custom dark wood / amber palette matching Haldor's Black Forest aesthetic.
- Fix: Capes (`CapeDeerHide`, `CapeTrollHide`, `CapeWolf`, `CapeFeather`, `CapeLinen`, `CapeLox`, `CapeAsh`, `CapeAsksvin`) were incorrectly categorized as Food; corrected to Armor.

## 1.0.2
- Fix: load after ValheimLegends to prevent Warp spell conflict (SoftDependency)

## 1.0.1
- Fix: the mod was silently failing to load on BepInEx 5.4.23.x due to a transitive dependency on `MonoMod.Backports` introduced by `HarmonyX 2.16.0`. Pinned `HarmonyX` to `2.10.1` to resolve this.
- Fix: replaced `record` type with a plain `class` to avoid potential runtime issues on .NET Framework 4.8.1.
- Fix: added a guard to skip items with `ItemType.None` for compatibility with Epic Loot's gamble system.

## 1.0.0
- Initial release
- 964 items from all biomes, gated by boss progression
- Real-time unlock on boss kill (no reload needed)
- Configurable price multiplier, stack multiplier, and category toggles
- Prices and quantities sourced from TraderOverhaul
