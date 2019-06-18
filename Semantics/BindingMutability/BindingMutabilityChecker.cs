using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    /// <summary>
    /// Uses a data flow analysis of variables that are definitely unassigned to
    /// determine if binding mutability is violated.
    /// </summary>
    public class BindingMutabilityChecker : IDataFlowAnalysisChecker<VariableFlags>
    {
        private readonly FunctionDeclarationSyntax function;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BindingMutabilityChecker(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            this.function = function;
            this.file = function.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start definitely unassigned
            var state = new VariableFlags(function, true);
            // All parameters are assigned
            state = state.Set(function.Parameters, false);
            return state;
        }

        public VariableFlags Assignment(
            AssignmentExpressionSyntax assignmentExpression,
            VariableFlags state)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case IdentifierNameSyntax identifier:
                    var symbol = identifier.ReferencedSymbol;
                    if (state.SymbolMap.TryGetValue(symbol, out var i)
                        && !state[i]
                        && !symbol.MutableBinding)
                    {
                        diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.Name));
                    }
                    return state.Set(symbol, false);
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(IdentifierNameSyntax identifierName, VariableFlags state)
        {
            return state;
        }

        public VariableFlags VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags state)
        {
            if (variableDeclaration.Initializer == null) return state;
            return state.Set(variableDeclaration, false);
        }
    }
}
