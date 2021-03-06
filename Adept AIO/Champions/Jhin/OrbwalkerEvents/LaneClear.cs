﻿namespace Adept_AIO.Champions.Jhin.OrbwalkerEvents
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        public static void OnUpdate()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(1500) != 0)
            {
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValidTarget(SpellManager.Q.Range));
            if (minion == null)
            {
                return;
            }

            if(MenuConfig.LaneClear["Q"].Enabled && SpellManager.Q.Ready)
            {
                var range = SpellManager.Q.Range;
                var circle = new Geometry.Circle(minion.ServerPosition.To2D(), range);
                var possible = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(range, false, true, circle.Center.To3D())).OrderBy(x => x.Distance(minion));

                if (possible.Count() >= MenuConfig.LaneClear["Q"].Value)
                {
                    SpellManager.Q.Cast(minion); 
                }
            }

            if (MenuConfig.LaneClear["E"].Enabled && SpellManager.E.Ready)
            {
                var range = SpellManager.E.Range;
                var circle = new Geometry.Circle(minion.ServerPosition.To2D(), range);
                var count = GameObjects.EnemyMinions.Count(x => x.IsValidTarget(range, false, true, circle.Center.To3D()));

                if (count >= MenuConfig.LaneClear["E"].Value)
                {
                    SpellManager.E.Cast(minion);
                }
            }
        }
    }
}
