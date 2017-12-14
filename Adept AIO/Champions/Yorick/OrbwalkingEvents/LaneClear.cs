namespace Adept_AIO.Champions.Yorick.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        public static void OnUpdate()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) > 0)
            {
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValidTarget(Global.Player.AttackRange));

            if (minion == null)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.LaneClear["Q"].Enabled && Global.Player.ManaPercent() >= MenuConfig.LaneClear["Q"].Value &&
                minion.Health < Global.Player.GetAutoAttackDamage(minion) +  Global.Player.GetSpellDamage(minion, SpellSlot.Q))
            {
                SpellManager.CastQ(minion);
            }

            if (MenuConfig.LaneClear["Shove"].Enabled)
            {
                if (SpellManager.W.Ready)
                {
                    SpellManager.W.Cast(minion, true, 3);
                }

                if (SpellManager.E.Ready)
                {
                    SpellManager.E.Cast(minion, true, 3);
                }

                if (SpellManager.Q.Ready)
                {
                    SpellManager.Q.Cast(minion);
                }
            }

            if (SpellManager.E.Ready &&
                MenuConfig.LaneClear["E"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.LaneClear["E"].Value)
            {
                SpellManager.CastE(minion);
            }
        }
    }
}