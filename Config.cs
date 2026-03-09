using BepInEx.Configuration;

namespace ValheimBossTrader
{
    /// <summary>
    /// Configuration du mod, générée dans BepInEx/config/juliani.mods.valheim.bosstrader.cfg
    /// Modifiable via r2modman (Thunderstore), le file manager DatHost, ou tout éditeur texte.
    /// Les changements sont pris en compte au prochain démarrage du jeu.
    /// </summary>
    public static class ModConfig
    {
        // ── Prix ────────────────────────────────────────────────────────────
        public static ConfigEntry<float> PriceMultiplier = null!;

        // ── Stacks ──────────────────────────────────────────────────────────
        public static ConfigEntry<float> StackMultiplier = null!;

        // ── Marchands ────────────────────────────────────────────────────────
        public static ConfigEntry<bool> EnableHaldor   = null!;
        public static ConfigEntry<bool> EnableHildir   = null!;
        public static ConfigEntry<bool> EnableBogWitch = null!;

        // ── Catégories activées ──────────────────────────────────────────────
        public static ConfigEntry<bool> EnableMaterials   = null!;
        public static ConfigEntry<bool> EnableFood        = null!;
        public static ConfigEntry<bool> EnableWeapons     = null!;
        public static ConfigEntry<bool> EnableArmor       = null!;
        public static ConfigEntry<bool> EnableAmmo        = null!;
        public static ConfigEntry<bool> EnableConsumables = null!;
        public static ConfigEntry<bool> EnableMisc        = null!;

        public static void Init(ConfigFile config)
        {
            const string sectionPrix      = "1 - Prix";
            const string sectionStacks    = "2 - Stacks";
            const string sectionMarchands = "3 - Marchands";
            const string sectionCats      = "4 - Catégories";

            PriceMultiplier = config.Bind(
                sectionPrix, "PriceMultiplier", 1.0f,
                new ConfigDescription(
                    "Multiplicateur appliqué à tous les prix.\n" +
                    "Ex: 0.5 = moitié prix, 2.0 = double prix.",
                    new AcceptableValueRange<float>(0.1f, 10.0f)));

            StackMultiplier = config.Bind(
                sectionStacks, "StackMultiplier", 1.0f,
                new ConfigDescription(
                    "Multiplicateur appliqué à toutes les quantités par achat.\n" +
                    "Ex: 2.0 = double la quantité achetée à chaque fois.",
                    new AcceptableValueRange<float>(0.1f, 10.0f)));

            EnableHaldor = config.Bind(
                sectionMarchands, "EnableHaldor", true,
                "Injecter les items chez Haldor.");

            EnableHildir = config.Bind(
                sectionMarchands, "EnableHildir", false,
                "Injecter les items chez Hildir.");

            EnableBogWitch = config.Bind(
                sectionMarchands, "EnableBogWitch", false,
                "Injecter les items chez la Bog Witch.");

            EnableMaterials = config.Bind(
                sectionCats, "EnableMaterials", true,
                "Activer la vente de matériaux / minerais / ressources.");

            EnableFood = config.Bind(
                sectionCats, "EnableFood", true,
                "Activer la vente de nourriture (crue et cuite).");

            EnableWeapons = config.Bind(
                sectionCats, "EnableWeapons", true,
                "Activer la vente d'armes.");

            EnableArmor = config.Bind(
                sectionCats, "EnableArmor", true,
                "Activer la vente d'armures et de capes.");

            EnableAmmo = config.Bind(
                sectionCats, "EnableAmmo", true,
                "Activer la vente de munitions (flèches, boulons, bombes).");

            EnableConsumables = config.Bind(
                sectionCats, "EnableConsumables", true,
                "Activer la vente de consommables (potions, hydromel, appâts).");

            EnableMisc = config.Bind(
                sectionCats, "EnableMisc", true,
                "Activer la vente des autres items (graines, trophées, outils, etc.).");
        }

        /// <summary>
        /// Retourne true si ce marchand est activé dans la config.
        /// On compare sur m_name (nom interne Valheim) et en fallback sur le nom du GameObject.
        /// </summary>
        public static bool IsTraderEnabled(Trader trader)
        {
            var name = trader.m_name ?? trader.gameObject.name;
            if (name.Contains("Haldor"))   return EnableHaldor.Value;
            if (name.Contains("Hildir"))   return EnableHildir.Value;
            if (name.Contains("BogWitch") || name.Contains("Bog_Witch") || name.Contains("Bog Witch"))
                return EnableBogWitch.Value;

            // Marchand inconnu (mod tiers) → injecter par défaut
            return true;
        }

        /// <summary>
        /// Applique les multiplicateurs de prix et de stack à une valeur brute.
        /// </summary>
        public static int ApplyPrice(int basePrice)
            => System.Math.Max(1, (int)(basePrice * PriceMultiplier.Value));

        public static int ApplyStack(int baseStack)
            => System.Math.Max(1, (int)(baseStack * StackMultiplier.Value));
    }
}
