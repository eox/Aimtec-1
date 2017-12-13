using System;

namespace Adept_BaseUlt.Local_SDK
{
    using Aimtec;

    class DebugConsole
    {
        private static MessageState _messageState;
        private static float _lastTick;
        private static string _lastMessage;

        public static void WriteLine(string message, MessageState messageState, bool onlyOnce = true)
        {
            if (onlyOnce && message == _lastMessage && Game.TickCount - _lastTick <= 5000)
            {
                return;
            }

            _messageState = messageState;

            Console.ForegroundColor = GetForeGroundColor();
            Console.WriteLine($"[{DateTime.Now}] [BASEULT] [{messageState}] {message}");
            Console.ResetColor();

            _lastTick = Game.TickCount;
            _lastMessage = message;
        }

        private static ConsoleColor GetForeGroundColor()
        {
            switch (_messageState)
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
