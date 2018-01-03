namespace Adept_AIO.Champions.Twitch.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;

    class Combo
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target;
            if (target == null || !(args.Target is Obj_AI_Hero))
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.Combo["Q"].Enabled && target.IsValidAutoRange())
            {
                SpellManager.Q.Cast();
            }
        }

        public static void OnUpdate()
        {
            if (MenuConfig.Combo["E"].Enabled)
            {
                var targ = GameObjects.EnemyHeroes.FirstOrDefault(x => x.HasBuff("twitchdeadlyvenom") && x.GetBuffCount("twitchdeadlyvenom") >= 6);
                if (targ != null)
                {
                    SpellManager.E.Cast();
                }
            }

            var target = Global.TargetSelector.GetTarget(SpellManager.R.Range);
            if (target == null)
            {
                return;
            }

            if (SpellManager.W.Ready && Global.Player.ManaPercent() >= 25 && MenuConfig.Combo["W"].Enabled && !(MenuConfig.Combo["W2"].Enabled && SpellManager.HasUltBuff()))
            {
                SpellManager.W.Cast(target);
            }

            if (MenuConfig.Combo["R"].Enabled)
            {
                if (Global.Player.CountEnemyHeroesInRange(1500) < MenuConfig.Combo["R2"].Value)
                {
                    return;
                }

                SpellManager.CastR(target);
            }
        }
    }
}