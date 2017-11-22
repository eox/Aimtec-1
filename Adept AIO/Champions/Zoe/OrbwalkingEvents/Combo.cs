﻿namespace Adept_AIO.Champions.Zoe.OrbwalkingEvents
{
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Combo
    {
        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellManager.PaddleStar.IsZero ? SpellManager.Q.Range + 200 : 3000);
            if (target == null)
            {
                return;
            }

            if (SpellManager.W.Ready &&
                MenuConfig.Combo["W"].Enabled &&
                target.HealthPercent() <= MenuConfig.Combo["W"].Value)
            {
                SpellManager.CastW(target);
            }

            if (SpellManager.E.Ready &&
                MenuConfig.Combo["E"].Enabled)
            {
                SpellManager.CastE(target);
            }

            if (!SpellManager.Q.Ready ||
                !MenuConfig.Combo["Q"].Enabled)
            {
                return;
            }

            if (SpellManager.R.Ready &&
                MenuConfig.Combo["R"].Enabled && target.Distance(Global.Player) < Global.Player.AttackRange && SpellManager.PaddleStar.IsZero)
            {
                SpellManager.CastR(target);
            }

            SpellManager.CastQ(target);
        }
    }
}