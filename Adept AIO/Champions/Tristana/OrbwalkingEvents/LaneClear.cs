namespace Adept_AIO.Champions.Tristana.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public LaneClear(SpellConfig spellConfig, MenuConfig menuConfig)
        {
            this.spellConfig = spellConfig;
            this.menuConfig = menuConfig;
        }

        public void OnPostAttack()
        {
            if (!spellConfig.Q.Ready || !menuConfig.LaneClear["Q"].Enabled || menuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2500) > 0)
            {
                return;
            }

            var minions = GameObjects.EnemyMinions.Count(x => x.Health > Global.Player.GetAutoAttackDamage(x) && x.IsValid);

            if (minions <= 3)
            {
                return;
            }

            spellConfig.Q.Cast();
        }

        public void OnUpdate()
        {
            if (menuConfig.LaneClear["Check"].Enabled && Global.Player.CountEnemyHeroesInRange(2500) > 0)
            {
                return;
            }

            if (spellConfig.E.Ready)
            {
                var turret = GameObjects.EnemyTurrets.FirstOrDefault(x => x.IsValid && x.Distance(Global.Player) <= spellConfig.FullRange);

                if (menuConfig.LaneClear["Turret"].Enabled && turret != null && turret.HealthPercent() >= 35)
                {
                    spellConfig.E.CastOnUnit(turret);
                }
                else
                {
                    var minions = GameObjects.EnemyMinions.Count(x => x.Health < Global.Player.GetSpellDamage(x, SpellSlot.E) + Global.Player.GetAutoAttackDamage(x) * 5 && x.IsValid);
                    var minion = GameObjects.EnemyMinions.FirstOrDefault(x =>
                        x.Health < Global.Player.GetSpellDamage(x, SpellSlot.E) + Global.Player.GetAutoAttackDamage(x) * 5 && x.IsValid);
                    var cannon = GameObjects.EnemyMinions.FirstOrDefault(x => x.UnitSkinName.ToLower().Contains("cannon") && x.IsValid);

                    if (minions >= menuConfig.LaneClear["E"].Value)
                    {
                        if (cannon != null)
                        {
                            spellConfig.E.CastOnUnit(cannon);
                        }
                        else if (minion != null)
                        {
                            spellConfig.E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
    }
}