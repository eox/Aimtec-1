namespace Adept_AIO.Champions.Ezreal.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Harass
    {
        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.Q.Range);
            if (target == null)
            {
                return;
            }

            if (SpellConfig.Q.Ready && MenuConfig.Harass["Q"].Enabled && target.IsValidTarget(SpellConfig.Q.Range))
            {
                SpellConfig.Q.Cast(target);
            }

            if (SpellConfig.W.Ready && Global.Player.ManaPercent() >= MenuConfig.Harass["W"].Value)
            {
                if (MenuConfig.Harass["W"].Enabled && target.IsValidTarget(SpellConfig.W.Range))
                {
                    SpellConfig.W.Cast(target);
                }

                if (MenuConfig.Harass["WA"].Enabled)
                {
                    var ally = GameObjects.AllyHeroes.FirstOrDefault(x => x.IsValidTarget(SpellConfig.W.Range - 100));
                    if (ally != null)
                    {
                        SpellConfig.W.Cast(ally);
                    }
                }
            }

            if (SpellConfig.E.Ready && MenuConfig.Harass["E"].Enabled && Global.Player.ManaPercent() >= 50 && target.IsValidTarget(SpellConfig.E.Range) &&
                Dmg.Damage(target) >= target.Health)
            {
                SpellConfig.E.Cast(target);
            }
        }
    }
}
