using System.Text;

namespace LetPortal.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLine(this StringBuilder stringBuilder, string appendString, int spaceNumber = 0)
        {
            var spaceSb = new StringBuilder();
            for (var i = 0; i < spaceNumber; i++)
            {
                spaceSb.Append("    ");
            }

            stringBuilder.AppendLine(spaceSb.ToString() + appendString);

            return stringBuilder;
        }
    }
}
