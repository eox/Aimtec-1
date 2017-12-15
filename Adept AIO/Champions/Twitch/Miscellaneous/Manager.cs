namespace Adept_AIO.Champions.Twitch.Miscellaneous
{
    using System.Linq;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Orbwalking;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class Manager
    {
        public static void OnUpdate()
        {
            if (Global.Player.IsDead)
            {
                return;
            }

            if (MenuConfig.Killsteal["E"].Enabled && SpellManager.E.Ready)
            {
                var t = GameObjects.EnemyHeroes.FirstOrDefault(x => x.Health <= Dmg.EDmg(x) && x.IsValidTarget(SpellManager.E.Range));
                if (t != null)
                {
                    SpellManager.CastE(t);
                }
            }

            if (Global.Orbwalker.IsWindingUp)
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
            }
        }
    }
}