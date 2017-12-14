namespace Adept_AIO.Champions.Xerath.OrbwalkingEvents
{
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Harass
    {
        public static void OnUpdate()
        {
            if (SpellManager.E.Ready && MenuConfig.Harass["E"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["E"].Value)
            {
                SpellManager.CastE();
            }

            if ((SpellManager.Q.Ready || SpellManager.Q.IsCharging) && MenuConfig.Harass["Q"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["Q"].Value)
            {
                SpellManager.CastQ();
            }

            if (SpellManager.W.Ready && MenuConfig.Harass["W"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["W"].Value)
            {
                SpellManager.CastW();
            }
        }
    }
}