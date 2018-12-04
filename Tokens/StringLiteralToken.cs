using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class StringLiteralToken : Token, IStringLiteralToken
    {
        [NotNull] public string Value { get; }

        public StringLiteralToken(TextSpan span, [NotNull] string value)
            : base(span)
        {
            Value = value;
        }

        // Helpful for debugging
        public override string ToString()
        {
            return '"' + Value.Escape() + '"';
        }
    }

    public static partial class TokenFactory
    {
        [NotNull]
        public static IStringLiteralToken StringLiteral(TextSpan span, [NotNull] string value)
        {
            return new StringLiteralToken(span, value);
        }
    }
}
