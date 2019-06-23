using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment
{
    // TODO check definite assignment of fields in constructors
    public class DefiniteAssignmentChecker : IDataFlowAnalysisChecker<VariableFlags>
    {
        private readonly FunctionDeclarationSyntax function;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public DefiniteAssignmentChecker(
            FunctionDeclarationSyntax function,
            Diagnostics diagnostics)
        {
            this.function = function;
            this.file = function.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            var state = new VariableFlags(function, false);
            // All parameters are assigned
            state = state.Set(function.Parameters, true);
            return state;
        }

        public VariableFlags Assignment(
            AssignmentExpressionSyntax assignmentExpression,
            VariableFlags state)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case IdentifierNameSyntax identifier:
                    return state.Set(identifier.ReferencedSymbol, true);
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            IdentifierNameSyntax identifierName,
            VariableFlags state)
        {
            if (state.SymbolMap.TryGetValue(identifierName.ReferencedSymbol, out var i)
                && !state[i])
            {
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file, identifierName.Span, identifierName.Name));
            }

            return state;
        }

        public VariableFlags VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags state)
        {
            if (variableDeclaration.Initializer == null) return state;
            return state.Set(variableDeclaration, true);
        }
    }
}
