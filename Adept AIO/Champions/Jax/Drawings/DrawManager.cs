namespace Adept_AIO.Champions.Jax.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Core;
    using SDK.Unit_Extensions;

    class DrawManager
    {
        public static void OnPresent()
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

            if (MenuConfig.Drawings["E"].Enabled && SpellConfig.E.LastCastAttemptT > 0 && Game.TickCount - SpellConfig.E.LastCastAttemptT < 2000)
            {
                Render.WorldToScreen(Global.Player.Position, out var screen);
                Render.Text("Time Until Q: " + (Game.TickCount - SpellConfig.E.LastCastAttemptT) + " / 2000",
                    new Vector2(screen.X - 55, screen.Y + 40),
                    RenderTextFlags.Center,
                    Color.Cyan);
            }

            if (MenuConfig.Drawings["Q"].Enabled && SpellConfig.Q.Ready)
            {
                Render.Circle(Global.Player.Position, SpellConfig.Q.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.Cyan);
            }
        }
    }
}