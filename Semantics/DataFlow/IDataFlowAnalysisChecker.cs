using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public interface IDataFlowAnalysisChecker<TState>
    {
        TState StartState();
        TState Assignment(IAssignmentExpressionSyntax assignmentExpression, TState state);
        TState IdentifierName(INameSyntax name, TState state);
        TState VariableDeclaration(IVariableDeclarationStatementSyntax variableDeclaration, TState state);
    }
}
