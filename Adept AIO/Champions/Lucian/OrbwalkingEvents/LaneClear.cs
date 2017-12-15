namespace Adept_AIO.Champions.Lucian.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class LaneClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(1500) != 0)
            {
                return;
            }

            var target = args.Target as Obj_AI_Base;

            if (SpellManager.E.Ready && MenuConfig.LaneClear["E3"].Value == 0)
            {
                SpellManager.CastE(target, MenuConfig.LaneClear["Mode3"].Value);
            }
            else if (SpellManager.Q.Ready && MenuConfig.LaneClear["Q"].Enabled)
            {
                SpellManager.CastQ(target, MenuConfig.LaneClear["Q"].Value);
            }
            else if (SpellManager.W.Ready && MenuConfig.LaneClear["W"].Enabled && GameObjects.EnemyMinions.Count(x => x.IsValidTarget(1000)) >= MenuConfig.LaneClear["W"].Value)
            {
                SpellManager.W.Cast(target);
            }
        }

        public static void OnUpdate()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(1500) != 0 || MenuConfig.LaneClear["Mana"].Value > Global.Player.ManaPercent())
            {
                return;
            }

            var target = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(SpellManager.ExtendedRange));
            if (target != null)
            {
                if (SpellManager.E.Ready && MenuConfig.LaneClear["E3"].Value == 1)
                {
                    SpellManager.CastE(target, MenuConfig.LaneClear["Mode3"].Value);
                }
            }
        }
    }
}