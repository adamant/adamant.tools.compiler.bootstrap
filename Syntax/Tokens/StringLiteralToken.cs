using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class StringLiteralToken : Token
    {
        [NotNull] public readonly string Value;

        public StringLiteralToken(TextSpan span, [NotNull] string value)
            : base(span)
        {
            Requires.NotNull(nameof(value), value);
            Value = value;
        }
    }
}
