namespace Adept_AIO.Champions.Jinx.Miscellaneous
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class Misc
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public Misc(SpellConfig spellConfig, MenuConfig menuConfig)
        {
            this.spellConfig = spellConfig;
            this.menuConfig = menuConfig;
        }

        public void OnUpdate()
        {
            if (spellConfig.E.Ready)
            {
                if (menuConfig.Combo["Teleport"].Enabled)
                {
                    var enemyTeleport = ObjectManager.Get<Obj_AI_Minion>().
                        FirstOrDefault(x => x.IsEnemy && x.Distance(Global.Player) <= spellConfig.E.Range && x.Buffs.Any(y => y.IsActive && y.Name.ToLower().Contains("teleport")));
                    if (enemyTeleport != null)
                    {
                        spellConfig.E.Cast(enemyTeleport.ServerPosition);
                    }
                }
            }

            var target = Global.TargetSelector.GetTarget(menuConfig.Killsteal["Range"].Value);

            if (target == null)
            {
                return;
            }

            if (spellConfig.R.Ready && menuConfig.Killsteal["Range"].Enabled && menuConfig.Whitelist[target.ChampionName].Enabled &&
                (target.Health < Global.Player.GetSpellDamage(target, SpellSlot.R) && target.Distance(Global.Player) > Global.Player.AttackRange || menuConfig.Combo["Semi"].Enabled))
            {
                spellConfig.R.Cast(target);
            }

            if (spellConfig.E.Ready)
            {
                var count = GameObjects.EnemyHeroes.Count(x => x.Distance(target) < spellConfig.E.Range * 3);

                if (menuConfig.Combo["Count"].Enabled && count >= 2 || menuConfig.Combo["Immovable"].Enabled && target.IsHardCc())
                {
                    spellConfig.E.Cast(target);
                }
            }
        }
    }
}