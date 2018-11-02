using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class ExpressionStatementAnalysis : StatementAnalysis
    {
        [NotNull] public new ExpressionStatementSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public ExpressionStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionStatementSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
