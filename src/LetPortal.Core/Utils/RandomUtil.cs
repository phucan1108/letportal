using System;

namespace LetPortal.Core.Utils
{
    public class RandomUtil
    {
        private static readonly Random random = new Random();

        public static int NextInt(int minimum = 0, int maximum = 0)
        {
            return random.Next(minimum, maximum);
        }
    }
}