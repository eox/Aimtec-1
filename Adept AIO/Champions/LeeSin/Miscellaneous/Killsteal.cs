namespace Adept_AIO.Champions.LeeSin.Miscellaneous
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    interface IKillsteal
    {
        void OnUpdate();
    }

    class Killsteal : IKillsteal
    {
        private readonly ISpellConfig spellConfig;

        public Killsteal(ISpellConfig spellConfig)
        {
            this.spellConfig = spellConfig;
        }

        public bool IgniteEnabled { get; set; }
        public bool SmiteEnabled { get; set; }
        public bool QEnabled { get; set; }
        public bool EEnabled { get; set; }
        public bool REnabled { get; set; }

        public void OnUpdate()
        {
            var target = GameObjects.EnemyHeroes.FirstOrDefault(x => x.Distance(Global.Player) < spellConfig.R.Range && x.HealthPercent() <= 40);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (this.SmiteEnabled && SummonerSpells.IsValid(SummonerSpells.Smite) && target.Health < SummonerSpells.SmiteChampions())
            {
                SummonerSpells.Smite.CastOnUnit(target);
            }
            if (spellConfig.Q.Ready &&
                (spellConfig.IsQ2()
                    ? target.Health < Global.Player.GetSpellDamage(target, SpellSlot.Q, DamageStage.SecondCast)
                    : target.Health < Global.Player.GetSpellDamage(target, SpellSlot.Q)) &&
                target.IsValidTarget(spellConfig.Q.Range) && this.QEnabled)
            {
                spellConfig.Q.Cast(target);
            }
            else if (spellConfig.E.Ready && target.Health < Global.Player.GetSpellDamage(target, SpellSlot.E) && target.IsValidTarget(spellConfig.E.Range) && this.EEnabled)
            {
                spellConfig.E.Cast();
            }
            else if (spellConfig.R.Ready && target.Health < Global.Player.GetSpellDamage(target, SpellSlot.R) && target.IsValidTarget(spellConfig.R.Range) && this.REnabled)
            {
                spellConfig.R.CastOnUnit(target);
            }
            else if (this.IgniteEnabled && SummonerSpells.IsValid(SummonerSpells.Ignite) && target.Health < SummonerSpells.IgniteDamage(target))
            {
                SummonerSpells.Ignite.Cast(target);
            }
        }
    }
}