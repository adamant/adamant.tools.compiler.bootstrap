using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class BoolLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        public bool Value { get; }

        public BoolLiteralExpressionSyntax(TextSpan span, bool value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
