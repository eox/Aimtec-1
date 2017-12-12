namespace Adept_AIO.Champions.Jinx.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class LaneClear
    {
        private readonly MenuConfig _menuConfig;
        private readonly SpellConfig _spellConfig;

        public LaneClear(MenuConfig menuConfig, SpellConfig spellConfig)
        {
            _menuConfig = menuConfig;
            _spellConfig = spellConfig;
        }

        public void OnUpdate()
        {
            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(_spellConfig.W.Range));
            if (minion == null)
            {
                return;
            }

            var dist = Global.Player.Distance(minion);

            if (_spellConfig.W.Ready && _menuConfig.LaneClear["W"].Enabled && Global.Player.CountEnemyHeroesInRange(2000) == 0)
            {
                if (dist > 800 && minion.Health < Global.Player.GetSpellDamage(minion, SpellSlot.W) && minion.UnitSkinName.ToLower().Contains("cannon"))
                {
                    _spellConfig.W.Cast(minion);
                }
            }

            if (_spellConfig.Q.Ready && _menuConfig.LaneClear["Q"].Enabled)
            {
                if (!_spellConfig.IsQ2 && dist > _spellConfig.DefaultAuotAttackRange && dist <= _spellConfig.Q2Range &&
                    GameObjects.EnemyMinions.Count(x => x.IsValidTarget(_spellConfig.Q2Range) && x.Health < Global.Player.GetAutoAttackDamage(x)) >= 2 ||
                    _spellConfig.IsQ2 && dist <= _spellConfig.DefaultAuotAttackRange)
                {
                    _spellConfig.Q.Cast();
                }
            }
        }
    }
}