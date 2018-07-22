using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class StringExtensions
    {
        public static string Repeat(this string input, int count)
        {
            if (string.IsNullOrEmpty(input) || count == 0)
                return string.Empty;

            return new StringBuilder(input.Length * count)
                .Insert(0, input, count)
                .ToString();
        }
    }
}
