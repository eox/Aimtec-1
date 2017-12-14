namespace Adept_AIO.Champions.MissFortune.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Core;
    using SDK.Unit_Extensions;

    class DrawManager
    {
        public DrawManager()
        {
            Render.OnPresent += OnPresent;
            Render.OnRender += OnRender;
        }

        public static void OnPresent()
        {
            if (Global.Player.IsDead || !MenuConfig.Drawings["Dmg"].Enabled)
            {
                return;
            }

            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsVisible && !x.IsDead))
            {
                var damage = Dmg.Damage(target);

                Global.DamageIndicator.Unit = target;
                Global.DamageIndicator.DrawDmg((float) damage, Color.FromArgb(153, 12, 177, 28));
            }
        }

        private static void OnRender()
        {
            if (Global.Player.IsDead)
            {
                return;
            }

            if (!SpellManager.Q.Ready)
            {
                return;
            }

            if (!MenuConfig.Drawings["Cone"].Enabled)
            {
                return;
            }

            var target = Global.TargetSelector.GetTarget(1000);
            if (target == null)
            {
                return;
            }

            var wM = SpellManager.WalkBehindMinion(target);
            if (!wM.IsZero)
            {
                Render.Circle(wM, 60, 100, Color.Yellow);
            }

            var m = SpellManager.ExtendedTarget(target);
            if (m != null)
            {
                SpellManager.Cone(m).Draw(Color.Yellow, 8);
            }
        }
    }
}