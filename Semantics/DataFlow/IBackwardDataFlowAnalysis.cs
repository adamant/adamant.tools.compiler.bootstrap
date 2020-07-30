using Adamant.Tools.Compiler.Bootstrap.CST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public interface IBackwardDataFlowAnalysis<TState>
    {
        TState StartState();
        TState Assignment(IAssignmentExpressionSyntax assignmentExpression, TState state);
        TState IdentifierName(INameExpressionSyntax nameExpression, TState state);
        TState VariableDeclaration(IVariableDeclarationStatementSyntax variableDeclaration, TState state);
        TState VariableDeclaration(IForeachExpressionSyntax foreachExpression, TState state);
    }
}
