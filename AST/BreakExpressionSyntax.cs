using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class BreakExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull] public ExpressionSyntax Value { get; }

        public BreakExpressionSyntax(
            TextSpan span,
            [CanBeNull] ExpressionSyntax value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value != null) return $"break {Value}";
            return "break";
        }
    }
}
