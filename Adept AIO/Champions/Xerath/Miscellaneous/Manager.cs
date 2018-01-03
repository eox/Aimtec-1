namespace Adept_AIO.Champions.Xerath.Miscellaneous
{
    using System;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;
    using PreAttackEventArgs = SDK.Orbwalking.PreAttackEventArgs;
    using PreMoveEventArgs = SDK.Orbwalking.PreMoveEventArgs;

    class Manager
    {
        public Manager()
        {
            Game.OnUpdate += OnUpdate;
            Global.Orbwalker.PreAttack += PreAttack;
            Global.Orbwalker.PreMove += PreMove;
        }

        private static void PreMove(object sender, PreMoveEventArgs args)
        {
            if (SpellManager.CastingUltimate)
            {
                args.Cancel = true;
            }
        }

        private static void PreAttack(object sender, PreAttackEventArgs args)
        {
            if (SpellManager.CastingUltimate || SpellManager.Q.IsCharging)
            {
                args.Cancel = true;
            }
        }

        private static void OnUpdate()
        {
            try
            {
                if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp || Global.Player.IsRecalling())
                {
                    return;
                }

                SpellManager.R.Range = new float[] { 3520, 4840, 6160 }[Math.Max(0, Global.Player.SpellBook.GetSpell(SpellSlot.R).Level - 1)];

                if (SpellManager.CastingUltimate && MenuConfig.Misc["Mode"].Value == 1)
                {
                    var target = Global.TargetSelector.GetTarget(SpellManager.R.Range);
                    if (target != null)
                    {
                        SpellManager.CastR(target);
                    }
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}