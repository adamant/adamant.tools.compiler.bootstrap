using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements
{
    public class ExpressionStatementAnalysis : StatementAnalysis
    {
        [NotNull] public ExpressionStatementSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public ExpressionStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionStatementSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
            : base(context)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
