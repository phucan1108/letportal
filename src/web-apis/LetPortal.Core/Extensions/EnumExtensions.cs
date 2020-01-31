using System;

namespace LetPortal.Core.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string enumValue, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(enumValue))
            {
                throw new ArgumentNullException("Enum value can't be null");
            }
            return (T)Enum.Parse(typeof(T), enumValue, ignoreCase);
        }
    }
}
