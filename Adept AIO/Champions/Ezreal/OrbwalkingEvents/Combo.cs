namespace Adept_AIO.Champions.Ezreal.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Combo
    {
        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.Q.Range);
            if (target == null)
            {
                return;
            }

            if (SpellConfig.Q.Ready && MenuConfig.Combo["Q"].Enabled)
            {
                if (target.IsValidTarget(SpellConfig.Q.Range))
                {
                    SpellConfig.Q.Cast(target);
                }
                else if (MenuConfig.Combo["QM"].Enabled && Global.Player.GetBuffCount("") >= 5)
                {
                    var objects = GameObjects.Enemy.FirstOrDefault(x => x.IsValidTarget(SpellConfig.Q.Range) && x.MaxHealth >= 10);
                    if (objects != null)
                    {
                        SpellConfig.Q.Cast(objects);
                    }
                }
            }

            if (SpellConfig.W.Ready && Global.Player.ManaPercent() >= MenuConfig.Combo["W"].Value)
            {
                if (MenuConfig.Combo["W"].Enabled && target.IsValidTarget(SpellConfig.W.Range))
                {
                    SpellConfig.W.Cast(target);
                }

                if (MenuConfig.Combo["WA"].Enabled)
                {
                    var ally = GameObjects.AllyHeroes.FirstOrDefault(x => x.IsValidTarget(SpellConfig.W.Range - 100));
                    if (ally != null)
                    {
                        SpellConfig.W.Cast(ally);
                    }
                }
            }

            if (SpellConfig.E.Ready && MenuConfig.Combo["E"].Enabled && Global.Player.ManaPercent() >= 30 && target.IsValidTarget(SpellConfig.E.Range) &&
                Global.Player.GetSpellDamage(target, SpellSlot.E) + Global.Player.GetAutoAttackDamage(target) >= target.Health)
            {
                SpellConfig.E.Cast(target);
            }
        }
    }
}
