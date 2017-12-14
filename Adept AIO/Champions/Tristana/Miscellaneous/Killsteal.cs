namespace Adept_AIO.Champions.Tristana.Miscellaneous
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Killsteal
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public Killsteal(MenuConfig menuConfig, SpellConfig spellConfig)
        {
            this.menuConfig = menuConfig;
            this.spellConfig = spellConfig;
        }

        public void OnUpdate()
        {
            var target = GameObjects.EnemyHeroes.FirstOrDefault(x => x.Distance(Global.Player) < spellConfig.FullRange + 65);
            if (target == null || !target.IsValid || target.IsDead)
            {
                return;
            }

            if (spellConfig.E.Ready && target.Health < Global.Player.GetSpellDamage(target, SpellSlot.E) && menuConfig.Killsteal["E"].Enabled)
            {
                spellConfig.E.CastOnUnit(target);
            }
            else if (spellConfig.R.Ready && menuConfig.Killsteal["R"].Enabled)
            {
                if (target.Health < Global.Player.GetAutoAttackDamage(target) +
                    (Global.Player.GetSpellDamage(target, SpellSlot.R) + (target.HasBuff("TristanaECharge") ? Global.Player.GetSpellDamage(target, SpellSlot.E) : 0)))
                {
                    spellConfig.R.CastOnUnit(target);
                    Global.Orbwalker.ForceTarget(target);
                }
            }
            else if (spellConfig.W.Ready && target.Health < Global.Player.GetSpellDamage(target, SpellSlot.W) && menuConfig.Killsteal["W"].Enabled &&
                     target.ServerPosition.CountEnemyHeroesInRange(1500) <= 2)
            {
                spellConfig.W.Cast(target);
            }
        }
    }
}