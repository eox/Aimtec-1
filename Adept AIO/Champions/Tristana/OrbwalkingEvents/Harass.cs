namespace Adept_AIO.Champions.Tristana.OrbwalkingEvents
{
    using Core;
    using SDK.Unit_Extensions;

    class Harass
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public Harass(SpellConfig spellConfig, MenuConfig menuConfig)
        {
            this.spellConfig = spellConfig;
            this.menuConfig = menuConfig;
        }

        public void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(spellConfig.FullRange);

            if (target == null)
            {
                return;
            }

            if (spellConfig.Q.Ready && menuConfig.Harass["Q"].Enabled)
            {
                spellConfig.Q.Cast();
            }

            if (spellConfig.E.Ready && menuConfig.Harass["E"].Enabled)
            {
                if (!menuConfig.Harass[target.ChampionName].Enabled)
                {
                    return;
                }
                spellConfig.E.CastOnUnit(target);
            }
        }
    }
}