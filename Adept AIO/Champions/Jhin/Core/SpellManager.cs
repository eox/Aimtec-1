namespace Adept_AIO.Champions.Jhin.Core
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Unit_Extensions;
    using Spell = Aimtec.SDK.Spell;

    class SpellManager
    {
        public bool IsReloading()
        {
            return Global.Player.HasBuff("JhinPassiveReload");
        }

        public static Spell Q, W, E, R;

        public SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 600f);
         
            W = new Spell(SpellSlot.W, 2500f);
            W.SetSkillshot(0.75f, 40f, float.MaxValue, false, SkillshotType.Line);

            E = new Spell(SpellSlot.E, 750f);
            E.SetSkillshot(1f, 260f, 1000f, false, SkillshotType.Circle);

            R = new Spell(SpellSlot.R, 3500f);
            R.SetSkillshot(0.25f, 80f, 5000f, false, SkillshotType.Line);
        }

        public static void CastQ(Obj_AI_Base target)
        {
            if (!target.IsValidTarget(Q.Range) || Global.Player.HasBuff("jhinpassiveattackbuff"))
            {
                return;
            }

            Q.CastOnUnit(target);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (!target.HasBuff("jhinespotteddebuff") ||
                !target.IsValidTarget(W.Range) ||
                Global.Player.SpellBook.GetSpell(SpellSlot.R).Name == "JhinRShot")
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

            E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target)
        {
            R.Cast(target);
        }
    }
}
