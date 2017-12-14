namespace Adept_AIO.Champions.MissFortune.Core
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using Spell = Aimtec.SDK.Spell;

    class SpellManager
    {
        public static Spell Q, W, E, R;
        private static float lastR;

        public SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            Q.SetSkillshot(0.25f, (float)(50f * Math.PI / 160f), 1000f, false, SkillshotType.Cone);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 1000f);
            E.SetSkillshot(0.5f, 350f, 500f, false, SkillshotType.Circle);

            R = new Spell(SpellSlot.R, 1000f);
            R.SetSkillshot(0.5f, 100f, 2000f, false, SkillshotType.Line);

            Global.Orbwalker.PreMove += OnPreMove;
            Global.Orbwalker.PreAttack += OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
        }

        private void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe || args.SpellSlot != SpellSlot.R)
            {
                return;
            }

            lastR = Environment.TickCount;
        }

        private static void OnPreAttack(object sender, PreAttackEventArgs args)
        {
            if (IsUlting())
            {
                args.Cancel = true;
            }
        }

        private static void OnPreMove(object sender, PreMoveEventArgs args)
        {
            if (IsUlting())
            {
                args.Cancel = true;
            }
        }

        public static bool IsUlting()
        {
            return Global.Player.HasBuff("missfortunebulletsound") || Environment.TickCount - lastR <= 700;
        }

        public static Geometry.Sector Cone(Obj_AI_Base target)
        {
            var dir = target.ServerPosition + (target.ServerPosition - Global.Player.ServerPosition).Normalized();
            return new Geometry.Sector(target.ServerPosition.To2D(), dir.To2D(), Q.Width, 475 - target.BoundingRadius, 300);
        }

        public static void CastQ(Obj_AI_Base target)
        {
            var enemy = ExtendedTarget(target);
            if (enemy != null)
            {
                CastExtendedQ(target);
            }
            else if (target.IsValidSpellTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
        }

        public static Vector3 WalkBehindMinion(Obj_AI_Base target)
        {
            var minion = GameObjects.Enemy
                .Where(x =>
                    x.IsValidTarget(Q.Range) &&
                    (x.IsMinion || x.IsHero) &&
                    Cone(x).IsInside(Q.GetPrediction(target).CastPosition.To2D()) &&
                    x.NetworkId != target.NetworkId)
                    
                .OrderBy(x => x.Distance(Global.Player))
                .ThenBy(x => x.Health)
                .FirstOrDefault();

            if (target == null || minion == null || !minion.IsValid)
            {
                return Vector3.Zero;
            }

            var position = minion.ServerPosition + (minion.ServerPosition - target.ServerPosition).Normalized() * Global.Player.Distance(minion) / 2;

            var isValid = position.Distance(ObjectManager.GetLocalPlayer()) < 350;
            if (isValid && !position.PointUnderEnemyTurret())
            {
                return position;
            }

            return Vector3.Zero;
        }

        public static Obj_AI_Base ExtendedTarget(Obj_AI_Base target)
        {
            return GameObjects.Enemy
                .Where(x => 
                x.IsValidTarget(Q.Range) &&
                (x.IsMinion || x.IsHero) &&
                x.NetworkId != target.NetworkId &&
                Cone(x).IsInside(target.ServerPosition.To2D()) &&
                Cone(x).IsInside(Q.GetPrediction(target).CastPosition.To2D()))

                .OrderBy(x => x.Health)
                .ThenBy(x => x.Distance(target))
                .FirstOrDefault();
        }

        public static void CastExtendedQ(Obj_AI_Base target)
        {
            var enemy = ExtendedTarget(target);

            if (enemy == null)
            {
                return;
            }
        
            Q.CastOnUnit(enemy);
        }

        public static void CastW(Obj_AI_Base target)
        {
            W.Cast();
        }

        public static void CastE(Obj_AI_Base target)
        {
            E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target)
        {
            R.Cast(target);
        }
    }
}