namespace Adept_AIO.Champions.Jax.OrbwalkingEvents
{
    using Aimtec;
    using Core;
    using Miscellaneous;
    using SDK.Unit_Extensions;
    using SDK.Usables;

    class Harass
    {
        public static void OnPostAttack()
        {
            if (!SpellConfig.W.Ready || !MenuConfig.Harass["W"].Enabled)
            {
                return;
            }

            SpellConfig.W.Cast();
            Items.CastTiamat();
            Global.Orbwalker.ResetAutoAttackTimer();
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.Q.Range);
            if (target == null)
            {
                return;
            }
            
            if (SpellConfig.E.Ready && MenuConfig.Harass["E"].Enabled)
            {
                SpellManager.CastE(target);
            }
            else if (SpellConfig.Q.Ready && MenuConfig.Harass["Q"].Enabled && Game.TickCount - SpellConfig.E.LastCastAttemptT >= 1800 || SpellConfig.E.LastCastAttemptT <= 0)
            {
                SpellConfig.Q.CastOnUnit(target);
            }
        }
    }
}