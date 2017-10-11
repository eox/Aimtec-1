﻿using System.Linq;
using Adept_AIO.Champions.Vayne.Core;
using Adept_AIO.SDK.Unit_Extensions;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Orbwalking;

namespace Adept_AIO.Champions.Vayne.OrbwalkingMode
{
    internal class JungleClear
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            var mob = GameObjects.JungleLarge.FirstOrDefault(x => x.MaxHealth > 20);
            if (mob == null || !mob.IsValidTarget())
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.JungleClear["Q"].Value != 1)
            {
                SpellManager.CastQ(mob, MenuConfig.JungleClear["Mode"].Value);
            }

            if (SpellManager.E.Ready && MenuConfig.JungleClear["E"].Enabled)
            {
                SpellManager.CastE(mob);
            }
        }

        public static void OnUpdate()
        {
            var mob = GameObjects.Jungle.FirstOrDefault(x => x.Distance(Global.Player) <= SpellManager.Q.Range + Global.Player.AttackRange);
            if (mob == null)
            {
                return;
            }

            if (SpellManager.Q.Ready && MenuConfig.JungleClear["Q"].Value == 1)
            {
                SpellManager.CastQ(mob, MenuConfig.JungleClear["Mode"].Value);
            }
        }
    }
}
