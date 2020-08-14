using System;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalysis : IBackwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IConcreteInvocableDeclarationSyntax invocable;
        private readonly ISymbolTree symbolTree;

        public LivenessAnalysis(IConcreteInvocableDeclarationSyntax invocable, ISymbolTree symbolTree)
        {
            this.invocable = invocable;
            this.symbolTree = symbolTree;
        }

        public VariableFlags StartState()
        {
            return new VariableFlags(invocable, symbolTree, false);
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags liveVariables)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpressionSyntax identifier:
                    var symbol = identifier.ReferencedSymbol.Result ?? throw new InvalidOperationException();
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
            var symbol = nameExpression.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            nameExpression.VariableIsLiveAfter = liveVariables[symbol]
                                         ?? throw new Exception($"No liveness data for variable {symbol}");
            return liveVariables.Set(symbol, true);
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags liveVariables)
        {
            variableDeclaration.VariableIsLiveAfter = liveVariables[variableDeclaration.Symbol.Result]
                                              ?? throw new Exception($"No liveness data for variable {variableDeclaration.FullName}");
            return liveVariables.Set(variableDeclaration.Symbol.Result, false);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags liveVariables)
        {
            foreachExpression.VariableIsLiveAfterAssignment = liveVariables[foreachExpression.Symbol.Result]
                                            ?? throw new Exception($"No liveness data for variable {foreachExpression.Symbol}");
            return liveVariables.Set(foreachExpression.Symbol.Result, false);
        }
    }
}
