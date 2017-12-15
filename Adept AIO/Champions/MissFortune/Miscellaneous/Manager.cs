namespace Adept_AIO.Champions.MissFortune.Miscellaneous
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Manager
    {
        public Manager()
        {
            Game.OnUpdate += OnUpdate;
            Global.Orbwalker.PostAttack += OnPostAttack;
        }

        private void OnPostAttack(object sender, PostAttackEventArgs args)
        {
            try
            {
                switch (Global.Orbwalker.Mode)
                {
                    case OrbwalkingMode.Combo:
                        Combo.PostAttack(sender, args);
                        break;
                    case OrbwalkingMode.Mixed:
                        Harass.PostAttack(sender, args);
                        break;
                    case OrbwalkingMode.Laneclear:
                        LaneClear.PostAttack(sender, args);
                        JungleClear.PostAttack(sender, args);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void OnUpdate()
        {
            try
            {
                if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp || Global.Player.IsRecalling() || SpellManager.IsUlting())
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
                        break;
                    case OrbwalkingMode.Lasthit:
                        Lasthit.OnUpdate();
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