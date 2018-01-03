namespace Adept_AIO.Champions.Tristana.OrbwalkingEvents
{
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Combo
    {
        private readonly Dmg dmg;
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public Combo(SpellConfig spellConfig, MenuConfig menuConfig, Dmg dmg)
        {
            this.spellConfig = spellConfig;
            this.menuConfig = menuConfig;
            this.dmg = dmg;
        }

        public void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(spellConfig.FullRange);

            if (target == null)
            {
                return;
            }

            if (spellConfig.E.Ready && menuConfig.Combo["E"].Enabled && menuConfig.Combo[target.ChampionName].Enabled)
            {
                spellConfig.E.CastOnUnit(target);
            }

            if (spellConfig.Q.Ready && menuConfig.Combo["Q"].Enabled)
            {
                spellConfig.Q.Cast();
            }

            if (spellConfig.W.Ready && menuConfig.Combo["W"].Enabled && target.Health < dmg.Damage(target) * 2 && target.Distance(Global.Player) > Global.Player.AttackRange + 100 &&
                Global.Player.CountEnemyHeroesInRange(2000) <= 2 && target.ServerPosition.CountAllyHeroesInRange(500) == 0)
            {
                spellConfig.W.Cast(target);
            }
        }
    }
}