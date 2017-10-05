﻿using System.Threading;
using Adept_AIO.Champions.Riven.Core;
using Adept_AIO.SDK.Unit_Extensions;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace Adept_AIO.Champions.Riven.Update.Miscellaneous
{
    internal class Animation
    {
        public static float LastReset;
        public static bool DidRecentlyCancel;

        public static void Reset()
        {
            Global.Orbwalker.AttackingEnabled = false;

            LastReset = Game.TickCount;
            DidRecentlyCancel = true;
        }

        public static float GetDelay()
        {
            return (Extensions.CurrentQCount == 1 ? 420 : 310) - 3.333f * Global.Player.Level; 
        }

        public static void OnPlayAnimation(Obj_AI_Base sender, Obj_AI_BasePlayAnimationEventArgs args)
        {
            if (sender == null || !sender.IsMe)
            {
                return;
            }

            switch (args.Animation)
            {
                case "Spell1a":
                    Extensions.CurrentQCount = 2; // Q1
                    break;
                case "Spell1b":
                    Extensions.CurrentQCount = 3; // Q2
                    break;
                case "Spell1c":
                    Extensions.CurrentQCount = 1; // Q3 
                    break;
            }
        }
    }
}
