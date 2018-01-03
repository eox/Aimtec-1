namespace Adept_AIO.Champions.Vayne.Miscellaneous
{
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;
    using PostAttackEventArgs = SDK.Orbwalking.PostAttackEventArgs;
    using PreAttackEventArgs = SDK.Orbwalking.PreAttackEventArgs;

    class Manager
    {
        public static void PostAttack(object sender, PostAttackEventArgs args)
        {
            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo.PostAttack(sender, args);
                    break;
                case OrbwalkingMode.Mixed:
                    Harass.PostAttack(sender, args);
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear.PostAttack(sender, args);
                    JungleClear.PostAttack(sender, args);
                    break;
                case OrbwalkingMode.Lasthit:
                    Lasthit.PostAttack(sender, args);
                    break;
            }
        }

        public static void PreAttack(object sender, PreAttackEventArgs args)
        {
            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo when MenuConfig.Combo["W"].Enabled:
                    var target = GameObjects.EnemyHeroes.FirstOrDefault(x => x.IsValidAutoRange() && x.GetBuffCount("vaynesilvereddebuff") == 2);
                    if (target != null)
                    {
                        args.Target = target;
                    }
                    break;
            }
        }

        public static void OnUpdate()
        {
            if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp)
            {
                return;
            }

            switch (Global.Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo.OnUpdate();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass.OnUpdate();
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear.OnUpdate();
                    JungleClear.OnUpdate();
                    break;
                case OrbwalkingMode.None:
                    SpellManager.DrawingPred = Vector3.Zero;
                    break;
            }
        }
    }
}