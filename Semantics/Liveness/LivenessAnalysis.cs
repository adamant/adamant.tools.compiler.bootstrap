using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalysis : IBackwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IConcreteCallableDeclarationSyntax callable;

        public LivenessAnalysis(IConcreteCallableDeclarationSyntax callable)
        {
            this.callable = callable;
        }

        public VariableFlags StartState()
        {
            return new VariableFlags(callable, false);
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags liveVariables)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpressionSyntax identifier:
                    identifier.VariablesLiveAfter = liveVariables;
                    return liveVariables.Set(identifier.ReferencedSymbol.Assigned(), false);
                case IFieldAccessExpressionSyntax _:
                    return liveVariables;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            INameExpressionSyntax nameExpression,
            VariableFlags liveVariables)
        {
            nameExpression.VariablesLiveAfter = liveVariables;
            return liveVariables.Set(nameExpression.ReferencedSymbol.Assigned(), true);
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags liveVariables)
        {
            variableDeclaration.VariablesLiveAfter = liveVariables;
            return liveVariables.Set(variableDeclaration, false);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags liveVariables)
        {
            foreachExpression.VariablesLiveAfterVariable = liveVariables;
            return liveVariables.Set(foreachExpression, false);
        }
    }
}
