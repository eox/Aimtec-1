namespace Adept_AIO.Champions.LeeSin.OrbwalkingEvents.Harass
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using Ward_Manager;

    class Harass : IHarass
    {
        private readonly ISpellConfig spellConfig;

        private readonly IWardManager wardManager;

        public Harass(IWardManager wardManager, ISpellConfig spellConfig)
        {
            this.wardManager = wardManager;
            this.spellConfig = spellConfig;
        }

        public bool Q1Enabled { get; set; }
        public bool Q2Enabled { get; set; }
        public int Mode { get; set; }
        public bool EEnabled { get; set; }
        public bool E2Enabled { get; set; }

        public void OnPostAttack(AttackableUnit target)
        {
            if (target == null || !target.IsHero)
            {
                return;
            }
            if (spellConfig.E.Ready && this.E2Enabled && !spellConfig.IsFirst(spellConfig.E))
            {
                spellConfig.E.Cast();
            }
            else if (spellConfig.W.Ready && this.Mode == 1)
            {
                spellConfig.W.CastOnUnit(Global.Player);
            }
        }

        public void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(spellConfig.Q.Range);
            if (target == null)
            {
                return;
            }

            if (spellConfig.Q.Ready && this.Q1Enabled)
            {
                if (spellConfig.IsQ2() && this.Q2Enabled || !spellConfig.IsQ2())
                {
                    spellConfig.Q.Cast(target);
                }
            }

            if (spellConfig.E.Ready)
            {
                if (spellConfig.IsFirst(spellConfig.E) && this.EEnabled && target.IsValidTarget(spellConfig.E.Range))
                {
                    spellConfig.E.Cast(target);
                }
            }

            if (spellConfig.W.Ready && spellConfig.IsFirst(spellConfig.W) && !spellConfig.E.Ready && !spellConfig.Q.Ready && this.Mode == 0)
            {
                var turret = GameObjects.AllyTurrets.OrderBy(x => x.Distance(Global.Player)).FirstOrDefault();
                if (turret != null)
                {
                    wardManager.WardJump(turret.ServerPosition, spellConfig.WardRange);
                }
            }
        }
    }
}