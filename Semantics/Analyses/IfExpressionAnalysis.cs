using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class IfExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new IfExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Condition { get; }
        [NotNull] public ExpressionAnalysis ThenBlock { get; }
        [CanBeNull] public ExpressionAnalysis ElseClause { get; }

        public IfExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] IfExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis condition,
            [NotNull] ExpressionAnalysis thenBlock,
            [CanBeNull] ExpressionAnalysis elseClause)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(condition), condition);
            Requires.NotNull(nameof(thenBlock), thenBlock);
            Syntax = syntax;
            Condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }
    }
}
