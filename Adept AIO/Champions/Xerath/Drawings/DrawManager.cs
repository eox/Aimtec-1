namespace Adept_AIO.Champions.Xerath.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;

    class DrawManager
    {
        public static void OnPresent()
        {
            if (Global.Player.IsDead ||
                MenuConfig.Drawings["Dmg"].Value == 0)
            {
                return;
            }

            if (SpellManager.R.Ready && !SpellManager.CastingUltimate && MenuConfig.Drawings["Minimap"].Enabled)
            {
                Geometry.DrawCircleOnMinimap(Global.Player.ServerPosition, SpellManager.R.Range, Color.DeepPink, 5);
            }

            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsVisible && !x.IsDead))
            {
             
                var damage = Dmg.Damage(target);

                Global.DamageIndicator.Unit = target;
                Global.DamageIndicator.DrawDmg((float) damage, Color.FromArgb(153, 12, 177, 28));
            }
        }

        public static void OnRender()
        {
            if (Global.Player.IsDead || !SpellManager.Q.Ready)
            {
                return;
            }

            if (MenuConfig.Drawings["Q"].Enabled)
            {
                Render.Circle(Global.Player.Position, SpellManager.Q.Range, 100, Color.Violet);
            }
          
            if (!MenuConfig.Drawings["Pred"].Enabled)
            {
                return;
            }

            var target = Global.TargetSelector.GetTarget(SpellManager.Q.Range + 200);
            if (target == null)
            {
                return;
            }

            SpellManager.QRealRect(target)?.Draw(Color.LimeGreen);
        }
    }
}