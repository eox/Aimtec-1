namespace Adept_AIO.Champions.MissFortune.Miscellaneous
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Generic;
    using SDK.Unit_Extensions;

    class Automatic
    {
        public Automatic()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(1000);

            if (target == null || Global.Player.IsDead || Global.Orbwalker.IsWindingUp || SpellManager.IsUlting())
            {
                return;
            }

            if (SpellManager.Q.Ready)
            {
                if (MenuConfig.Automatic["Q"].Enabled && target.Health < Global.Player.GetSpellDamage(target, SpellSlot.Q))
                {
                    SpellManager.CastQ(target);
                }
              
                if (MenuConfig.Automatic["QAuto"].Enabled)
                {
                    if (MenuConfig.Automatic["Path"].Enabled)
                    {
                        var wM = SpellManager.WalkBehindMinion(target);
                        if (!wM.IsZero)
                        {
                            Global.Orbwalker.Move(wM);
                        }
                    }

                    SpellManager.CastExtendedQ(target);
                }
            }

            if (!SpellManager.R.Ready)
            {
                return;
            }

            if (MenuConfig.Automatic["RCC"].Enabled && target.IsHardCc() && target.Distance(Global.Player) > Global.Player.AttackRange + 200 && target.CountAllyHeroesInRange(1000) >= 1)
               // || target.Health < Dmg.Ult(target) && MenuConfig.Automatic["R"].Enabled)
            {
                if (SpellManager.E.Ready && target.IsValidTarget(SpellManager.E.Range))
                {
                    SpellManager.CastE(target);
                }

                SpellManager.CastR(target);
            }
        }
    }
}