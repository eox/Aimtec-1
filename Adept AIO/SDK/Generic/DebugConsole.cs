namespace Adept_AIO.SDK.Generic
{
    using System;

    class DebugConsole
    {
        private static MessageState _messageState;
        private static float _lastTick;
        private static string _lastMessage;

        public static void WriteLine(string message, MessageState messageState, bool onlyOnce = true)
        {
            if (onlyOnce && message == _lastMessage && Environment.TickCount - _lastTick <= 5000)
            {
                return;
            }

            _messageState = messageState;

            Console.ForegroundColor = GetForeGroundColor();
            Console.WriteLine($"[{DateTime.Now}] [ADEPT AIO] [{messageState}] {message}");
            Console.ResetColor();

            _lastTick = Environment.TickCount;
            _lastMessage = message;
        }

        public static void Write(string message, MessageState messageState)
        {
            _messageState = messageState;

            Console.ForegroundColor = GetForeGroundColor();
            Console.Write($"[{DateTime.Now}] [ADEPT AIO] [{messageState}] {message}");
            Console.ResetColor();
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