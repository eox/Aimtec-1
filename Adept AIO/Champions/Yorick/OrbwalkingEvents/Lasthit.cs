using System.Linq;

namespace Adept_AIO.Champions.Yorick.OrbwalkingEvents
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;

    class Lasthit
    {
      
        public static void OnUpdate()
        {
            if (MenuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) > 0)
            {
                return;
            }

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).LastOrDefault(x => x.IsValidTarget(Global.Player.AttackRange + 200));

            if (minion == null)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.LaneClear["Q"].Enabled && Global.Player.ManaPercent() >= MenuConfig.LaneClear["Q"].Value &&
                minion.Health < Global.Player.GetAutoAttackDamage(minion) + Global.Player.GetSpellDamage(minion, SpellSlot.Q))
            {
                SpellManager.CastQ(minion);
            }

            if (SpellManager.E.Ready && 
                minion.Health < Global.Player.GetSpellDamage(minion, SpellSlot.E) &&
                MenuConfig.LaneClear["E"].Enabled && 
                Global.Player.ManaPercent() >= MenuConfig.LaneClear["E"].Value)
            {
                SpellManager.CastE(minion);
            }
        }
    }
}
