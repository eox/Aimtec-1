namespace Adept_AIO.Champions.LeeSin.OrbwalkingEvents.JungleClear
{
    using System;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using Core.Spells;
    using SDK.Unit_Extensions;
    using SDK.Usables;
    using Ward_Manager;

    class JungleClear : IJungleClear
    {
        private readonly Vector3[] positions =
        {
            new Vector3(5740, 56, 10629),
            new Vector3(5808, 54, 10319),
            new Vector3(5384, 57, 11282),
            new Vector3(9076, 53, 4446),
            new Vector3(9058, 53, 4117),
            new Vector3(9687, 56, 3490)
        };

        private readonly string[] smiteAlways =
        {
            "SRU_Dragon_Air",
            "SRU_Dragon_Fire",
            "SRU_Dragon_Earth",
            "SRU_Dragon_Water",
            "SRU_Dragon_Elder",
            "SRU_Baron",
            "SRU_RiftHerald"
        };

        private readonly string[] smiteOptional =
        {
            "Sru_Crab",
            "SRU_Razorbeak",
            "SRU_Krug",
            "SRU_Murkwolf",
            "SRU_Gromp",
            "SRU_Blue",
            "SRU_Red"
        };

        private readonly ISpellConfig spellConfig;

        private readonly IWardManager wardManager;
        private float q2Time;

        public JungleClear(IWardManager wardManager, ISpellConfig spellConfig)
        {
            this.wardManager = wardManager;
            this.spellConfig = spellConfig;
        }

        public bool StealEnabled { get; set; }
        public bool SmiteEnabled { get; set; }
        public bool BlueEnabled { get; set; }
        public bool Q1Enabled { get; set; }
        public bool WEnabled { get; set; }
        public bool EEnabled { get; set; }

        public void OnPostAttack(AttackableUnit mobPre)
        {
            var mob = mobPre as Obj_AI_Minion;

            if (mob == null)
            {
                return;
            }

            var count = GameObjects.Jungle.Count(x => x.Distance(Global.Player) <= spellConfig.Q.Range / 2f);

            if (count <= 1 && mob.Health < Global.Player.GetAutoAttackDamage(mob))
            {
                return;
            }

            if (Global.Player.Level <= 4)
            {
                if (spellConfig.PassiveStack() >= 1)
                {
                    return;
                }

                if (spellConfig.Q.Ready)
                {
                    spellConfig.Q.Cast(mob);
                }

                if (spellConfig.W.Ready && this.WEnabled && !spellConfig.IsQ2())
                {
                    spellConfig.W.CastOnUnit(Global.Player);
                }
                else if (spellConfig.E.Ready && this.EEnabled && !spellConfig.IsQ2())
                {
                    if (spellConfig.IsFirst(spellConfig.E))
                    {
                        if (Items.CanUseTiamat())
                        {
                            Items.CastTiamat(false);
                            DelayAction.Queue(50, () => spellConfig.E.Cast(mob));
                        }
                        else
                        {
                            spellConfig.E.Cast(mob);
                        }
                    }
                    else
                    {
                        spellConfig.E.Cast();
                    }
                }
            }
            else if (Global.Player.Level <= 8)
            {
                if (spellConfig.PassiveStack() >= 1)
                {
                    return;
                }

                if (spellConfig.Q.Ready)
                {
                    spellConfig.Q.Cast(mob);
                }

                if (spellConfig.W.Ready && this.WEnabled)
                {
                    spellConfig.W.CastOnUnit(Global.Player);
                }
                else if (spellConfig.E.Ready && this.EEnabled)
                {
                    if (spellConfig.IsFirst(spellConfig.E))
                    {
                        if (Items.CanUseTiamat())
                        {
                            Items.CastTiamat(false);
                            DelayAction.Queue(50, () => spellConfig.E.Cast(mob));
                        }
                        else
                        {
                            spellConfig.E.Cast(mob);
                        }
                    }
                    else
                    {
                        spellConfig.E.Cast();
                    }
                }
            }
            else
            {
                if (spellConfig.Q.Ready)
                {
                    spellConfig.Q.Cast(mob);
                }
                else
                {
                    if (spellConfig.E.Ready && this.EEnabled)
                    {
                        if (Items.CanUseTiamat())
                        {
                            Items.CastTiamat(false);
                            DelayAction.Queue(50, () => spellConfig.E.Cast(mob));
                        }
                        else
                        {
                            spellConfig.E.Cast(mob);
                        }
                    }

                    if (spellConfig.W.Ready && this.WEnabled)
                    {
                        if (spellConfig.E.Ready && this.EEnabled && !spellConfig.IsFirst(spellConfig.E))
                        {
                            return;
                        }
                        spellConfig.W.CastOnUnit(Global.Player);
                    }
                }
            }
        }

        public void OnUpdate()
        {
            if (!spellConfig.Q.Ready || !this.Q1Enabled)
            {
                return;
            }

            var mob = ObjectManager.Get<Obj_AI_Minion>().
                FirstOrDefault(x => x.Distance(Global.Player) < spellConfig.Q.Range / 2 && x.GetJungleType() != GameObjects.JungleType.Unknown && x.MaxHealth > 7);

            if (mob == null)
            {
                return;
            }

            if (spellConfig.Q.Ready && spellConfig.IsQ2() &&
                (spellConfig.QAboutToEnd || Global.Player.GetSpellDamage(mob, SpellSlot.Q, DamageStage.SecondCast) + Global.Player.GetAutoAttackDamage(mob) > mob.Health))
            {
                spellConfig.Q.CastOnUnit(mob);
            }

            if (!smiteOptional.Contains(mob.UnitSkinName) && !smiteAlways.Contains(mob.UnitSkinName))
            {
                return;
            }

            if (spellConfig.Q.Ready && spellConfig.IsQ2() && mob.Health < Global.Player.GetSpellDamage(mob, SpellSlot.Q, DamageStage.SecondCast))
            {
                Global.Player.SpellBook.CastSpell(SpellSlot.Q);
            }

            if (!spellConfig.IsQ2() && Game.TickCount - spellConfig.LastQ1CastAttempt > 500)
            {
                Global.Player.SpellBook.CastSpell(SpellSlot.Q, mob.Position);
            }
        }

        public void SmiteMob()
        {
            var smiteAbleMob = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Distance(Global.Player) < 1300);

            if (smiteAbleMob != null && (smiteAlways.Contains(smiteAbleMob.UnitSkinName) || smiteOptional.Contains(smiteAbleMob.UnitSkinName)))
            {
                if (smiteAbleMob.Health < StealDamage(smiteAbleMob))
                {
                    if (smiteOptional.Contains(smiteAbleMob.UnitSkinName) && Global.Player.HealthPercent() >= (SummonerSpells.Ammo("Smite") <= 1 ? 40 : 50) ||
                        smiteAbleMob.UnitSkinName.ToLower().Contains("blue") && !this.BlueEnabled)
                    {
                        return;
                    }

                    if (this.SmiteEnabled && SummonerSpells.IsValid(SummonerSpells.Smite))
                    {
                        SummonerSpells.Smite.CastOnUnit(smiteAbleMob);
                    }

                    if (spellConfig.IsQ2() && spellConfig.Q.Ready)
                    {
                        spellConfig.Q.Cast();
                    }
                }
            }

            var mob = GameObjects.JungleLegendary.FirstOrDefault(x => x.Distance(Global.Player) <= 1500);

            if (mob == null || !this.SmiteEnabled)
            {
                return;
            }

            if (q2Time > 0 && Game.TickCount - q2Time <= 1500 && SummonerSpells.IsValid(SummonerSpells.Smite) && StealDamage(mob) > mob.Health)
            {
                if (spellConfig.W.Ready && spellConfig.IsFirst(spellConfig.W) && Global.Player.Distance(mob) <= 500)
                {
                    SummonerSpells.Smite.CastOnUnit(mob);
                    wardManager.WardJump(positions.FirstOrDefault(), (int) mob.Distance(Global.Player));
                }
            }

            if (mob.Position.CountAllyHeroesInRange(700) <= 1 && spellConfig.Q.Ready && spellConfig.IsQ2() && StealDamage(mob) > mob.Health)
            {
                spellConfig.Q.Cast();
                q2Time = Game.TickCount;
            }
        }

        private double StealDamage(Obj_AI_Base mob)
        {
            return SummonerSpells.SmiteMonsters() + (spellConfig.IsQ2() ? Global.Player.GetSpellDamage(mob, SpellSlot.Q, DamageStage.SecondCast) : 0);
        }
    }
}