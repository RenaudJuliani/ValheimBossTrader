namespace ValheimBossTrader
{
    /// <summary>
    /// Clés globales Valheim posées quand un boss est tué.
    /// Vérifiées via ZoneSystem.instance.GetGlobalKey(key).
    /// </summary>
    public static class BossKey
    {
        public const string Eikthyr  = "defeated_eikthyr";   // Meadows
        public const string Elder    = "defeated_gdking";     // Black Forest
        public const string Bonemass = "defeated_bonemass";   // Swamp
        public const string Moder    = "defeated_dragon";     // Mountain
        public const string Yagluth  = "defeated_goblinking"; // Plains
        public const string Queen    = "defeated_queen";      // Mistlands
        public const string Fader    = "defeated_fader";      // Ashlands
    }
}
