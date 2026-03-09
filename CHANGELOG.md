# Changelog

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
