namespace Adept_AIO.Champions.Yorick.OrbwalkingEvents
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
            if (target == null || !target.IsHero)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.Harass["Q"].Enabled)
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

            if (SpellManager.E.Ready && MenuConfig.Harass["E"].Enabled && Global.Player.ManaPercent() >= MenuConfig.Harass["E"].Value)
            {
                SpellManager.CastE(target);
            }

            if (SpellManager.W.Ready &&
                MenuConfig.Harass["W"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.Harass["W"].Value && (target.IsHardCc() || target.Distance(Global.Player) <= SpellManager.W.Range - 150))
            {
                SpellManager.CastW(target);
            }
        }
    }
}