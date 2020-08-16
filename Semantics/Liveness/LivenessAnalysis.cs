using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalysis : IBackwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IExecutableDeclaration declaration;
        private readonly ISymbolTree symbolTree;

        public LivenessAnalysis(IExecutableDeclaration declaration, ISymbolTree symbolTree)
        {
            this.declaration = declaration;
            this.symbolTree = symbolTree;
        }

        public VariableFlags StartState()
        {
            return new VariableFlags(declaration, symbolTree, false);
        }

        public VariableFlags Assignment(
            IAssignmentExpression assignmentExpression,
            VariableFlags liveVariables)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpression identifier:
                    var symbol = identifier.ReferencedSymbol;
                    var isLifeAfter = liveVariables[symbol]
                        ?? throw new Exception($"No liveness data for variable {symbol}");
                    identifier.VariableIsLiveAfter.Fulfill(isLifeAfter);
                    return liveVariables.Set(symbol, false);
                case IFieldAccessExpression _:
                    return liveVariables;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            INameExpression nameExpression,
            VariableFlags liveVariables)
        {
            SetLiveness(nameExpression.ReferencedSymbol, nameExpression.VariableIsLiveAfter, liveVariables);
            return liveVariables.Set(nameExpression.ReferencedSymbol, true);
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatement variableDeclaration,
            VariableFlags liveVariables)
        {
            SetLiveness(variableDeclaration.Symbol, variableDeclaration.VariableIsLiveAfter, liveVariables);
            return liveVariables.Set(variableDeclaration.Symbol, false);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpression foreachExpression,
            VariableFlags liveVariables)
        {
            SetLiveness(foreachExpression.Symbol, foreachExpression.VariableIsLiveAfterAssignment, liveVariables);
            return liveVariables.Set(foreachExpression.Symbol, false);
        }

        private static void SetLiveness(
            NamedBindingSymbol symbol,
            Promise<bool> promise,
            VariableFlags liveVariables)
        {
            var isLiveAfter = liveVariables[symbol]
                              ?? throw new Exception($"No liveness data for variable {symbol}");

            promise.Fulfill(isLiveAfter);
        }
    }
}
