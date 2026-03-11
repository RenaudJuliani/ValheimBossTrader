using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ValheimBossTrader
{
    /// <summary>
    /// Patch principal : injecte les items dans Trader.m_items au démarrage du marchand.
    /// </summary>
    [HarmonyPatch(typeof(Trader), nameof(Trader.Start))]
    public static class TraderStartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Trader __instance)
        {
            TraderPatch.InjectItems(__instance);
        }
    }

    /// <summary>
    /// Patch de détection de boss tué : quand ZoneSystem enregistre une global key,
    /// on vérifie si c'est une clé de boss et on rafraîchit tous les traders actifs.
    /// </summary>
    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.SetGlobalKey), typeof(string))]
    public static class BossKillPatch
    {
        private static readonly HashSet<string> BossKeys = new()
        {
            BossKey.Eikthyr,
            BossKey.Elder,
            BossKey.Bonemass,
            BossKey.Moder,
            BossKey.Yagluth,
            BossKey.Queen,
            BossKey.Fader,
        };

        [HarmonyPostfix]
        public static void Postfix(string name)
        {
            if (!BossKeys.Contains(name))
                return;

            Plugin.Log.LogInfo($"[BossTrader] Boss détecté : {name} — rafraîchissement du marchand.");

            // Trouver tous les Traders dans la scène et ré-injecter
            var traders = Object.FindObjectsOfType<Trader>();
            foreach (var trader in traders)
                TraderPatch.InjectItems(trader);
        }
    }

    /// <summary>
    /// Logique d'injection commune aux deux patches.
    /// </summary>
    public static class TraderPatch
    {
        public static void InjectItems(Trader trader)
        {
            if (ZoneSystem.instance == null || ObjectDB.instance == null)
                return;

            if (!ModConfig.IsTraderEnabled(trader))
            {
                Plugin.Log.LogInfo($"[BossTrader] {trader.m_name} ignoré (désactivé dans la config).");
                return;
            }

            // Ensemble des prefab déjà présents pour éviter les doublons
            var existing = new HashSet<string>();
            foreach (var t in trader.m_items)
            {
                if (t.m_prefab != null)
                    existing.Add(t.m_prefab.name);
            }

            int added = 0;
            foreach (var def in TraderItems.GetEnabled())
            {
                // Condition boss
                if (def.RequiredKey != null && !ZoneSystem.instance.GetGlobalKey(def.RequiredKey))
                    continue;

                // Pas de doublon
                if (existing.Contains(def.PrefabName))
                    continue;

                var prefabGo = ObjectDB.instance.GetItemPrefab(def.PrefabName);
                if (prefabGo == null)
                {
                    Plugin.Log.LogWarning($"[BossTrader] Prefab introuvable : {def.PrefabName}");
                    continue;
                }

                var itemDrop = prefabGo.GetComponent<ItemDrop>();
                if (itemDrop == null)
                    continue;

                // Compatibilité EpicLoot : ignorer les items sans type valide
                // (ItemType.None provoque une KeyNotFoundException dans GatedItemTypeHelper)
                if (itemDrop.m_itemData?.m_shared?.m_itemType == ItemDrop.ItemData.ItemType.None)
                {
                    Plugin.Log.LogWarning($"[BossTrader] Ignoré (ItemType.None, incompatible EpicLoot) : {def.PrefabName}");
                    continue;
                }

                trader.m_items.Add(new Trader.TradeItem
                {
                    m_prefab = itemDrop,
                    m_stack  = ModConfig.ApplyStack(def.Stack),
                    m_price  = ModConfig.ApplyPrice(def.Price),
                });

                CategoryFilter.Register(def.PrefabName, def.Cat);
                existing.Add(def.PrefabName);
                added++;
            }

            if (added > 0)
                Plugin.Log.LogInfo($"[BossTrader] {added} items ajoutés à {trader.name}.");
        }
    }
}
