namespace Adept_AIO.Champions.Azir.Core
{
    using Aimtec;
    using Aimtec.SDK.Orbwalking;
    using SDK.Geometry_Related;
    using OrbwalkerMode = SDK.Orbwalking.OrbwalkerMode;

    class AzirHelper
    {
        public static OrbwalkerMode JumpMode, InsecMode;
        public static Geometry.Rectangle Rect;

        public static int LastR, LastQ, LastW, LastE;

        public static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.SpellSlot)
            {
                case SpellSlot.Q:
                    LastQ = Game.TickCount;
                    break;
                case SpellSlot.W:
                    LastW = Game.TickCount;
                    break;
                case SpellSlot.E:
                    LastE = Game.TickCount;
                    break;
                case SpellSlot.R:
                    LastR = Game.TickCount;
                    break;
            }
        }
    }
}