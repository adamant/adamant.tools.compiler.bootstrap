using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Condition { get; }
        [NotNull] public ExpressionBlockSyntax ThenBlock { get; }
        [CanBeNull] public ExpressionSyntax ElseClause { get; }

        public IfExpressionSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax condition,
            [NotNull] ExpressionBlockSyntax thenBlock,
            [CanBeNull] ExpressionSyntax elseClause)
            : base(span)
        {
            Condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }

        public override string ToString()
        {
            if (ElseClause != null)
                return $"if {Condition} {ThenBlock} else {ElseClause}";
            return $"if {Condition} {ThenBlock}";
        }
    }
}
