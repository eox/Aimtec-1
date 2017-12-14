namespace Adept_AIO.Champions.Jinx.Core
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using SDK.Unit_Extensions;

    class Dmg
    {
        private readonly SpellConfig spellConfig;

        public Dmg(SpellConfig spellConfig)
        {
            this.spellConfig = spellConfig;
        }

        public double Damage(Obj_AI_Base target)
        {
            if (target == null)
            {
                return 0;
            }

            var dmg = 0d;

            if (Global.Orbwalker.CanAttack())
            {
                dmg += Global.Player.GetAutoAttackDamage(target);
            }

            if (spellConfig.W.Ready)
            {
                dmg += Global.Player.GetSpellDamage(target, SpellSlot.W);
            }

            if (spellConfig.E.Ready)
            {
                dmg += Global.Player.GetSpellDamage(target, SpellSlot.E);
            }

            if (spellConfig.R.Ready)
            {
                dmg += Global.Player.GetSpellDamage(target, SpellSlot.R);
            }
            return dmg;
        }
    }
}