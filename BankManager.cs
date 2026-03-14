using System.IO;
using BepInEx;

namespace ValheimBossTrader
{
    /// <summary>
    /// Gère le solde bancaire du joueur.
    /// Persistance : BepInEx/config/BossTrader_Bank_{NomPersonnage}.dat
    /// (un fichier par personnage).
    /// </summary>
    public static class BankManager
    {
        private static string SaveDir =>
            Path.Combine(Paths.ConfigPath, "BossTrader_Bank");

        private static string FilePath(string playerName) =>
            Path.Combine(SaveDir, $"{playerName}.dat");

        private static string GetPlayerName()
        {
            var player = Player.m_localPlayer;
            return player == null ? "unknown" : player.GetPlayerName();
        }

        public static long GetBalance()
        {
            var path = FilePath(GetPlayerName());
            if (!File.Exists(path)) return 0;
            if (long.TryParse(File.ReadAllText(path).Trim(), out long balance))
                return balance;
            return 0;
        }

        private static void SetBalance(long amount)
        {
            Directory.CreateDirectory(SaveDir);
            File.WriteAllText(FilePath(GetPlayerName()), amount.ToString());
        }

        /// <summary>Utilisé par la commande console de debug.</summary>
        public static void SetBalanceDebug(long amount)
        {
            SetBalance(amount);
            Plugin.Log.LogInfo($"[Bank] Solde forcé à {amount} (debug).");
        }

        public static string GetCoinSharedName()
        {
            if (StoreGui.instance?.m_coinPrefab != null)
                return StoreGui.instance.m_coinPrefab.m_itemData.m_shared.m_name;
            var prefab = ObjectDB.instance?.GetItemPrefab("Coins");
            return prefab?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_name ?? "$item_coins";
        }

        public static int GetPlayerCoins()
        {
            var player = Player.m_localPlayer;
            if (player == null) return 0;
            return CountCoinsInInventory(player.GetInventory());
        }

        private static int CountCoinsInInventory(Inventory inv)
        {
            var coinName = GetCoinSharedName();
            int total = 0;
            foreach (var item in inv.GetAllItems())
            {
                if (item.m_shared?.m_name == coinName)
                    total += item.m_stack;
            }
            return total;
        }

        public static bool Deposit(int amount, out string message)
        {
            if (amount <= 0) { message = "Montant invalide."; return false; }

            var player = Player.m_localPlayer;
            if (player == null) { message = "Joueur introuvable."; return false; }

            int coins = GetPlayerCoins();
            if (coins < amount)
            {
                message = $"Pas assez de pièces. Vous en avez {coins}.";
                return false;
            }

            RemoveCoinsFromInventory(player, amount);
            SetBalance(GetBalance() + amount);
            message = $"Déposé {amount} pièces. Solde : {GetBalance()}";
            Plugin.Log.LogInfo($"[Bank] Dépôt {amount}. Solde : {GetBalance()}");
            return true;
        }

        /// <summary>
        /// Retire des pièces de l'inventaire en manipulant directement les stacks.
        /// Compatible avec toutes les versions de Valheim.
        /// </summary>
        private static void RemoveCoinsFromInventory(Player player, int amount)
        {
            var inv      = player.GetInventory();
            var coinName = GetCoinSharedName();
            int remaining = amount;

            var allItems = new System.Collections.Generic.List<ItemDrop.ItemData>(inv.GetAllItems());
            foreach (var item in allItems)
            {
                if (remaining <= 0) break;
                if (item.m_shared?.m_name != coinName) continue;

                int toRemove = System.Math.Min(remaining, item.m_stack);
                item.m_stack -= toRemove;
                remaining    -= toRemove;

                if (item.m_stack <= 0)
                    inv.RemoveItem(item);
            }
            inv.Changed();
        }

        /// <summary>
        /// Ajoute des pièces à l'inventaire en remplissant d'abord les stacks existants,
        /// puis en créant de nouveaux stacks via le prefab ObjectDB.
        /// Compatible avec toutes les versions de Valheim.
        /// </summary>
        private static void AddCoinsToInventory(Inventory inv, int amount)
        {
            var coinName  = GetCoinSharedName();
            int remaining = amount;

            // Remplir les stacks de pièces existants d'abord
            var allItems = new System.Collections.Generic.List<ItemDrop.ItemData>(inv.GetAllItems());
            foreach (var item in allItems)
            {
                if (remaining <= 0) break;
                if (item.m_shared?.m_name != coinName) continue;

                int canAdd = item.m_shared.m_maxStackSize - item.m_stack;
                if (canAdd <= 0) continue;

                int toAdd     = System.Math.Min(remaining, canAdd);
                item.m_stack += toAdd;
                remaining    -= toAdd;
            }

            // Créer de nouveaux stacks pour le reste
            if (remaining > 0)
            {
                var prefab   = ObjectDB.instance?.GetItemPrefab("Coins");
                var coinDrop = prefab?.GetComponent<ItemDrop>();
                if (coinDrop != null)
                {
                    int maxStack = coinDrop.m_itemData.m_shared.m_maxStackSize;
                    while (remaining > 0)
                    {
                        var newItem  = coinDrop.m_itemData.Clone();
                        newItem.m_stack = System.Math.Min(remaining, maxStack);
                        if (!inv.AddItem(newItem)) break;   // inventaire plein
                        remaining -= newItem.m_stack;
                    }
                }
            }

            inv.Changed();
        }

        public static bool Withdraw(int amount, out string message)
        {
            if (amount <= 0) { message = "Montant invalide."; return false; }

            var player = Player.m_localPlayer;
            if (player == null) { message = "Joueur introuvable."; return false; }

            long balance = GetBalance();
            if (balance < amount)
            {
                message = $"Solde insuffisant : {balance} pièces.";
                return false;
            }

            var inv = player.GetInventory();
            bool hasCoins  = CountCoinsInInventory(inv) > 0;
            bool hasSpace  = inv.GetEmptySlots() > 0;
            if (!hasCoins && !hasSpace)
            {
                message = "Inventaire plein ! Faites de la place.";
                return false;
            }

            SetBalance(balance - amount);
            AddCoinsToInventory(inv, amount);
            message = $"Retiré {amount} pièces. Solde : {GetBalance()}";
            Plugin.Log.LogInfo($"[Bank] Retrait {amount}. Solde : {GetBalance()}");
            return true;
        }

        public static bool WithdrawAll(out string message)
        {
            long balance = GetBalance();
            if (balance <= 0) { message = "La banque est vide."; return false; }
            int amount = (int)System.Math.Min(balance, int.MaxValue);
            return Withdraw(amount, out message);
        }
    }
}
