using System;
using System.Collections;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment
{
    public class DefiniteAssignmentChecker : IDataFlowAnalysisChecker<VariablesAssigned>
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

        public VariablesAssigned StartState()
        {
            var symbolMap = function.ChildSymbols.Values.SelectMany(l => l).Enumerate()
                                .ToFixedDictionary(t => t.Item1, t => t.Item2);
            var assigned = new BitArray(symbolMap.Count);
            // All parameters are assigned
            foreach (var parameter in function.Parameters)
                assigned[symbolMap[parameter]] = true;

            return new VariablesAssigned(symbolMap, assigned);
        }

        public VariablesAssigned Assignment(
            AssignmentExpressionSyntax assignmentExpression,
            VariablesAssigned state)
        {
            var newState = state.Clone();
            switch (assignmentExpression.LeftOperand)
            {
                case IdentifierNameSyntax identifier:
                    newState.Assigned(identifier.ReferencedSymbol);
                    break;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }

            return newState;
        }

        public VariablesAssigned IdentifierName(
            IdentifierNameSyntax identifierName,
            VariablesAssigned state)
        {
            if (state.SymbolMap.TryGetValue(identifierName.ReferencedSymbol, out var i)
                && !state.IsDefinitelyAssigned(i))
            {
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file, identifierName.Span, identifierName.Name));
            }

            return state;
        }

        public VariablesAssigned VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariablesAssigned state)
        {
            if (variableDeclaration.Initializer == null) return state;
            var newState = state.Clone();
            newState.Assigned(variableDeclaration);
            return newState;
        }
    }
}
