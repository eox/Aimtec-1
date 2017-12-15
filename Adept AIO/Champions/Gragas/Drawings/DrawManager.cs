namespace Adept_AIO.Champions.Gragas.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
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

            if (MenuConfig.Drawings["Q"].Enabled && SpellManager.Q.Ready)
            {
                Render.Circle(Global.Player.Position, SpellManager.Q.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.Cyan);
            }

            if (MenuConfig.Drawings["R"].Enabled && SpellManager.R.Ready)
            {
                Render.Circle(Global.Player.Position, SpellManager.R.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.Crimson);
            }

            if (!MenuConfig.Drawings["Debug"].Enabled || !SpellManager.R.Ready || !Gragas.InsecOrbwalkerMode.Active && Global.Orbwalker.Mode != OrbwalkingMode.Combo)
            {
                return;
            }

            var target = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidTarget(2000));
            if (target == null)
            {
                return;
            }

            var insec3D = InsecManager.InsecPosition(target);
            var qPos3D = InsecManager.QInsecPos(target);

            if (!Render.WorldToScreen(qPos3D, out var qPos) || !Render.WorldToScreen(target.ServerPosition, out var targetPos))
            {
                return;
            }

            if (Gragas.InsecOrbwalkerMode.Active)
            {
                Render.Line(targetPos, qPos, Color.Orange);
                Render.Circle(qPos3D, 30, (uint) MenuConfig.Drawings["Segments"].Value, Color.Crimson);
            }

            Render.Circle(insec3D, 30, (uint) MenuConfig.Drawings["Segments"].Value, Color.Crimson);
            Render.Circle(insec3D, 50, (uint) MenuConfig.Drawings["Segments"].Value, Color.Orange);
        }
    }
}