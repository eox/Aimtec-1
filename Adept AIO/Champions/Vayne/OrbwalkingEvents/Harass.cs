namespace Adept_AIO.Champions.Vayne.OrbwalkingEvents
{
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Harass
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (!SpellManager.Q.Ready || MenuConfig.Harass["Q"].Value == 1)
            {
                return;
            }

            var t = Global.TargetSelector.GetTarget(SpellManager.Q.Range + Global.Player.AttackRange);
            if (t == null)
            {
                return;
            }

            SpellManager.CastQ(t, MenuConfig.Harass["Mode"].Value);
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.E.Range);
            if (target == null)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.Harass["Q"].Value == 1)
            {
                SpellManager.CastQ(target, MenuConfig.Harass["Mode"].Value);
            }

            if (SpellManager.E.Ready && MenuConfig.Harass["E"].Enabled && MenuConfig.Harass[target.ChampionName].Enabled)
            {
                SpellManager.CastE(target);
            }
        }
    }
}