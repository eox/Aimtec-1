namespace Adept_AIO.Champions.Irelia.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Core;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class DrawManager
    {
        public static void RenderDamage()
        {
            if (Global.Player.IsDead || !MenuConfig.Drawings["Dmg"].Enabled)
            {
                return;
            }

            foreach (var target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsFloatingHealthBarActive && x.IsVisible))
            {
                var damage = Dmg.Damage(target);

                Global.DamageIndicator.Unit = target;
                Global.DamageIndicator.DrawDmg((float) damage, Color.FromArgb(153, 12, 177, 28));
            }
        }

        public static void OnRender()
        {
            if (Global.Player.IsDead)
            {
                return;
            }

            if (MenuConfig.Drawings["Engage"].Enabled && Global.Orbwalker.Mode != OrbwalkingMode.None)
            {
                switch (MenuConfig.Combo["Mode"].Value)
                {
                    case 1:
                        Render.Circle(Global.Player.Position, MenuConfig.Combo["Range"].Value, (uint) MenuConfig.Drawings["Segments"].Value, Color.White);
                        break;
                    case 0:
                        Render.Circle(Game.CursorPos, MenuConfig.Combo["Range"].Value, (uint) MenuConfig.Drawings["Segments"].Value, Color.White);
                        break;
                }
            }

            if (MenuConfig.Drawings["Q"].Enabled && SpellConfig.Q.Ready)
            {
                Render.Circle(Global.Player.Position, SpellConfig.Q.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.Aqua);
            }

            if (MenuConfig.Drawings["R"].Enabled && SpellConfig.R.Ready)
            {
                Render.Circle(Global.Player.Position, SpellConfig.R.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.IndianRed);
            }
        }
    }
}