using System;

namespace StringMath
{
    internal static class Extensions
    {
        public static void EnsureNotNull<T>(this T value, string name) where T : class
        {
            if (value == default)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
