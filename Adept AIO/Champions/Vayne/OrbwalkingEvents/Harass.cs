﻿namespace Adept_AIO.Champions.Vayne.OrbwalkingEvents
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;

    class Harass
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            if (!SpellManager.Q.Ready || MenuConfig.Harass["Q2"].Value == 1)
            {
                return;
            }

            SpellManager.CastQ(args.Target as Obj_AI_Base, MenuConfig.Harass["Mode2"].Value);
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(1000);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.Harass["Q2"].Value == 1)
            {
                SpellManager.CastQ(target, MenuConfig.Harass["Mode2"].Value);
            }

            if (SpellManager.E.Ready &&
                MenuConfig.Harass["E"].Enabled &&
                MenuConfig.Harass[target.ChampionName].Enabled)
            {
                SpellManager.CastE(target);
            }
        }
    }
}