namespace Adept_AIO.Champions.Jinx.Core
{
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Prediction.Skillshots;
    using SDK.Unit_Extensions;
    using Spell = Aimtec.SDK.Spell;

    class SpellConfig
    {
        public int DefaultAuotAttackRange = 590;
        public Spell Q, W, E, R;

        public bool IsQ2 => Global.Player.HasBuff("JinxQ");
        public int Q2Range => 630 + 25 * Global.Player.GetSpell(SpellSlot.Q).Level;

        public void Load()
        {
            Q = new Spell(SpellSlot.Q);

            W = new Spell(SpellSlot.W, 1500);
            W.SetSkillshot(0.75f, 80, 3300, true, SkillshotType.Line, false, HitChance.VeryHigh);

            E = new Spell(SpellSlot.E, 900);
            E.SetSkillshot(1f, 325, 1750, false, SkillshotType.Circle, false, HitChance.VeryHigh);

            R = new Spell(SpellSlot.R, int.MaxValue);
            R.SetSkillshot(0.5f, 140, 2200, false, SkillshotType.Line, false, HitChance.VeryHigh);
        }
    }
}