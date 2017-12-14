namespace Adept_AIO.Champions.Jinx.OrbwalkingEvents
{
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Combo
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public Combo(SpellConfig spellConfig, MenuConfig menuConfig)
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

            if (spellConfig.E.Ready && menuConfig.Combo["Close"].Enabled && target.Distance(Global.Player) <= Global.Player.AttackRange - 250)
            {
                spellConfig.E.Cast(target);
            }

            if (spellConfig.Q.Ready && menuConfig.Combo["Q"].Enabled)
            {
                if (!spellConfig.IsQ2 && dist > spellConfig.DefaultAuotAttackRange && dist <= spellConfig.Q2Range || spellConfig.IsQ2 && dist <= spellConfig.DefaultAuotAttackRange)
                {
                    spellConfig.Q.Cast();
                }
            }

            if (spellConfig.W.Ready && menuConfig.Combo["W"].Enabled && dist <= menuConfig.Combo["W"].Value && target.Distance(Global.Player) > Global.Player.AttackRange + 200)
            {
                spellConfig.W.Cast(target);
            }
        }
    }
}