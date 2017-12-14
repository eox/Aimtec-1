namespace Adept_AIO.Champions.Jinx.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class JungleClear
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public JungleClear(MenuConfig menuConfig, SpellConfig spellConfig)
        {
            this.menuConfig = menuConfig;
            this.spellConfig = spellConfig;
        }

        public void OnUpdate()
        {
            var minion = GameObjects.JungleLarge.FirstOrDefault(x => x.IsValidTarget(spellConfig.W.Range));
            if (minion == null)
            {
                return;
            }

            var dist = Global.Player.Distance(minion);

            if (spellConfig.W.Ready && menuConfig.JungleClear["W"].Enabled && menuConfig.JungleClear["W"].Value < Global.Player.ManaPercent() && dist <= 650 &&
                Global.Player.CountEnemyHeroesInRange(2000) == 0)
            {
                spellConfig.W.Cast(minion);
            }

            if (spellConfig.Q.Ready && menuConfig.JungleClear["Q"].Enabled)
            {
                if (!spellConfig.IsQ2 && dist > spellConfig.DefaultAuotAttackRange || spellConfig.IsQ2 && dist <= spellConfig.DefaultAuotAttackRange)
                {
                    spellConfig.Q.Cast();
                }
            }
        }
    }
}