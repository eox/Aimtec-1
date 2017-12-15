namespace Adept_AIO.Champions.Graves.OrbwalkingEvents
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Harass
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.Harass["E"].Enabled && target.IsValidTarget(SpellManager.E.Range))
            {
                SpellManager.CastE(target);
            }
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.Q.Range);
            if (target == null)
            {
                return;
            }

            var canQ = SpellManager.Q.Ready && MenuConfig.Harass["Q"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["Q"].Value;

            if (SpellManager.E.Ready && MenuConfig.Harass["E"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["E"].Value && canQ)
            {
                SpellManager.CastE(target);
            }

            if (canQ)
            {
                SpellManager.CastQ(target);
            }

            if (SpellManager.W.Ready &&
                MenuConfig.Harass["W"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.Harass["W"].Value && target.IsHardCc())
            {
                SpellManager.CastW(target);
            }
        }
    }
}