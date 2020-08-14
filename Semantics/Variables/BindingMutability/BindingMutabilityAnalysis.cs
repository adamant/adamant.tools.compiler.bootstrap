using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.BindingMutability
{
    /// <summary>
    /// Uses a data flow analysis of variables that are definitely unassigned to
    /// determine if binding mutability is violated.
    /// </summary>
    public class BindingMutabilityAnalysis : IForwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IConcreteInvocableDeclarationSyntax invocable;
        private readonly ISymbolTree symbolTree;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BindingMutabilityAnalysis(IConcreteInvocableDeclarationSyntax invocable, ISymbolTree symbolTree, Diagnostics diagnostics)
        {
            this.invocable = invocable;
            this.symbolTree = symbolTree;
            file = invocable.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start definitely unassigned
            var definitelyUnassigned = new VariableFlags(invocable, symbolTree, true);
            // All parameters are assigned
            var namedParameters = invocable.Parameters.OfType<INamedParameterSyntax>();
            var parameterSymbols = namedParameters.Select(p => p.Symbol.Result);
            definitelyUnassigned = definitelyUnassigned.Set(parameterSymbols, false);
            return definitelyUnassigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyUnassigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpressionSyntax identifier:
                    var symbol = identifier.ReferencedSymbol.Result ?? throw new InvalidOperationException("Identifier doesn't have referenced symbol");
                    if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                        diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.Name ?? throw new InvalidOperationException()));
                    return definitelyUnassigned.Set(symbol, false);
                case IFieldAccessExpressionSyntax _:
                    return definitelyUnassigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            INameExpressionSyntax nameExpression,
            VariableFlags definitelyUnassigned)
        {
            return definitelyUnassigned;
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyUnassigned)
        {
            if (variableDeclaration.Initializer is null)
                return definitelyUnassigned;
            return definitelyUnassigned.Set(variableDeclaration.Symbol.Result, false);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags definitelyUnassigned)
        {
            return definitelyUnassigned.Set(foreachExpression.Symbol.Result, false);
        }
    }
}
