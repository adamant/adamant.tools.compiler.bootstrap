using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class MemberAccessExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new MemberAccessExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public MemberAccessExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberAccessExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
