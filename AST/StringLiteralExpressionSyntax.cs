using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public string Value { get; }

        public StringLiteralExpressionSyntax(TextSpan span, [NotNull] string value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"\"{Value.Escape()}\"";
        }
    }
}
