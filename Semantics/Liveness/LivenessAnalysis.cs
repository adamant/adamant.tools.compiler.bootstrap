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
                    var symbol = identifier.ReferencedSymbol.Assigned();
                    identifier.VariableIsLiveAfter = liveVariables[symbol]
                                             ?? throw new Exception($"No liveness data for variable {symbol}");
                    return liveVariables.Set(symbol, false);
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
            var symbol = nameExpression.ReferencedSymbol.Assigned();
            nameExpression.VariableIsLiveAfter = liveVariables[symbol]
                                         ?? throw new Exception($"No liveness data for variable {symbol.FullName}");
            return liveVariables.Set(symbol, true);
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags liveVariables)
        {
            variableDeclaration.VariableIsLiveAfter = liveVariables[variableDeclaration]
                                              ?? throw new Exception($"No liveness data for variable {variableDeclaration.FullName}");
            return liveVariables.Set(variableDeclaration, false);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags liveVariables)
        {
            foreachExpression.VariableIsLiveAfterAssignment = liveVariables[foreachExpression]
                                            ?? throw new Exception($"No liveness data for variable {foreachExpression.FullName}");
            return liveVariables.Set(foreachExpression, false);
        }
    }
}
