using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class StringExtensions
    {
        [NotNull]
        private static readonly Regex LineEndings = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);

        [NotNull]
        public static string Repeat([NotNull] this string input, int count)
        {
            Requires.NotNull(nameof(input), input);

            if (string.IsNullOrEmpty(input) || count == 0)
                return string.Empty;

            return new StringBuilder(input.Length * count)
                .Insert(0, input, count).AssertNotNull()
                .ToString().AssertNotNull();
        }

        public static string NormalizeLineEndings([NotNull] this string input, [NotNull] string lineEnding)
        {
            return LineEndings.Replace(input, lineEnding);
        }
    }
}
