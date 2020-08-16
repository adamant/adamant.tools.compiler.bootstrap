using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public interface IForwardDataFlowAnalysis<TState>
    {
        TState StartState();
        TState Assignment(IAssignmentExpression assignmentExpression, TState state);
        TState IdentifierName(INameExpression nameExpression, TState state);
        TState VariableDeclaration(IVariableDeclarationStatement variableDeclaration, TState state);
        TState VariableDeclaration(IForeachExpression foreachExpression, TState state);
    }
}
