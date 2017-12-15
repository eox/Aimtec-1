namespace Adept_AIO.Champions.Yorick.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class JungleClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (target == null)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.JungleClear["Q"].Enabled && Global.Player.ManaPercent() > MenuConfig.JungleClear["Q"].Value)
            {
                SpellManager.CastQ(target);
            }
        }

        public static void OnUpdate()
        {
            var creep = GameObjects.Jungle.OrderByDescending(x => x.MaxHealth).ThenBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValidTarget(700) && x.MaxHealth > 15);
            if (creep == null )
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.JungleClear["E"].Enabled && Global.Player.ManaPercent() > MenuConfig.JungleClear["E"].Value)
            {
                SpellManager.CastE(creep);
            }

            if (SpellManager.W.Ready && MenuConfig.JungleClear["W"].Enabled && Global.Player.ManaPercent() > MenuConfig.JungleClear["W"].Value)
            {
                SpellManager.CastW(creep);
            }
        }
    }
}