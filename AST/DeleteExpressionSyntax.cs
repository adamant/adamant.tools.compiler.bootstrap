using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [VisitorNotSupported("Only implemented in parser")]
    public sealed class DeleteExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Expression { get; }

        public DeleteExpressionSyntax(TextSpan span, ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"delete {Expression}";
        }
    }
}
