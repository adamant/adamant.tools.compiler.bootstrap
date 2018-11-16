using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders
{
    public class StatementAnalysisBuilder : IStatementAnalysisBuilder
    {
        [NotNull] private readonly IExpressionAnalysisBuilder expressionBuilder;

        public StatementAnalysisBuilder([NotNull] IExpressionAnalysisBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        [NotNull]
        public StatementAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    var name = functionName.Qualify((SimpleName)variableDeclaration.Name.Value);
                    var type = variableDeclaration.TypeExpression != null ? expressionBuilder.BuildExpression(context, functionName, variableDeclaration.TypeExpression) : null;
                    var initializer = variableDeclaration.Initializer != null ? expressionBuilder.BuildExpression(context, functionName, variableDeclaration.Initializer) : null;
                    return new VariableDeclarationStatementAnalysis(context, variableDeclaration, name, type, initializer);
                case ExpressionSyntax expressionStatement:
                    return new ExpressionStatementAnalysis(context, expressionStatement,
                        expressionBuilder.BuildExpression(context, functionName, expressionStatement));
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }
    }
}
