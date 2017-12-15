namespace Adept_AIO.Champions.Zed.Miscellaneous
{
    using System;
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class Manager
    {
        public static void OnUpdate()
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
                    case OrbwalkingMode.Lasthit:
                        Lasthit.OnUpdate();
                        break;
                }

                if (Global.Orbwalker.Mode != OrbwalkingMode.Mixed)
                {
                    PermaSpells();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void PermaSpells()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && x.IsHero))
            {
                if (MenuConfig.Misc["Q"].Enabled && SpellManager.Q.Ready)
                {
                    SpellManager.CastQ(enemy);
                }

                if (MenuConfig.Misc["E"].Enabled && SpellManager.E.Ready)
                {
                    SpellManager.CastE(enemy);
                }
            }
        }
    }
}