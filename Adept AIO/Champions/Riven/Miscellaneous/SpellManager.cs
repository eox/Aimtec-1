namespace Adept_AIO.Champions.Riven.Miscellaneous
{
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using Core;
    using SDK.Generic;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    class SpellManager
    {
        private static bool canWq;
        private static bool canUseQ;
        private static bool canUseW;

        private static Obj_AI_Base unit;
        private static bool serverPosition;

        public static float LastR;

        private static readonly string[] InvulnerableSpells =
        {
            "FioraW",
            "kindrednodeathbuff",
            "Undying Rage",
            "JudicatorIntervention"
        };

        public static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.SpellData.Name)
            {
                case "RivenTriCleave":
                    Extensions.LastQCastAttempt = Game.TickCount;
                    canUseQ = false;
                    canWq = false;
                    serverPosition = false;

                    Extensions.CurrentQCount++;
                    if (Extensions.CurrentQCount > 3)
                    {
                        Extensions.CurrentQCount = 1;
                    }

                    Animation.Reset();
                    break;
                case "RivenMartyr":
                    canUseW = false;
                    break;
                case "RivenFengShuiEngine":
                    LastR = Game.TickCount;
                    Enums.UltimateMode = UltimateMode.Second;
                    Maths.DisableAutoAttack(200);
                    break;
                case "RivenIzunaBlade":
                    Enums.UltimateMode = UltimateMode.First;
                    break;
            }
        }

        public static void OnUpdate()
        {
            switch (unit)
            {
                case null: return;
                case Obj_AI_Hero _ when unit.HasBuff("FioraW") || unit.HasBuff("PoppyW"): return;
            }

            if (canWq)
            {
                SpellConfig.W.Cast();
                Global.Player.SpellBook.CastSpell(SpellSlot.Q, unit);
                DelayAction.Queue(600,
                    () =>
                    {
                        Global.Orbwalker.ResetAutoAttackTimer();
                        Global.Orbwalker.Attack(unit);
                    }, new CancellationToken(false));
            }

            if (canUseW && unit.IsValidTarget(SpellConfig.W.Range))
            {
                canUseW = false;

                if (Items.CanUseTiamat())
                {
                    Items.CastTiamat();
                    DelayAction.Queue(300, () => SpellConfig.W.Cast(unit), new CancellationToken(false));
                }

                SpellConfig.W.Cast(unit);
            }

            if (canUseQ)
            {
                if (Extensions.DidJustAuto)
                {
                    Extensions.DidJustAuto = false;
                    Global.Player.SpellBook.CastSpell(SpellSlot.Q, unit);
                }
                else if (serverPosition)
                {
                    SpellConfig.Q.CastOnUnit(unit);
                }
            }
        }

        public static void CastWq(Obj_AI_Base target)
        {
            unit = target;
            canWq = true;
        }

        public static void CastQ(Obj_AI_Base target, bool serverPosition = false)
        {
            unit = target;
            canUseQ = true;
            SpellManager.serverPosition = serverPosition;
        }

        public static void CastW(Obj_AI_Base target)
        {
            canUseW = true;
            unit = target;
        }

        public static void CastR2(Obj_AI_Base target)
        {
            if (InvulnerableSpells.Any(target.HasBuff))
            {
                return;
            }

            if (target.IsValidAutoRange())
            {
                if (Items.CanUseTiamat())
                {
                    Items.CastTiamat();
                    DelayAction.Queue(1000, () => SpellConfig.R2.Cast(target));
                }
                else
                {
                    DelayAction.Queue(500, () => SpellConfig.R2.Cast(target));
                }
                canUseQ = true;
            }
            else
            {
                SpellConfig.R2.Cast(target);
            }
        }
    }
}