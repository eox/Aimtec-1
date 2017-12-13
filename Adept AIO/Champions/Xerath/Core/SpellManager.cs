namespace Adept_AIO.Champions.Xerath.Core
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Generic;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using Spell = Aimtec.SDK.Spell;

    class SpellManager
    {
        public static bool CastingUltimate => Global.Player.HasBuff("XerathLocusOfPower2");

        private static float _lastRCast;

        private static readonly int[] UltiShots = { 0, 3, 4, 5 };

        public static int GetUltiShots()
        {
            return UltiShots[ObjectManager.GetLocalPlayer().SpellBook.GetSpell(SpellSlot.R).Level];
        }

        public static Spell Q, W, E, R;

        public SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 1600);
            Q.SetSkillshot(0.6f, 95f, 3000f, false, SkillshotType.Line, false, HitChance.Medium);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1400, 1.5f);

            W = new Spell(SpellSlot.W, 1100);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 1050);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 2500);
            R.SetSkillshot(0.7f, 130f, float.MaxValue, false, SkillshotType.Circle);
        }
        
        public static void CastQ(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(Q.ChargedMaxRange))
            {
                return;
            }

            if (!Q.IsCharging)
            {
                Q.StartCharging(target.ServerPosition);
            }

            var rect = QRealRect(target);
            if (rect == null || !Q.IsCharging)
            {
                return;
            }
            var targetPosIn250ms = target.Position + (target.Position - Q.GetPrediction(target).CastPosition).Normalized() * (0.25f * target.MoveSpeed);

            if(rect.IsInside(targetPosIn250ms.To2D()) && rect.End.Distance(targetPosIn250ms) > target.BoundingRadius)
            Q.ShootChargedSpell(Q.GetPrediction(target).CastPosition);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(W.Range))
            {
                return;
            }

            W.Cast(target);
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(E.Range))
            {
                return;
            }

            var rect = ERect(target);
            if (rect == null)
            {
                return;
            }

            if (GameObjects.EnemyMinions.OrderBy(x => x.Distance(Global.Player)).
                Any(x => rect.IsInside(x.ServerPosition.To2D()) && rect.Start.Distance(x) < rect.Start.Distance(target)))
            {
                return;
            }

            E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (!CastingUltimate)
            {
                if (Global.Player.CountEnemyHeroesInRange(1000) == 0)
                {
                    R.Cast();
                }
            }
            else if (Game.TickCount - _lastRCast > 700)
            {
                R.Cast(target);
            }
        }

        public static Geometry.Rectangle QRealRect(Obj_AI_Base target)
        {
            return new Geometry.Rectangle(Global.Player.ServerPosition.To2D(),
                Global.Player.ServerPosition.Extend(Q.GetPrediction(target).CastPosition, Q.Range).To2D(),
                Q.Width);
        }

        public static Geometry.Rectangle ERect(Obj_AI_Base target)
        {
            return new Geometry.Rectangle(Global.Player.ServerPosition.To2D(),
                (Global.Player.ServerPosition + target.ServerPosition).To2D().Normalized() * E.Range,
                E.Width + 80);
        }

        public static Geometry.Circle WCircle(Obj_AI_Base target)
        {
            return new Geometry.Circle(W.GetPrediction(target).CastPosition.To2D(), W.Range / 2);
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (sender == null ||
                !sender.IsMe ||
                args.SpellData.Name != "XerathLocusPulse")
            {
                return;
            }

            _lastRCast = Game.TickCount;
        }
    }
}