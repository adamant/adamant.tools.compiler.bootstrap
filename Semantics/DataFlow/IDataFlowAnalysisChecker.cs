using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public interface IDataFlowAnalysisChecker<TState>
    {
        TState StartState();
        TState Assignment(AssignmentExpressionSyntax assignmentExpression, TState state);
        TState IdentifierName(NameSyntax name, TState state);
        TState VariableDeclaration(IVariableDeclarationStatementSyntax variableDeclaration, TState state);
    }
}
