using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Builders
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
            [NotNull] QualifiedName functionName,
            [NotNull] StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    var name = functionName.Qualify(variableDeclaration.Name.Value ?? "_");
                    var type = variableDeclaration.TypeExpression != null ? expressionBuilder.BuildExpression(context, functionName, variableDeclaration.TypeExpression) : null;
                    var initializer = variableDeclaration.Initializer != null ? expressionBuilder.BuildExpression(context, functionName, variableDeclaration.Initializer) : null;
                    return new VariableDeclarationStatementAnalysis(context, variableDeclaration, name, type, initializer);
                case ExpressionStatementSyntax expressionStatement:
                    return new ExpressionStatementAnalysis(context, expressionStatement,
                        expressionBuilder.BuildExpression(context, functionName, expressionStatement.Expression));
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }
    }
}
