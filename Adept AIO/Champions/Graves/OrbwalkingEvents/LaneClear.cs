namespace Adept_AIO.Champions.Graves.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class LaneClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.LaneClear["E"].Enabled && Global.Player.ManaPercent() >= MenuConfig.LaneClear["E"].Value)
            {
                SpellManager.CastE(target);
            }
        }

        public static void OnUpdate()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) > 0)
            {
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).LastOrDefault(x => x.IsValidTarget(Global.Player.AttackRange));

            if (minion == null)
            {
                return;
            }

            if (SpellManager.Q.Ready &&
                MenuConfig.LaneClear["Q"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.LaneClear["Q"].Value)
            {
                var rect = SpellManager.QRect(minion);
                if (rect != null && GameObjects.EnemyMinions.Count(x => rect.IsInside(x.ServerPosition.To2D())) < 3)
                {
                    return;
                }
                SpellManager.CastQ(minion);
            }
        }
    }
}