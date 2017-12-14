using System;

namespace Adept_BaseUlt.Local_SDK
{
    using Aimtec;

    class DebugConsole
    {
        private static MessageState messageState;
        private static float lastTick;
        private static string lastMessage;

        public static void WriteLine(string message, MessageState messageState, bool onlyOnce = true)
        {
            if (onlyOnce && message == lastMessage && Game.TickCount - lastTick <= 5000)
            {
                return;
            }

            DebugConsole.messageState = messageState;

            Console.ForegroundColor = GetForeGroundColor();
            Console.WriteLine($"[{DateTime.Now}] [BASEULT] [{messageState}] {message}");
            Console.ResetColor();

            lastTick = Game.TickCount;
            lastMessage = message;
        }

        private static ConsoleColor GetForeGroundColor()
        {
            switch (messageState)
            {
                case MessageState.Debug: return ConsoleColor.Cyan;
                case MessageState.Error: return ConsoleColor.Red;
                case MessageState.Warn: return ConsoleColor.Yellow;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    enum MessageState
    {
        Warn,
        Error,
        Debug
    }
}
