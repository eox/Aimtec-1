namespace Adept_AIO.Champions.LeeSin.OrbwalkingEvents.Insec
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Core;
    using Core.Insec_Manager;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using SDK.Usables;
    using Ward_Manager;

    class Insec : IInsec
    {
        private readonly IInsecManager insecManager;
        private readonly ISpellConfig spellConfig;
        private readonly IWardManager wardManager;

        private readonly IWardTracker wardTracker;

        private Obj_AI_Base lastQUnit;

        private bool isBkActive;

        public Insec(IWardTracker wardTracker, IWardManager wardManager, ISpellConfig spellConfig, IInsecManager insecManager)
        {
            this.wardTracker = wardTracker;
            this.wardManager = wardManager;
            this.spellConfig = spellConfig;
            this.insecManager = insecManager;
        }

        public bool FlashEnabled { get; set; }
        public bool Bk { get; set; }
        public bool QLast { get; set; }
        public bool ObjectEnabled { get; set; }

        private bool FlashReady => SummonerSpells.IsValid(SummonerSpells.Flash) && this.FlashEnabled;

        private bool CanWardJump => spellConfig.W.Ready && spellConfig.IsFirst(spellConfig.W) && wardTracker.IsWardReady();

        private static Obj_AI_Hero Target => Global.TargetSelector.GetSelectedTarget();

        private Obj_AI_Base EnemyObject => GameObjects.EnemyMinions.OrderBy(x => x.Health).
            LastOrDefault(x => InsecInRange(x.ServerPosition) && !x.IsDead && x.IsValid && !x.IsTurret && x.NetworkId != Target.NetworkId &&
                               x.Health * 0.9 > Global.Player.GetSpellDamage(x, SpellSlot.Q) && x.MaxHealth > 7 && Global.Player.Distance(x) <= spellConfig.Q.Range &&
                               x.Distance(GetInsecPosition()) < Global.Player.Distance(GetInsecPosition()));

        public bool Enabled { get; set; }

        // R Flash
        public void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!this.Enabled || !this.FlashReady || sender == null || !sender.IsMe || insecManager.InsecKickValue != 1 || this.CanWardJump && !wardTracker.DidJustWard ||
                wardTracker.DidJustWard ||
                Global.Player.Distance(GetInsecPosition()) <= 220 || Target == null || args.SpellSlot != SpellSlot.R || Global.Player.Distance(GetInsecPosition()) <= 80)
            {
                return;
            }

            SummonerSpells.Flash.Cast(GetInsecPosition());
        }

        public void OnKeyPressed()
        {
            if (!this.Enabled || !Target.IsValidTarget() || Global.Player.Level < 6)
            {
                return;
            }

            Temp.IsBubbaKush = this.Bk;

            var dist = GetInsecPosition().Distance(Global.Player);

            if (spellConfig.Q.Ready && !(this.CanWardJump && dist <= spellConfig.WardRange && this.QLast))
            {
                if (spellConfig.IsQ2())
                {
                    spellConfig.Q.Cast();
                }
                else if (!Global.Player.IsDashing())
                {
                    if (Target.IsValidTarget(spellConfig.Q.Range))
                    {
                        lastQUnit = Target;

                        spellConfig.QSmite(Target);
                        spellConfig.Q.Cast(Target);
                    }
                    
                    if (!this.ObjectEnabled || this.EnemyObject == null)
                    {
                        return;
                    }

                    lastQUnit = this.EnemyObject;
                    spellConfig.Q.Cast(this.EnemyObject);
                }
            }

            if (this.CanWardJump && dist <= InsecRange())
            {
                if (dist <= spellConfig.WardRange)
                {
                    wardManager.WardJump(GetInsecPosition(), (int) dist);
                }
                else if(this.FlashReady)
                {
                    if (Game.TickCount - spellConfig.LastQ1CastAttempt <= 900 || lastQUnit != null && spellConfig.IsQ2() && InsecInRange(lastQUnit.ServerPosition))
                    {
                        return;
                    }

                    if (Game.TickCount - spellConfig.Q.LastCastAttemptT <= 1000)
                    {
                        return;
                    }

                    wardManager.WardJump(GetInsecPosition(), spellConfig.WardRange);
                }
            }

            if (spellConfig.R.Ready)
            {
                if (dist <= 125 || this.FlashReady)
                {
                    if (isBkActive)
                    {
                        var enemy = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget(spellConfig.R.Range) && x.NetworkId != Target.NetworkId);
                        if (enemy != null)
                        {
                            spellConfig.R.CastOnUnit(enemy);
                        }
                    }
                    else if (Target.IsValidTarget(spellConfig.R.Range))
                    {
                        spellConfig.R.CastOnUnit(Target);
                    }
                }

                if (insecManager.InsecKickValue == 0 && this.FlashReady && GetInsecPosition().Distance(Global.Player) <= 425 && GetInsecPosition().Distance(Global.Player) > 220 &&
                    (!this.CanWardJump || wardTracker.DidJustWard))
                {
                    if (Global.Player.GetDashInfo().EndPos.Distance(GetInsecPosition()) <= 215 || this.CanWardJump)
                    {
                        return;
                    }

                    SummonerSpells.Flash.Cast(GetInsecPosition());
                    spellConfig.R.CastOnUnit(Target);
                }
            }
        }

        private int InsecRange()
        {
            var temp = 65;

            if (this.FlashReady)
            {
                temp += 425;
            }

            if (this.CanWardJump)
            {
                temp += spellConfig.WardRange;
            }

            return temp;
        }

        private bool InsecInRange(Vector3 source)
        {
            return GetInsecPosition().Distance(source) <= InsecRange();
        }

        private Vector3 GetInsecPosition()
        {
            if (this.Bk && insecManager.BkPosition(Target) != Vector3.Zero)
            {
                isBkActive = true;
                return insecManager.BkPosition(Target);
            }
            isBkActive = false;
            return insecManager.InsecPosition(Target);
        }
    }
}