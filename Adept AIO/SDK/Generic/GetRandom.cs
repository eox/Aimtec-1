namespace Adept_AIO.SDK.Generic
{
    using System;

    class GetRandom
    {
        private static Random random;

        public GetRandom()
        {
            random = new Random();
        }

        public static int Next(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}