using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
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
        private readonly IExecutableDeclaration declaration;
        private readonly ISymbolTree symbolTree;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BindingMutabilityAnalysis(IExecutableDeclaration declaration, ISymbolTree symbolTree, Diagnostics diagnostics)
        {
            this.declaration = declaration;
            this.symbolTree = symbolTree;
            file = declaration.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start definitely unassigned
            var definitelyUnassigned = new VariableFlags(declaration, symbolTree, true);
            if (declaration is IInvocableDeclaration invocable)
            {
                // All parameters are assigned
                var namedParameters = invocable.Parameters.OfType<INamedParameter>();
                var parameterSymbols = namedParameters.Select(p => p.Symbol);
                definitelyUnassigned = definitelyUnassigned.Set(parameterSymbols, false);
            }
            return definitelyUnassigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpression assignmentExpression,
            VariableFlags definitelyUnassigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpression identifier:
                    var symbol = identifier.ReferencedSymbol;
                    if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                        diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.ReferencedSymbol.Name));
                    return definitelyUnassigned.Set(symbol, false);
                case IFieldAccessExpression _:
                    return definitelyUnassigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            INameExpression nameExpression,
            VariableFlags definitelyUnassigned)
        {
            return definitelyUnassigned;
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatement variableDeclaration,
            VariableFlags definitelyUnassigned)
        {
            if (variableDeclaration.Initializer is null)
                return definitelyUnassigned;
            return definitelyUnassigned.Set(variableDeclaration.Symbol, false);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpression foreachExpression,
            VariableFlags definitelyUnassigned)
        {
            return definitelyUnassigned.Set(foreachExpression.Symbol, false);
        }
    }
}
