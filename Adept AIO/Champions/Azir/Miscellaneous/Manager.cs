namespace Adept_AIO.Champions.Azir.Miscellaneous
{
    using System;
    using System.Linq;
    using System.Threading;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;
    using Core;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;
    using OrbwalkingMode = SDK.Orbwalking.OrbwalkingMode;

    class Manager
    {
        private static int lastAa;

        public static void OnUpdate()
        {
            try
            {
                if (Global.Player.IsDead || Global.Orbwalker.IsWindingUp || Global.Player.IsRecalling())
                {
                    return;
                }

                if (Global.Orbwalker.Mode == OrbwalkingMode.Laneclear || Global.Orbwalker.Mode == OrbwalkingMode.Lasthit)
                {
                    foreach (var soldier in SoldierManager.Soldiers)
                    {
                        var enemy = GameObjects.Enemy.FirstOrDefault(x => x.Distance(soldier) <= 300 + x.BoundingRadius && !x.IsDead && x.MaxHealth > 10 &&
                                                                          soldier.Distance(Global.Player) <= SpellConfig.Q.Range + 65 &&
                                                                          soldier.Distance(Global.Player) > Global.Player.AttackRange);
                        if (enemy == null || Game.TickCount - lastAa <= 1000)
                        {
                            continue;
                        }

                        lastAa = Game.TickCount;
                        Global.Player.IssueOrder(OrderType.AttackUnit, enemy);
                        DelayAction.Queue(300, () => Global.Player.IssueOrder(OrderType.MoveTo, Game.CursorPos), new CancellationToken(false));
                    }
                }

                SpellConfig.R.Width = 133 * (3 + Global.Player.GetSpell(SpellSlot.R).Level);

                Insec.OnKeyPressed();

                switch (Global.Orbwalker.Mode)
                {
                    case OrbwalkingMode.Combo:
                        Combo.OnUpdate();
                        break;
                    case OrbwalkingMode.Mixed:
                        Harass.OnUpdate();
                        break;
                    case OrbwalkingMode.Laneclear:
                        JungleClear.OnUpdate();
                        LaneClear.OnUpdate();
                        break;
                    case OrbwalkingMode.None:
                        AzirHelper.Rect = null;
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