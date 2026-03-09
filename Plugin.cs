using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ValheimBossTrader
{
    [BepInDependency("ValheimLegends", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid    = "juliani.mods.valheim.bosstrader";
        public const string PluginName    = "ValheimBossTrader";
        public const string PluginVersion = "1.0.1";

        internal static ManualLogSource Log = null!;

        private readonly Harmony _harmony = new(PluginGuid);

        private void Awake()
        {
            Log = Logger;
            ModConfig.Init(Config);
            Log.LogInfo($"{PluginName} v{PluginVersion} chargé — {CountItems()} items configurés.");
            _harmony.PatchAll();
        }

        private void OnDestroy() => _harmony.UnpatchSelf();

        private static int CountItems()
        {
            int n = 0;
            foreach (var _ in TraderItems.GetAll()) n++;
            return n;
        }
    }
}
