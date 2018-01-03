namespace Adept_AIO.BOT.SR.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aimtec;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Menu.Config;
    using Data;
    using Menu;
    using SDK.Delegates;
    using SDK.Orbwalking;
    using SDK.Unit_Extensions;
    using Utilities;

    class UpdateManager
    {
        public UpdateManager()
        {
            MenuConfig.Mainmenu["Mode"].OnValueChanged += OnIsDisabledValueChanged;

            movementManager = new MovementManager();
            modeManager = new ModeManager();

            KeyManager.Load();
            KeyManager.ClearKeys();

            new BotData();

            new Camps();

            if (MenuConfig.Mainmenu["Mode"].Enabled)
            {
                Game.OnUpdate += OnUpdate;
                Game.OnEnd += OnEnd;
                Global.Orbwalker.PreMove += OnPreMove;
                Teleport.OnTeleport += OnTeleport;
            }
        }

        private void OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender.IsMe && args.Type == TeleportType.Recall && args.Status == TeleportStatus.Finish)
            {
                BotData.LastRecallAttempt = Environment.TickCount;
            }
        }

        private void OnEnd(GameObjectTeam team)
        {
            KeyManager.ClearKeys();
        }

        private void OnPreMove(object sender, PreMoveEventArgs args)
        {
            if (Global.Player.IsRecalling() || Environment.TickCount - BotData.LastRecallAttempt <= 1500)
            {
                args.Cancel = true;
                return;
            }

            if (!BotData.MovePosition.IsZero)
            {
                args.MovePosition = BotData.MovePosition;
            }
        }

        private void OnIsDisabledValueChanged(MenuComponent sender, ValueChangedArgs args)
        {
            if (args.GetNewValue<MenuBool>().Enabled)
            {
                Console.WriteLine("SUBSCRIBED");
                Game.OnUpdate += OnUpdate;
                Global.Orbwalker.PreMove += OnPreMove;
            }
            else
            {
                Console.WriteLine("DE-SUBSCRIBED");
                Game.OnUpdate -= OnUpdate;
                Global.Orbwalker.PreMove -= OnPreMove;

                KeyManager.ClearKeys();
            }
        }

        private void OnUpdate()
        {
            try
            {
               
                if (Global.Player.IsDead || Global.Player.IsRecalling() || Environment.TickCount - BotData.LastRecallAttempt <= 700)
                {
                    KeyManager.ClearKeys();
                    return;
                }

                movementManager.Update();
                modeManager.Update();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private readonly ModeManager modeManager;

        private readonly MovementManager movementManager;
    }
}