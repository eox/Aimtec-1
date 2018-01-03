namespace Adept_AIO.Champions.Yorick.OrbwalkingEvents
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Combo
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (target == null || !target.IsHero)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.Combo["Q"].Enabled)
            {
                SpellManager.CastQ(target);
            }
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(1000);
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.Combo["E"].Enabled && target.IsValidTarget(SpellManager.E.Range))
            {
                SpellManager.CastE(target);
            }

            if (SpellManager.W.Ready && MenuConfig.Combo["W"].Enabled && target.IsValidTarget(SpellManager.W.Range))
            {
                SpellManager.CastW(target);
            }

            if (!SpellManager.R.Ready || !MenuConfig.Combo["R"].Enabled || target.HealthPercent() > MenuConfig.Combo["R"].Value || !target.IsValidTarget(SpellManager.R.Range))
            {
                return;
            }

            SpellManager.CastR(target.ServerPosition);
        }
    }
}