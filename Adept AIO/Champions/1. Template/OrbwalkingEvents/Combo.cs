namespace Adept_AIO.Champions._1._Template.OrbwalkingEvents
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
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.Combo["E"].Enabled && target.IsValidTarget(SpellManager.E.Range))
            {
                SpellManager.CastE(target);
            }
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.R.Range);
            if (target == null)
            {
                return;
            }

     
            if (SpellManager.E.Ready && MenuConfig.Combo["E"].Enabled && target.IsValidTarget(SpellManager.E.Range))
            {
                SpellManager.CastE(target);
            }

            if (SpellManager.Q.Ready && MenuConfig.Combo["Q"].Enabled && target.IsValidTarget(SpellManager.Q.Range))
            {
                SpellManager.CastQ(target);
            }

            if (SpellManager.W.Ready && MenuConfig.Combo["W"].Enabled)
            {
                SpellManager.CastW(target);
            }

            if (!SpellManager.R.Ready || !MenuConfig.Combo["R"].Enabled)
            {
                return;
            }

            SpellManager.CastR(target);
        }
    }
}