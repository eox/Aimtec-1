﻿using Adept_AIO.Champions.Azir.Core;
using Aimtec;

namespace Adept_AIO.Champions.Azir.Update.OrbwalkingEvents
{
    class Flee
    {
        public static void OnKeyPressed()
        {
            AzirHelper.Jump(Game.CursorPos);
        }
    }
}
