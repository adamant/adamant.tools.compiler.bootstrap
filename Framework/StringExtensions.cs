using System.Text;
using System.Text.RegularExpressions;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class StringExtensions
    {
        private static readonly Regex LineEndings = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);

        public static string Repeat(this string input, int count)
        {
            if (string.IsNullOrEmpty(input) || count == 0)
                return string.Empty;

            return new StringBuilder(input.Length * count)
                .Insert(0, input, count)
                .ToString();
        }

        public static string NormalizeLineEndings(this string input, string lineEnding)
        {
            return LineEndings.Replace(input, lineEnding);
        }
    }
}
