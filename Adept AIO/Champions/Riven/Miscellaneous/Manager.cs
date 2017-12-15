namespace Adept_AIO.Champions.Riven.Miscellaneous
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
        public static void OnProcessAutoAttack(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
          
            try
            {
                if (!sender.IsMe || !args.Sender.IsMe)
                {
                    return;
                }

                DelayAction.Queue(Game.Ping / 2 + (Global.Player.HasBuff("RivenFengShuiEngine") ? 260 : 240), ()=> Action(args), CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void Action(Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (Game.TickCount - Extensions.LastQCastAttempt < 500 + Game.Ping / 2)
            {
                return;
            }
            Extensions.DidJustAuto = true;

            if (MenuConfig.BurstMode.Active)
            {
                Burst.OnProcessAutoAttack();
            }
            else
            {
                switch (Global.Orbwalker.Mode)
                {
                    case OrbwalkingMode.Combo:
                        Combo.OnPostAttack();
                        break;
                    case OrbwalkingMode.Mixed:
                        Harass.OnProcessAutoAttack();
                        break;
                    case OrbwalkingMode.Laneclear:
                        if (args.Target.IsMinion)
                        {
                            Lane.OnProcessAutoAttack();
                            Jungle.OnProcessAutoAttack();
                        }
                        else if ((args.Target as Obj_AI_Base).IsBuilding() && SpellConfig.Q.Ready)
                        {
                            SpellConfig.Q.Cast(Global.Player.ServerPosition.Extend(args.Target.ServerPosition, 100));
                        }
                        break;
                }
            }
        }

        public static void OnUpdate()
        {
            try
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
                        Lane.OnUpdate();
                        Jungle.OnUpdate();
                        break;
                    case OrbwalkingMode.None:
                        Extensions.AllIn = false;
                        break;
                }

                if (!SpellConfig.Q.Ready || Extensions.CurrentQCount == 1 || !MenuConfig.Miscellaneous["Active"].Enabled || Global.Player.HasBuff("Recall") ||
                    Global.Orbwalker.Mode == OrbwalkingMode.Laneclear || Global.Orbwalker.Mode == OrbwalkingMode.Lasthit ||
                    Game.TickCount - Extensions.LastQCastAttempt < 3400 + Game.Ping / 2 ||
                    Game.TickCount - Extensions.LastQCastAttempt > 3700 + Game.Ping / 2)
                {
                    return;
                }

                var path = Global.Player.Path.FirstOrDefault();
                Global.Orbwalker.Move(path);
                SpellConfig.Q.Cast(path); // fuck this shit.
              

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}