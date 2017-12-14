namespace Adept_AIO.Champions.LeeSin.OrbwalkingEvents.Combo
{
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using SDK.Usables;
    using Ward_Manager;

    class Combo : ICombo
    {
        private readonly ISpellConfig spellConfig;

        private readonly IWardManager wardManager;
        private readonly IWardTracker wardTracker;

        public Combo(IWardManager wardManager, ISpellConfig spellConfig, IWardTracker wardTracker)
        {
            this.wardManager = wardManager;
            this.spellConfig = spellConfig;
            this.wardTracker = wardTracker;
        }

        public bool TurretCheckEnabled { get; set; }
        public bool Q1Enabled { get; set; }
        public bool Q2Enabled { get; set; }
        public bool WEnabled { get; set; }
        public bool WardEnabled { get; set; }
        public bool EEnabled { get; set; }

        public void OnPostAttack(AttackableUnit target)
        {
            if (target == null)
            {
                return;
            }

            if (spellConfig.Q.Ready && !spellConfig.IsQ2() && target.IsValidTarget(spellConfig.Q.Range))
            {
                spellConfig.Q.Cast();
            }

            else if (spellConfig.W.Ready && this.WEnabled)
            {
                spellConfig.W.Cast(Global.Player);
            }
            else if (spellConfig.E.Ready && this.EEnabled)
            {
                if (!spellConfig.IsFirst(spellConfig.E))
                {
                    spellConfig.E.Cast();
                }
            }
        }

        public void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(1600);
            if (!target.IsValidTarget())
            {
                return;
            }

            var distance = target.Distance(Global.Player);

            if (spellConfig.Q.Ready && this.Q1Enabled)
            {
                if (distance > 1300)
                {
                    return;
                }

                if (spellConfig.IsQ2())
                {
                    if (this.TurretCheckEnabled && target.IsUnderEnemyTurret() || !this.Q2Enabled)
                    {
                        return;
                    }

                    if (spellConfig.QAboutToEnd || distance >= Global.Player.AttackRange + 100)
                    {
                        spellConfig.Q.Cast();
                    }
                }
                else if (target.IsValidTarget(spellConfig.Q.Range))
                {
                    spellConfig.QSmite(target);
                    spellConfig.Q.Cast(target);
                }
            }

            if (spellConfig.R.Ready && spellConfig.Q.Ready && this.Q1Enabled && distance <= 550 && target.Health <= Global.Player.GetSpellDamage(target, SpellSlot.R) +
                Global.Player.GetSpellDamage(target, SpellSlot.Q) + Global.Player.GetSpellDamage(target, SpellSlot.Q, DamageStage.SecondCast))
            {
                spellConfig.R.CastOnUnit(target);
                spellConfig.Q.Cast(target);
            }

            if (spellConfig.E.Ready && this.EEnabled && spellConfig.IsFirst(spellConfig.E) && distance <= 350)
            {
                if (Items.CanUseTiamat())
                {
                    Items.CastTiamat(false);
                    DelayAction.Queue(50, () => spellConfig.E.Cast(target));
                }
                else
                {
                    spellConfig.E.Cast(target);
                }
            }

            if (spellConfig.W.Ready && spellConfig.IsFirst(spellConfig.W) && wardTracker.IsWardReady() && this.WEnabled && this.WardEnabled &&
                distance > (spellConfig.Q.Ready ? 1000 : spellConfig.WardRange))
            {
                if (Game.TickCount - spellConfig.Q.LastCastAttemptT <= 3000 || target.Position.CountEnemyHeroesInRange(2000) > 1)
                {
                    return;
                }

                wardManager.WardJump(target.Position, spellConfig.WardRange);
            }
        }
    }
}