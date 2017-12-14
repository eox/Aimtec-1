namespace Adept_AIO.Champions.Xerath.OrbwalkingEvents
{
    using Core;

    class Combo
    {
        public static void OnUpdate()
        {
            if (SpellManager.E.Ready && MenuConfig.Combo["E"].Enabled)
            {
                SpellManager.CastE();
            }

            if (SpellManager.Q.Ready && MenuConfig.Combo["Q"].Enabled)
            {
              
                SpellManager.CastQ();
            }

            if (SpellManager.W.Ready && MenuConfig.Combo["W"].Enabled)
            {
                SpellManager.CastW();
            }
        }
    }
}