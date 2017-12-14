namespace Adept_AIO.Champions.Gnar.Core
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using SDK.Usables;
    using Spell = Aimtec.SDK.Spell;

    enum GnarState
    {
        Small,
        Mega
    }

    class SpellManager
    {
        public static Spell Q, W, E, R;

        private static int lastCheckTick;

        public static GnarState GnarState;

        public SpellManager()
        {
            R = new Spell(SpellSlot.R, 475);
            W = new Spell(SpellSlot.W);

            Game.OnUpdate += OnUpdate;
        }

        public static void OnUpdate()
        {
            if (Global.Player.HasBuff("gnartransform") || Global.Player.HasBuff("gnartransformsoon"))
            {
                GnarState = GnarState.Mega;
            }
            else if (Global.Player.HasBuff("gnartransformtired"))
            {
                GnarState = GnarState.Small;
            }

            if (Game.TickCount - lastCheckTick <= 1000)
            {
                return;
            }

            if (GnarState == GnarState.Small)
            {
                Q = new Spell(SpellSlot.Q, 1100);
                Q.SetSkillshot(0.25f, 60, 1400, true, SkillshotType.Line);

                E = new Spell(SpellSlot.E, 475);
                E.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.Circle);
                lastCheckTick = Game.TickCount;
            }
            else
            {
                Q = new Spell(SpellSlot.Q, 1100);
                Q.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.Line);

                W = new Spell(SpellSlot.W, 500);
                W.SetSkillshot(0.25f, 80, 1200, false, SkillshotType.Line, false, HitChance.VeryHigh);

                E = new Spell(SpellSlot.E, 475);
                E.SetSkillshot(0.6f, 60, 1500, false, SkillshotType.Circle);
                lastCheckTick = Game.TickCount;
            }
        }

        public static void CastQ(Obj_AI_Base target, int hitCount = 1)
        {
            Q.Cast(target, true, hitCount);
        }

        public static void CastW(Obj_AI_Base target, int hitCount = 1)
        {
            if (GnarState == GnarState.Small)
            {
                return;
            }

            W.Cast(target, true, hitCount);
        }

        public static void CastE(Obj_AI_Base target)
        {
            E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target, int count = 1)
        {
            var wall = WallExtension.NearestWall(target, 590);

            if (wall.IsZero || count != 1 && wall.CountEnemyHeroesInRange(590) < count)
            {
                return;
            }

            var flashPos = wall + (wall - Global.Player.ServerPosition).Normalized() * SummonerSpells.Flash.Range;

            if (SummonerSpells.IsValid(SummonerSpells.Flash) && MenuConfig.Combo["Flash"].Enabled && target.Distance(Global.Player) > 475 && wall.Distance(Global.Player) < 475 + 425)
            {
                SummonerSpells.Flash.Cast(flashPos);
            }

            if (wall.Distance(Global.Player) <= 590)
            {
                R.Cast(wall);
            }
        }
    }
}