namespace Adept_AIO.Champions.Zed.Drawings
{
    using System.Drawing;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using SDK.Geometry_Related;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class DrawManager
    {
        public static void OnPresent()
        {
            if (Global.Player.IsDead ||
                !MenuConfig.Drawings["Dmg"].Enabled)
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

        public static void OnRender()
        {
            if (Global.Player.IsDead)
            {
                return;
            }

            if (MenuConfig.Drawings["Pred"].Enabled &&
                SpellManager.Q.Ready)
            {
                foreach (var shadow in ShadowManager.Shadows.Where(ShadowManager.IsShadow))
                {
                    if (shadow.Distance(Global.Player) > 1300)
                    {
                        continue;
                    }

                    if ((Global.Player.GetSpell(SpellSlot.W).ToggleState != 0 || Global.Player.GetSpell(SpellSlot.R).ToggleState != 0) && 

                        Render.WorldToScreen(Global.Player.ServerPosition, out var playerVector2) &&
                        Render.WorldToScreen(shadow.ServerPosition,        out var shadowVector2))
                    {
                        Render.Line(playerVector2, shadowVector2, 3, true, Color.White);
                    }

                    var enemy = GameObjects.Enemy.FirstOrDefault(x => x.Distance(shadow) <= SpellManager.Q.Range);
                    if (enemy == null || !enemy.IsValidTarget())
                    {
                        return;
                    }

                    var pred = SpellManager.Q.GetPrediction(enemy, shadow.ServerPosition, shadow.ServerPosition);
                    var extended = shadow.ServerPosition.Extend(pred.CastPosition, SpellManager.Q.Range);
                    var rect = new Geometry.Rectangle(shadow.ServerPosition.To2D(), extended.To2D(), SpellManager.Q.Width);
                    rect.Draw(Color.Crimson);

                    Render.WorldToScreen(shadow.ServerPosition, out var shadow2D);
                    Render.WorldToScreen(extended, out var extended2D);
                    Render.Line(shadow2D, extended2D, Color.MediumVioletRed);
                }
            }

            if (MenuConfig.Drawings["Range"].Enabled)
            {
                Render.Circle(Global.Player.Position,
                    Global.Orbwalker.Mode == OrbwalkingMode.Mixed ? SpellManager.WCastRange + Global.Player.AttackRange : SpellManager.R.Range,
                    (uint) MenuConfig.Drawings["Segments"].Value,
                    Color.Crimson);
            }

            if (SpellManager.Q.Ready &&
                MenuConfig.Drawings["Q"].Enabled)
            {
                Render.Circle(Global.Player.Position, SpellManager.Q.Range, (uint) MenuConfig.Drawings["Segments"].Value, Color.Cyan);
            }
        }
    }
}