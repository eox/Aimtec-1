﻿namespace Adept_AIO.Champions.Yorick.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) > 0)
            {
                return;
            }

            var target = args.Target as Obj_AI_Base;
            if (target == null)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.LaneClear["Q"].Enabled && Global.Player.ManaPercent() >= MenuConfig.LaneClear["Q"].Value)
            {
                SpellManager.CastQ(target);
            }
        }

        public static void OnUpdate()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) > 0)
            {
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).LastOrDefault(x => x.IsValidTarget(Global.Player.AttackRange + 200));

            if (minion == null)
            {
                return;
            }

            if (MenuConfig.LaneClear["Shove"].Enabled)
            {
                if (SpellManager.W.Ready)
                {
                    SpellManager.W.Cast(minion, true, 3);
                }

                if (SpellManager.E.Ready)
                {
                    SpellManager.E.Cast(minion, true, 3);
                }

                if (SpellManager.Q.Ready)
                {
                    SpellManager.Q.Cast(minion);
                }
            }

            if (SpellManager.E.Ready &&
                MenuConfig.LaneClear["E"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.LaneClear["E"].Value)
            {
                SpellManager.CastE(minion);
            }
        }
    }
}