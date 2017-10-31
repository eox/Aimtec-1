﻿namespace Adept_AIO.Champions.Kalista.Miscellaneous
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;

    class Automatic
    {
        public static void OnUpdate()
        {
            if (Global.Player.IsRecalling())
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.Misc["E"].Enabled && GameObjects.EnemyHeroes.Any(x => x.HasBuff("kalistaexpungemarker")))
            {
                var m = GameObjects.EnemyMinions.FirstOrDefault(x => x.HasBuff("kalistaexpungemarker") &&
                                                                     x.Health < Dmg.EDmg(x) &&
                                                                     x.IsValidTarget(SpellManager.E.Range));
                if (m != null)
                {
                    SpellManager.E.Cast();
                }
            }

            if (SpellManager.E.Ready &&
                GameObjects.Jungle.Count(x => x.HasBuff("kalistaexpungemarker") && Dmg.EDmg(x) > x.Health && x.IsValidSpellTarget(SpellManager.E.Range) && x.GetJungleType() != GameObjects.JungleType.Small) >= 1 &&
                MenuConfig.JungleClear["E"].Enabled)
            {
                if (Global.Player.Level == 1 && Global.Player.CountAllyHeroesInRange(2000) >= 1)
                {
                    return;
                }
                SpellManager.E.Cast();
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    var m = GameObjects.EnemyMinions.FirstOrDefault(x => x.Distance(Global.Player) <= 2000);
                    if (m != null &&
                        Global.Orbwalker.CanAttack() &&
                        Global.Player.CountEnemyHeroesInRange(Global.Player.AttackRange) <= 0 &&
                        MenuConfig.Combo["Minions"].Enabled &&
                        m.IsValidAutoRange())
                    {
                        Global.Orbwalker.Attack(m);
                    }
                    break;
                case OrbwalkingMode.Laneclear:
                    if (SpellManager.E.Ready &&
                        GameObjects.EnemyMinions.Any(x => x.HasBuff("kalistaexpungemarker") && Dmg.EDmg(x) > x.Health && x.IsValidSpellTarget(SpellManager.E.Range)) &&
                        MenuConfig.LaneClear["E"].Enabled)
                    {
                        SpellManager.E.Cast();
                    }
                    break;
                case OrbwalkingMode.None:
                    if (SpellManager.W.Ready && MenuConfig.Misc["W"].Enabled && Global.Player.CountEnemyHeroesInRange(1800) <= 0)
                    {
                        SpellManager.CastW();
                    }
                    break;
            }
            Console.WriteLine(Game.CursorPos);
            if (SpellManager.R.Ready && MenuConfig.Misc["R"].Enabled)
            {
                var soulbound = GameObjects.AllGameObjects.FirstOrDefault(x => x.Name == "Kalista_Base_P_LinkIcon.troy") as Obj_AI_Hero;
                if (soulbound != null && soulbound.HealthPercent() >= MenuConfig.Misc["R"].Value)
                {
                    SpellManager.R.Cast();
                }
            }
        }
    }
}