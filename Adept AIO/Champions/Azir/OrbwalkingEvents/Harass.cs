namespace Adept_AIO.Champions.Azir.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;

    class Harass
    {
        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.Q.Range + 400);
            if (target == null)
            {
                return;
            }

            var dist = target.Distance(Global.Player) - Global.Player.BoundingRadius - target.BoundingRadius;

            if (SpellConfig.Q.Ready && MenuConfig.Harass["Q"].Enabled && Global.Player.ManaPercent() > MenuConfig.Harass["Q"].Value && dist < SpellConfig.Q.Range + 200)
            {
                SpellConfig.CastQ(target);
            }

            if (SpellConfig.W.Ready && MenuConfig.Harass["W"].Enabled && Global.Player.ManaPercent() > MenuConfig.Harass["W"].Value && target.IsValidTarget(SpellConfig.W.Range))
            {
                SpellConfig.W.Cast(Global.Player.ServerPosition.Extend(target.ServerPosition, SpellConfig.W.Range));
            }

            if (SpellConfig.E.Ready && MenuConfig.Harass["E"].Enabled && Global.Player.ManaPercent() > MenuConfig.Harass["E"].Value)
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
            }
        }
    }
}