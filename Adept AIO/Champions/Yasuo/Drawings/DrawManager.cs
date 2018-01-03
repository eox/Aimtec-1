namespace Adept_AIO.Champions.Yasuo.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Events;
    using Aimtec.SDK.Extensions;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

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

            if (Global.Orbwalker.Mode != OrbwalkingMode.None && MenuConfig.Drawings["Debug"].Enabled)
            {
                var circle = new Geometry.Circle(Global.Player.GetDashInfo().EndPos, 200);
                Render.Circle(circle.Center.To3D(), circle.Radius, 100, Color.Yellow);
                Render.Circle(circle.Center.To3D(), circle.Radius / 2, 100, Color.Crimson);
            }

            if (SpellConfig.R.Ready && MenuConfig.Drawings["R"].Enabled)
            {
                Render.Circle(Global.Player.Position, SpellConfig.R.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.Cyan);
            }

            if (MenuConfig.Drawings["Range"].Enabled && MenuConfig.Combo["Dash"].Value == 0 && Global.Orbwalker.Mode != OrbwalkingMode.None)
            {
                Render.Circle(Game.CursorPos, MenuConfig.Combo["Range"].Value, (uint) MenuConfig.Drawings["Segments"].Value, Color.Yellow);
            }

            if (MenuConfig.Drawings["Debug"].Enabled)
            {
                if (SpellConfig.Q.Ready)
                {
                    var t = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget(2000));
                    if (t != null)
                    {
                        SpellConfig.Q3Rect(t)?.Draw(Color.Crimson);
                    }
                }

                Render.WorldToScreen(Global.Player.Position, out var temp);
                Render.Text("Q Mode: " + Extension.CurrentMode + " | Range: " + SpellConfig.Q.Range, new Vector2(temp.X - 55, temp.Y + 40), RenderTextFlags.Center, Color.Cyan);
            }

            if (SpellConfig.E.Ready && MenuConfig.Drawings["Path"].Enabled)
            {
                if (MinionHelper.ExtendedMinion.IsZero || MinionHelper.ExtendedTarget.IsZero)
                {
                    return;
                }

                Render.WorldToScreen(MinionHelper.ExtendedTarget, out var targetV2);
                Render.WorldToScreen(MinionHelper.ExtendedMinion, out var minionV2);
                Render.WorldToScreen(Global.Player.ServerPosition, out var playerV2);

                Render.Line(playerV2, minionV2, Color.DeepSkyBlue);
                Render.Line(minionV2, targetV2, Color.DeepPink);

                Render.Circle(MinionHelper.ExtendedMinion, 25, 300, Color.Crimson);
                Render.Circle(MinionHelper.ExtendedMinion, 50, 300, Color.Yellow);
            }
        }
    }
}