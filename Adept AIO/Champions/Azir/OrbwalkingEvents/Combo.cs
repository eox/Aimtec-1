namespace Adept_AIO.Champions.Azir.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;

    class Combo
    {
        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.Q.Range + 400);
            if (target == null)
            {
                return;
            }

            if (SpellConfig.E.Ready && MenuConfig.Combo["E"].Enabled)
            {
                foreach (var soldier in SoldierManager.Soldiers)
                {
                    var rect = new Geometry.Rectangle(Global.Player.ServerPosition.To2D(), soldier.ServerPosition.To2D(), SpellConfig.E.Width);
                    var count = GameObjects.EnemyHeroes.Count(x => rect.IsInside(x.ServerPosition.To2D()));

                    if (count >= 1)
                    {
                        SpellConfig.E.Cast(soldier.ServerPosition);
                    }
                }

                if (target.HealthPercent() <= MenuConfig.Combo["EDmg"].Value)
                {
                    var soldier = SoldierManager.Soldiers.FirstOrDefault(x => x.Distance(target) <= 500 && !x.IsMoving);
                    if (soldier != null && soldier.ServerPosition != Vector3.Zero)
                    {
                        SpellConfig.E.Cast(soldier);
                    }
                }
            }

            if (SpellConfig.Q.Ready && MenuConfig.Combo["Q"].Enabled && target.IsValidTarget(SpellConfig.Q.Range))
            {
                if (SoldierManager.Soldiers.All(soldier => soldier.Distance(target) <= 200))
                {
                    return;
                }

                if (SoldierManager.Soldiers.Count >= MenuConfig.Combo["QCount"].Value)
                {
                    SpellConfig.CastQ(target, MenuConfig.Combo["Extend"].Enabled);
                }
            }

            if (SpellConfig.W.Ready && MenuConfig.Combo["W"].Enabled)
            {
                if (SpellConfig.Q.Ready && MenuConfig.Combo["Q"].Enabled)
                {
                    SpellConfig.W.Cast(Global.Player.ServerPosition.Extend(target.ServerPosition, SpellConfig.W.Range));
                }
                else if (target.IsValidTarget(SpellConfig.W.Range))
                {
                    SpellConfig.W.Cast(target);
                }
            }

            if (SpellConfig.R.Ready && MenuConfig.Combo["R"].Enabled && target.HealthPercent() <= 40 && target.IsValidTarget(SpellConfig.R.Range))
            {
                AzirHelper.Rect = new Geometry.Rectangle(target.ServerPosition.To2D(),
                    Global.Player.ServerPosition.Extend(target.ServerPosition, -SpellConfig.R.Width / 2f).To2D(),
                    SpellConfig.R.Width / 2f);
                if (AzirHelper.Rect.IsInside(target.ServerPosition.To2D()))
                {
                    SpellConfig.R.Cast(target);
                }
            }
        }
    }
}