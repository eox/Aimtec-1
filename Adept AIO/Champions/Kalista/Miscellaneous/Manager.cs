namespace Adept_AIO.Champions.Kalista.Miscellaneous
{
    using OrbwalkerEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class Manager
    {
        public static void OnUpdate()
        {
            if (Global.Player.IsDead)
            {
                return;
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo.OnUpdate();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass.OnUpdate();
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear.OnUpdate();
                    JungleClear.OnUpdate();
                    break;
            }
        }
    }
}