namespace Adept_AIO.Champions.Graves.OrbwalkingEvents
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
            var target = args.Target as Obj_AI_Base;
            if (target == null || !target.IsHero)
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

            var canQ = SpellManager.Q.Ready && MenuConfig.Combo["Q"].Enabled && target.IsValidTarget(SpellManager.Q.Range);

            if (SpellManager.E.Ready && MenuConfig.Combo["E"].Enabled && target.IsValidTarget(SpellManager.E.Range) && canQ)
            {
                SpellManager.CastE(target);
            }

            if (canQ)
            {
                SpellManager.CastQ(target);
            }

            if (SpellManager.W.Ready && MenuConfig.Combo["W"].Enabled)
            {
                if (MenuConfig.Combo["WCC"].Enabled && !target.IsHardCc())
                {
                    return;
                }
                SpellManager.CastW(target);
            }

            if (!SpellManager.R.Ready)
            {
                return;
            }

            if (MenuConfig.Combo["RHealth"].Enabled && target.HealthPercent() > MenuConfig.Combo["RHealth"].Value)
            {
                return;
            }

            var rect = SpellManager.RRect(target);
            if (rect != null &&
                MenuConfig.Combo["RCount"].Enabled &&
                GameObjects.EnemyHeroes.Count(x => rect.IsInside(x.ServerPosition.To2D())) < MenuConfig.Combo["RCount"].Value)
            {
                return;
            }

            SpellManager.CastR(target);
        }
    }
}