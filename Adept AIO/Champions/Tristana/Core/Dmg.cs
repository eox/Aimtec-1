namespace Adept_AIO.Champions.Tristana.Core
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
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

            if (spellConfig.E.Ready || target.HasBuff("TristanaECharge"))
            {
                dmg += Global.Player.GetSpellDamage(target, SpellSlot.E) + Global.Player.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff);
            }

            if (spellConfig.R.Ready)
            {
                dmg += Global.Player.GetSpellDamage(target, SpellSlot.R);
            }
            return dmg;
        }
    }
}