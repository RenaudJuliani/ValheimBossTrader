using System.Collections.Generic;

namespace ValheimBossTrader
{
    /// <summary>
    /// Gère le filtre de catégorie en remplaçant directement m_trader.m_items
    /// par une liste filtrée, puis en la restaurant à la fermeture.
    /// </summary>
    public static class CategoryFilter
    {
        public static Category? Active { get; private set; } = null;

        private static readonly Dictionary<string, Category> _lookup = new();
        private static List<Trader.TradeItem> _fullList = null;

        public static void Register(string prefabName, Category cat)
            => _lookup[prefabName] = cat;

        public static void ClearLookup()
        {
            _lookup.Clear();
            Active    = null;
            _fullList = null;
        }

        /// <summary>Applique un filtre et rafraîchit la liste du marchand.</summary>
        public static void SetCategory(Category? cat)
        {
            var store = StoreGui.instance;
            if (store?.m_trader == null) return;

            // Toujours restaurer la liste complète en premier
            if (_fullList != null)
            {
                store.m_trader.m_items = _fullList;
                _fullList = null;
            }

            Active = cat;

            if (cat != null)
            {
                // Sauvegarder puis filtrer
                _fullList = new List<Trader.TradeItem>(store.m_trader.m_items);

                var filtered = new List<Trader.TradeItem>();
                foreach (var item in _fullList)
                {
                    if (Matches(item)) filtered.Add(item);
                }
                store.m_trader.m_items = filtered;
            }

            RefreshList();
        }

        /// <summary>Restaure la liste complète sans filtrage.</summary>
        public static void Reset()
        {
            var store = StoreGui.instance;
            if (_fullList != null && store?.m_trader != null)
                store.m_trader.m_items = _fullList;

            _fullList = null;
            Active    = null;
        }

        private static bool Matches(Trader.TradeItem item)
        {
            var name = item.m_prefab?.name;
            return name != null
                && _lookup.TryGetValue(name, out var cat)
                && cat == Active.Value;
        }

        private static System.Reflection.MethodInfo _fillListMethod;

        private static void RefreshList()
        {
            if (StoreGui.instance == null) return;
            if (_fillListMethod == null)
                _fillListMethod = typeof(StoreGui).GetMethod(
                    "FillList",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public   |
                    System.Reflection.BindingFlags.NonPublic);
            _fillListMethod?.Invoke(StoreGui.instance, null);
        }
    }
}
