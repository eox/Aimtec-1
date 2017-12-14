namespace Adept_AIO.Champions.Tristana.OrbwalkingEvents
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Unit_Extensions;

    class JungleClear
    {
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public JungleClear(MenuConfig menuConfig, SpellConfig spellConfig)
        {
            this.menuConfig = menuConfig;
            this.spellConfig = spellConfig;
        }

        public void OnPostAttack(AttackableUnit target)
        {
            if (!spellConfig.Q.Ready || !menuConfig.JungleClear["Q"].Enabled || target == null || menuConfig.JungleClear["Avoid"].Enabled && Global.Player.Level == 1)
            {
                return;
            }

            spellConfig.Q.Cast();
        }

        public void OnUpdate()
        {
            var mob = GameObjects.Jungle.Where(x => x.IsValidTarget(spellConfig.FullRange)).OrderByDescending(x => x.GetJungleType()).FirstOrDefault();

            if (mob == null || menuConfig.JungleClear["Avoid"].Enabled && Global.Player.Level == 1)
            {
                return;
            }

            if (spellConfig.E.Ready && menuConfig.JungleClear["E"].Enabled)
            {
                spellConfig.E.CastOnUnit(mob);
            }
        }
    }
}