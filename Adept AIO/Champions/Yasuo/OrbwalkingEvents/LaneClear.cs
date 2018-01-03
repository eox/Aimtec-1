namespace Adept_AIO.Champions.Yasuo.OrbwalkingEvents
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        public static void OnPostAttack()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) != 0)
            {
                return;
            }

            if (SpellConfig.Q.Ready && Extension.CurrentMode == Mode.Normal)
            {
                var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(SpellConfig.Q.Range));
                if (minion == null)
                {
                    return;
                }

                SpellConfig.Q.Cast(minion);
            }

            if (SpellConfig.E.Ready && MenuConfig.LaneClear["EAA"].Enabled)
            {
                var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.Distance(Global.Player) <= SpellConfig.E.Range && !x.HasBuff("YasuoDashWrapper"));

                if (minion == null || MenuConfig.LaneClear["Turret"].Enabled && minion.IsUnderEnemyTurret() ||
                    MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) != 0)
                {
                    return;
                }

                switch (MenuConfig.LaneClear["Mode"].Value)
                {
                    case 1 when minion.Health < Global.Player.GetSpellDamage(minion, SpellSlot.E):
                        SpellConfig.E.CastOnUnit(minion);
                        break;
                    case 2:
                        SpellConfig.E.CastOnUnit(minion);
                        break;
                }
            }
        }

        public static void OnUpdate()
        {
            if (SpellConfig.E.Ready && !MenuConfig.LaneClear["EAA"].Enabled)
            {
                var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget() && x.Distance(Global.Player) <= SpellConfig.E.Range && !x.HasBuff("YasuoDashWrapper"));

                if (!SpellConfig.E.Ready || minion == null || MenuConfig.LaneClear["Turret"].Enabled && minion.IsUnderEnemyTurret() ||
                    MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) != 0)
                {
                    return;
                }

                switch (MenuConfig.LaneClear["Mode"].Value)
                {
                    case 1:

                        if (minion.Health > Global.Player.GetSpellDamage(minion, SpellSlot.E))
                        {
                            return;
                        }

                        SpellConfig.E.CastOnUnit(minion);
                        break;
                    case 2:
                        SpellConfig.E.CastOnUnit(minion);
                        break;
                }
            }

            if (SpellConfig.Q.Ready)
            {
                switch (Extension.CurrentMode)
                {
                    case Mode.Normal:
                    {
                        var m = GameObjects.EnemyMinions.LastOrDefault(x => x.IsValidTarget(SpellConfig.Q.Range));
                        if (m == null || Global.Player.IsDashing())
                        {
                            return;
                        }

                        SpellConfig.Q.Cast(m);
                    }
                        break;
                    case Mode.Tornado:
                    {
                        var m = GameObjects.EnemyMinions.LastOrDefault(x => x.IsValidTarget(SpellConfig.Q.Range));
                        if (m == null || Global.Player.IsDashing())
                        {
                            return;
                        }

                        var rect = SpellConfig.Q3Rect(m);
                        var count = GameObjects.EnemyMinions.Count(x => rect.IsInside(x.ServerPosition.To2D()));

                        if (MenuConfig.LaneClear["Q3"].Enabled && count >= 3)
                        {
                            SpellConfig.Q.Cast(m);
                        }
                    }

                        break;
                    case Mode.DashingTornado:
                    case Mode.Dashing:
                        var dashM = GameObjects.EnemyMinions.FirstOrDefault(x => x.Distance(Global.Player.GetDashInfo().EndPos) <= 220);
                        if (dashM == null)
                        {
                            return;
                        }

                        var circle = new Geometry.Circle(Global.Player.GetDashInfo().EndPos, 220);
                        var circleCount = GameObjects.EnemyMinions.Count(x => !x.IsDead && x.IsValidTarget() && circle.Center.Distance(x.ServerPosition) <= circle.Radius);

                        if (circleCount >= 3)
                        {
                            Console.WriteLine($"COUNT: {circleCount}");
                            SpellConfig.Q.Cast(dashM);
                        }
                        break;
                }
            }
        }
    }
}