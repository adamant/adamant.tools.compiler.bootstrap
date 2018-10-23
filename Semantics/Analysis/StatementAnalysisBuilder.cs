using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class StatementAnalysisBuilder
    {
        [NotNull] private readonly ExpressionAnalysisBuilder expressionBuilder;

        public StatementAnalysisBuilder([NotNull] ExpressionAnalysisBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
            this.expressionBuilder.StatementBuilder = this;
        }

        [NotNull]
        public StatementAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    return new VariableDeclarationStatementAnalysis(context, variableDeclaration);
                case ExpressionStatementSyntax expressionStatement:
                    // TODO that isn't the right scope I don't think
                    return new ExpressionStatementAnalysis(context, expressionStatement, expressionBuilder.Build(context, expressionStatement.Expression));
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }
    }
}
