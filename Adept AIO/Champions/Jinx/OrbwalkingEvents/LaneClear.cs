namespace Adept_AIO.Champions.Jinx.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public LaneClear(MenuConfig menuConfig, SpellConfig spellConfig)
        {
            this.menuConfig = menuConfig;
            this.spellConfig = spellConfig;
        }

        public void OnUpdate()
        {
            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(spellConfig.W.Range));
            if (minion == null)
            {
                return;
            }

            var dist = Global.Player.Distance(minion);

            if (spellConfig.W.Ready && menuConfig.LaneClear["W"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) == 0)
            {
                if (dist > 800 && minion.Health < Global.Player.GetSpellDamage(minion, SpellSlot.W) && minion.UnitSkinName.ToLower().Contains("cannon"))
                {
                    spellConfig.W.Cast(minion);
                }
            }

            if (spellConfig.Q.Ready && menuConfig.LaneClear["Q"].Enabled)
            {
                if (!spellConfig.IsQ2 && dist > spellConfig.DefaultAuotAttackRange && dist <= spellConfig.Q2Range &&
                    GameObjects.EnemyMinions.Count(x => x.IsValidTarget(spellConfig.Q2Range) && x.Health < Global.Player.GetAutoAttackDamage(x)) >= 2 ||
                    spellConfig.IsQ2 && dist <= spellConfig.DefaultAuotAttackRange)
                {
                    spellConfig.Q.Cast();
                }
            }
        }
    }
}