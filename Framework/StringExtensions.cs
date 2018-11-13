using System.Globalization;
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
                .Insert(0, input, count).NotNull()
                .ToString().NotNull();
        }

        [NotNull]
        public static string NormalizeLineEndings([NotNull] this string input, [NotNull] string lineEnding)
        {
            return LineEndings.Replace(input, lineEnding);
        }

        [NotNull]
        public static string Escape([NotNull] this string input)
        {
            var escaped = new StringBuilder(input.Length + 2);
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'': escaped.Append(@"\'"); break;
                    case '\"': escaped.Append("\\\""); break;
                    case '\\': escaped.Append(@"\\"); break;
                    case '\0': escaped.Append(@"\0"); break;
                    case '\a': escaped.Append(@"\a"); break;
                    case '\b': escaped.Append(@"\b"); break;
                    case '\f': escaped.Append(@"\f"); break;
                    case '\n': escaped.Append(@"\n"); break;
                    case '\r': escaped.Append(@"\r"); break;
                    case '\t': escaped.Append(@"\t"); break;
                    case '\v': escaped.Append(@"\v"); break;
                    default:
                        if (char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                            escaped.Append(c);
                        else
                        {
                            escaped.Append(@"\u(");
                            escaped.Append(((ushort)c).ToString("x"));
                            escaped.Append(@")");
                        }

                        break;
                }
            }
            return escaped.ToString().NotNull();
        }

        [NotNull]
        public static FixedList<string> SplitOrEmpty([CanBeNull] this string value, params char[] separators)
        {
            if (string.IsNullOrEmpty(value)) return FixedList<string>.Empty;
            return value.Split(separators).NotNull().ToFixedList();
        }
    }
}
