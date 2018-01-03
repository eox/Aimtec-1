namespace Adept_AIO.SDK.Generic
{
    using System;

    class DebugConsole
    {
        private static MessageState messageState;
     
        private static string lastMessage;

        public static void WriteLine(string message, MessageState messageState, bool onlyOnce = true)
        {
            if (onlyOnce && message == lastMessage)
            {
                return;
            }

            DebugConsole.messageState = messageState;

            Console.ForegroundColor = GetForeGroundColor();
            Console.WriteLine($"[{DateTime.Now}] [ADEPT AIO] [{messageState}] {message}");
            Console.ResetColor();

       
            lastMessage = message;
        }

        public static void Write(string message, MessageState messageState)
        {
            DebugConsole.messageState = messageState;

            Console.ForegroundColor = GetForeGroundColor();
            Console.Write($"[{DateTime.Now}] [ADEPT AIO] [{messageState}] {message}");
            Console.ResetColor();
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