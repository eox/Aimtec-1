namespace Adept_AIO.Champions.Draven.Miscellaneous
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Extensions;
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
            try
            {
                if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp || Global.Player.IsRecalling())
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}