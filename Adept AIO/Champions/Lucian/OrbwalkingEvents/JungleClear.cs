namespace Adept_AIO.Champions.Lucian.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class JungleClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = GameObjects.Jungle.FirstOrDefault(x => x.IsValidTarget() && x.MaxHealth > 20);
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.JungleClear["E4"].Value == 0)
            {
                SpellManager.CastE(target, MenuConfig.JungleClear["Mode4"].Value);
            }
            else if (SpellManager.Q.Ready && MenuConfig.JungleClear["Q"].Enabled)
            {
                SpellManager.CastQ(target);
            }
            else if (SpellManager.W.Ready && MenuConfig.JungleClear["W"].Enabled)
            {
                SpellManager.W.Cast(target);
            }
        }

        public static void OnUpdate()
        {
            var target = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(SpellManager.ExtendedRange));
            if (target != null)
            {
                if (SpellManager.E.Ready && MenuConfig.JungleClear["E4"].Value == 1)
                {
                    SpellManager.CastE(target, MenuConfig.JungleClear["Mode4"].Value);
                }
            }
        }
    }
}