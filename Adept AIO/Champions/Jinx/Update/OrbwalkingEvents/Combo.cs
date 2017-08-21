﻿using Adept_AIO.Champions.Jinx.Core;
using Adept_AIO.SDK.Extensions;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Prediction.Skillshots;

namespace Adept_AIO.Champions.Jinx.Update.OrbwalkingEvents
{
    internal class Combo
    {
        private readonly SpellConfig SpellConfig;
        private readonly MenuConfig MenuConfig;

        public Combo(SpellConfig spellConfig, MenuConfig menuConfig)
        {
            SpellConfig = spellConfig;
            MenuConfig = menuConfig;
        }

        public void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.W.Range);

            if (target == null || Global.Orbwalker.IsWindingUp)
            {
                return;
            }

            var dist = target.Distance(Global.Player);

            if (SpellConfig.Q.Ready && MenuConfig.Combo["Q"].Enabled)
            {
                if (!SpellConfig.IsQ2 && dist > SpellConfig.DefaultAuotAttackRange && dist <= SpellConfig.Q2Range ||
                     SpellConfig.IsQ2 && dist <= SpellConfig.DefaultAuotAttackRange)
                {
                    SpellConfig.Q.Cast();
                }
            }

            if (SpellConfig.W.Ready && MenuConfig.Combo["W"].Enabled && dist <= MenuConfig.Combo["W"].Value)
            {
                SpellConfig.W.Cast(target);
            }
        }
    }
}