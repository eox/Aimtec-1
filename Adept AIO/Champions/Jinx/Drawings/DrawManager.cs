namespace Adept_AIO.Champions.Jinx.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Damage;
    using Core;
    using SDK.Unit_Extensions;

    class DrawManager
    {
        private readonly Dmg dmg;
        private readonly MenuConfig menuConfig;
        private readonly SpellConfig spellConfig;

        public DrawManager(MenuConfig menuConfig, Dmg dmg, SpellConfig spellConfig)
        {
            this.menuConfig = menuConfig;
            this.dmg = dmg;
            this.spellConfig = spellConfig;
        }

        public void OnPresent()
        {
            if (Global.Player.IsDead || !menuConfig.Drawings["Dmg"].Enabled)
            {
                return;
            }

            foreach (var target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsFloatingHealthBarActive && x.IsVisible))
            {
                var damage = Global.Player.GetSpellDamage(target, SpellSlot.R);

                Global.DamageIndicator.Unit = target;
                Global.DamageIndicator.DrawDmg((float) damage, Color.FromArgb(153, 12, 177, 28));
            }
        }

        public void OnRender()
        {
            if (Global.Player.IsDead)
            {
                return;
            }

            if (menuConfig.Drawings["R"].Enabled)
            {
                Render.Circle(Global.Player.Position, menuConfig.Killsteal["Range"].Value, (uint) menuConfig.Drawings["Segments"].Value, Color.CadetBlue);
            }

            if (menuConfig.Drawings["W"].Enabled)
            {
                Render.Circle(Global.Player.Position, spellConfig.W.Range, (uint) menuConfig.Drawings["Segments"].Value, Color.Gray);
            }
        }
    }
}