﻿namespace Adept_AIO.Champions.Graves.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Unit_Extensions;

    class JungleClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (target == null)
            {
                return;
            }

            if (SpellManager.E.Ready && MenuConfig.JungleClear["E"].Enabled)
            {
                SpellManager.CastE(target);
            }
        }

        public static void OnUpdate()
        {
            var creep = GameObjects.Jungle.OrderByDescending(x => x.MaxHealth).ThenBy(x => x.Distance(Global.Player)).FirstOrDefault(x => x.IsValidTarget(700) && x.MaxHealth > 15);
            if (creep == null )
            {
                return;
            }


            if (SpellManager.Q.Ready && MenuConfig.JungleClear["Q"].Enabled)
            {
                SpellManager.CastQ(creep);
            }

            if (SpellManager.W.Ready && MenuConfig.JungleClear["W"].Enabled)
            {
                SpellManager.CastW(creep);
            }
        }
    }
}