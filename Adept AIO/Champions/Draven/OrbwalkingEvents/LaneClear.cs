namespace Adept_AIO.Champions.Draven.OrbwalkingEvents
{
    using System.Linq;
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

            var minion = GameObjects.EnemyMinions.OrderBy(x => x.Health).ThenBy(x => x.Distance(Global.Player)).LastOrDefault(x => x.IsValidTarget(Global.Player.AttackRange));

            if (minion == null)
            {
                return;
            }

            if (SpellManager.Q.Ready &&
                MenuConfig.LaneClear["Q"].Enabled &&
                Global.Player.ManaPercent() >= MenuConfig.LaneClear["Q"].Value)
            {
                SpellManager.CastQ();
            }

            if (SpellManager.W.Ready && MenuConfig.LaneClear["W"].Enabled && Global.Player.ManaPercent() >= MenuConfig.LaneClear["W"].Value)
            {
                SpellManager.CastW();
            }
        }
    }
}