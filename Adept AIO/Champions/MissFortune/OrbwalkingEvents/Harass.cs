namespace Adept_AIO.Champions.MissFortune.OrbwalkingEvents
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Harass
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (target == null || !target.IsHero)
            {
                return;
            }

            if (SpellManager.W.Ready &&
                MenuConfig.Harass["W"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.Harass["W"].Value)
            {
                SpellManager.CastW(target);
            }
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.Q.Range);
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.Harass["E"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["E"].Value)
            {
                SpellManager.CastE(target);
            }

            if (SpellManager.Q.Ready && MenuConfig.Harass["Q"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["Q"].Value)
            {
                SpellManager.CastExtendedQ(target);
            }
        }
    }
}