namespace Adept_AIO.Champions.Graves.Core
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using Geometry = SDK.Geometry_Related.Geometry;
    using Spell = Aimtec.SDK.Spell;

    class SpellManager
    {
        public static Vector3 WallForQ;
        public static Spell Q, W, E, R;

        public SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.25f, 70f, 900f, false, SkillshotType.Line);

            W = new Spell(SpellSlot.W, 850f);
            W.SetSkillshot(0.25f, 250f, 1650f, false, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 425f);
        
            R = new Spell(SpellSlot.R, 1000f);
            R.SetSkillshot(0.25f, 100f, 2100, false, SkillshotType.Line);

            Game.OnUpdate += GenereateWallForQ;
        }

        private static void GenereateWallForQ()
        {
            var target = Global.TargetSelector.GetTarget(Q.Range);
            if (target == null)
            {
                WallForQ = Vector3.Zero;
                return;
            }
            
            WallForQ = WallExtension.NearestWall(target, Q.Range);
        }

        public static void CastQ(Obj_AI_Base target)
        {
            var rect = QRect(target);
            if (rect == null || !target.IsValidTarget(Q.Range))
            {
                return;
            }

            Q.Cast(target);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (target.IsValidTarget(W.Range))
            {
                W.Cast(target);
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
         
            if (Q.Ready && !WallForQ.IsZero)
            {
                WallForQ = target.ServerPosition + (target.ServerPosition - WallForQ).Normalized() * 100;

                if (WallForQ.Distance(Global.Player) <= E.Range)
                {
                    E.Cast(WallForQ);
                    return;
                }
            }

            E.Cast(DashManager.DashKite(target, E.Range));
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (target.IsValidTarget(R.Range))
            {
                R.Cast(target);
            }
        }

        public static Geometry.Rectangle QRect(Obj_AI_Base target)
        {
            return new Geometry.Rectangle(Global.Player.ServerPosition.To2D(),
                (Global.Player.ServerPosition + target.ServerPosition).To2D().Normalized() * Q.Range,
                Q.Width);
        }

        public static Geometry.Rectangle RRect(Obj_AI_Base target)
        {
            return new Geometry.Rectangle(Global.Player.ServerPosition.To2D(),
                (Global.Player.ServerPosition + target.ServerPosition).To2D().Normalized() * R.Range,
                R.Width);
        }
    }
}