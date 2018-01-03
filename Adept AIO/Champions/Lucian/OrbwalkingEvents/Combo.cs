namespace Adept_AIO.Champions.Lucian.OrbwalkingEvents
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

            if (SpellManager.E.Ready && MenuConfig.Combo["E1"].Value == 0)
            {
                SpellManager.CastE(target, MenuConfig.Combo["Mode1"].Value);
            }
            else if (SpellManager.Q.Ready)
            {
                SpellManager.CastQ(target);
            }
            else if (SpellManager.W.Ready)
            {
                SpellManager.W.Cast(target);
            }
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.ExtendedRange);
            if (target != null)
            {
                if (Global.Player.HasBuff("LucianR") && target.IsValidAutoRange() && SpellManager.E.Ready)
                {
                    SpellManager.R.Cast();
                }

                if (SpellManager.E.Ready && MenuConfig.Combo["E1"].Value == 1)
                {
                    SpellManager.CastE(target, MenuConfig.Combo["Mode1"].Value);
                }
                else if (SpellManager.Q.Ready && !target.IsValidAutoRange())
                {
                    SpellManager.CastQExtended(target);
                }
                else if (SpellManager.R.Ready && MenuConfig.Combo["Last"].Enabled && !SpellManager.Q.Ready && !SpellManager.W.Ready && !SpellManager.E.Ready &&
                         Global.Player.ManaPercent() >= 35)
                {
                    SpellManager.CastR(target);
                }
            }
        }
    }
}