using HarmonyLib;
using UnityEngine;

namespace ValheimBossTrader
{
    /// <summary>
    /// Affiche / cache la fenêtre bancaire en même temps que le StoreGui.
    /// </summary>
    [HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Show))]
    public static class StoreGui_Show_BankPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (BankUI.Instance == null)
            {
                var go = new GameObject("BossTrader_BankUI");
                go.AddComponent<BankUI>();
            }
            BankUI.Instance.Show();

            if (CategoryFilterUI.Instance == null)
            {
                var go = new GameObject("BossTrader_CategoryFilterUI");
                go.AddComponent<CategoryFilterUI>();
            }
            CategoryFilterUI.Instance.Show();
        }
    }

    [HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Hide))]
    public static class StoreGui_Hide_BankPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            BankUI.Instance?.Hide();
            CategoryFilterUI.Instance?.Hide();
        }
    }

    /// <summary>
    /// Remplace le comptage de pièces du StoreGui par inventaire + solde bancaire.
    /// Corrige à la fois le grisage des items ET le check CanAfford (qui appelle GetPlayerCoins).
    /// </summary>
    [HarmonyPatch(typeof(StoreGui), "GetPlayerCoins")]
    public static class StoreGui_GetPlayerCoins_BankPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            long bank = BankManager.GetBalance();
            if (bank > 0)
                __result = (int)System.Math.Min((long)__result + bank, int.MaxValue);
        }
    }

    /// <summary>
    /// Avant l'achat, complète automatiquement l'inventaire depuis la banque
    /// si le joueur n'a pas assez de pièces sur lui.
    /// </summary>
    [HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
    public static class StoreGui_BuySelectedItem_BankPatch
    {
        [HarmonyPrefix]
        public static void Prefix(StoreGui __instance)
        {
            var item = __instance.m_selectedItem;
            if (item == null) return;

            int shortfall = item.m_price - BankManager.GetPlayerCoins();
            if (shortfall <= 0) return;

            if (BankManager.GetBalance() >= shortfall)
                BankManager.Withdraw(shortfall, out _);
        }
    }

    /// <summary>
    /// Commande console de debug : bankset [montant]
    /// Nécessite devcommands activé en jeu.
    /// Usage : bankset 9999
    /// Patch sur Awake (pas InitTerminal) pour éviter l'ouverture automatique de la console au démarrage.
    /// </summary>
    [HarmonyPatch(typeof(Terminal), "Awake")]
    public static class Terminal_Awake_BankPatch
    {
        private static bool _registered = false;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (_registered) return;
            _registered = true;

            new Terminal.ConsoleCommand(
                "bankset",
                "[montant] — Définit le solde de la banque (debug BossTrader)",
                args =>
                {
                    if (args.Args.Length < 2)
                    {
                        args.Context.AddString("Usage : bankset <montant>");
                        return;
                    }
                    if (!long.TryParse(args.Args[1], out long amount) || amount < 0)
                    {
                        args.Context.AddString("Montant invalide. Exemple : bankset 9999");
                        return;
                    }
                    BankManager.SetBalanceDebug(amount);
                    args.Context.AddString($"[BossTrader] Solde bancaire → {amount} pièces.");
                },
                isCheat: true);
        }
    }
}
