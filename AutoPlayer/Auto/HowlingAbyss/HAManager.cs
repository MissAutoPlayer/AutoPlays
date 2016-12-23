using EloBuddy;
using EloBuddy.SDK;

// ReSharper disable InconsistentNaming

namespace AutoSharp.Auto.HowlingAbyss
{
    public static class HAManager
    {
        public static void Load()
        {
            Game.OnTick += DecisionMaker.OnTick;
            ARAMShopAI.Main.Init();
        }

        public static void Unload()
        {
            Game.OnUpdate -= DecisionMaker.OnTick;
        }

        public static void FastHalt()
        {
            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;
        }

    }
}
