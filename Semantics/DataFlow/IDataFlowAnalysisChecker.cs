using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public interface IDataFlowAnalysisChecker<TState>
    {
        TState StartState();
        TState Assignment(AssignmentExpressionSyntax assignmentExpression, TState state);
        TState IdentifierName(IdentifierNameSyntax identifierName, TState state);
        TState VariableDeclaration(VariableDeclarationStatementSyntax variableDeclaration, TState state);
    }
}
