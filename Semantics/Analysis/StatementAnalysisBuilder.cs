using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
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
                    var type = variableDeclaration.TypeExpression != null ? expressionBuilder.Build(context, functionName, variableDeclaration.TypeExpression) : null;
                    var initializer = variableDeclaration.Initializer != null ? expressionBuilder.Build(context, functionName, variableDeclaration.Initializer) : null;
                    return new VariableDeclarationStatementAnalysis(context, variableDeclaration, name, type, initializer);
                case ExpressionStatementSyntax expressionStatement:
                    // TODO that isn't the right scope I don't think
                    return new ExpressionStatementAnalysis(context, expressionStatement, expressionBuilder.Build(context, functionName, expressionStatement.Expression));
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }
    }
}
