namespace Adept_AIO.Champions.Jinx.OrbwalkingEvents
{
    using Aimtec.SDK.Extensions;
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
            var target = Global.TargetSelector.GetTarget(spellConfig.W.Range);

            if (target == null)
            {
                return;
            }

            var dist = target.Distance(Global.Player);

            if (spellConfig.Q.Ready && menuConfig.Harass["Q"].Enabled)
            {
                if (!spellConfig.IsQ2 && dist > spellConfig.DefaultAuotAttackRange && dist <= spellConfig.Q2Range || spellConfig.IsQ2 && dist <= spellConfig.DefaultAuotAttackRange)
                {
                    spellConfig.Q.Cast();
                }
            }

            if (spellConfig.W.Ready && menuConfig.Harass["W"].Enabled && dist <= menuConfig.Harass["W"].Value)
            {
                spellConfig.W.Cast(target);
            }
        }
    }
}