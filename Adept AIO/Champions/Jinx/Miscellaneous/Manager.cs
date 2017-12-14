namespace Adept_AIO.Champions.Jinx.Miscellaneous
{
    using Aimtec.SDK.Orbwalking;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;

    class Manager
    {
        private readonly Combo combo;
        private readonly Harass harass;
        private readonly JungleClear jungleClear;
        private readonly LaneClear laneClear;
        private readonly SpellConfig spellConfig;

        public Manager(Combo combo, Harass harass, LaneClear laneClear, JungleClear jungleClear, SpellConfig spellConfig)
        {
            this.combo = combo;
            this.harass = harass;
            this.laneClear = laneClear;
            this.jungleClear = jungleClear;
            this.spellConfig = spellConfig;
        }

        public void OnUpdate()
        {
            if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp)
            {
                return;
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    combo.OnUpdate();
                    break;
                case OrbwalkingMode.Mixed:
                    harass.OnUpdate();
                    break;
                case OrbwalkingMode.Laneclear:
                    laneClear.OnUpdate();
                    jungleClear.OnUpdate();
                    break;
                case OrbwalkingMode.Lasthit:
                case OrbwalkingMode.None:
                    if (spellConfig.IsQ2)
                    {
                        spellConfig.Q.Cast();
                    }
                    break;
            }
        }
    }
}