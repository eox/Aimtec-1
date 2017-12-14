namespace Adept_AIO.Champions.Jinx.Miscellaneous
{
    using Aimtec.SDK.Orbwalking;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;

    class Manager
    {
        private readonly Combo combo;
        private readonly Harass harass;
        private readonly JungleClear jungleClear;
        private readonly LaneClear laneClear;

        public Manager(Combo combo, Harass harass, LaneClear laneClear, JungleClear jungleClear)
        {
            this.combo = combo;
            this.harass = harass;
            this.laneClear = laneClear;
            this.jungleClear = jungleClear;
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
            }
        }
    }
}