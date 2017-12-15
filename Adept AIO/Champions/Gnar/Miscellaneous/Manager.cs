namespace Adept_AIO.Champions.Gnar.Miscellaneous
{
    using Aimtec;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class Manager
    {
        public Manager()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp)
            {
                return;
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo.OnUpdate();
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear.OnUpdate();
                    JungleClear.OnUpdate();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass.OnUpdate();
                    break;
            }

            if (SpellManager.GnarState != GnarState.Mega || !MenuConfig.Misc["Auto"].Enabled)
            {
                return;
            }
            var t = Global.TargetSelector.GetTarget(SpellManager.R.Range);
            if (t != null)
            {
                SpellManager.CastR(t, MenuConfig.Misc["Auto"].Value);
            }
        }
    }
}