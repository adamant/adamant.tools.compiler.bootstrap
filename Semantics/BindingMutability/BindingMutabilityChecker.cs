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
        private readonly IConcreteCallableDeclarationSyntax callable;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BindingMutabilityChecker(IConcreteCallableDeclarationSyntax callable, Diagnostics diagnostics)
        {
            this.callable = callable;
            file = callable.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start definitely unassigned
            var definitelyUnassigned = new VariableFlags(callable, true);
            // All parameters are assigned
            definitelyUnassigned = definitelyUnassigned.Set(callable.Parameters, false);
            return definitelyUnassigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyUnassigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpressionSyntax identifier:
                    var symbol = identifier.ReferencedSymbol ?? throw new InvalidOperationException("Identifier doesn't have referenced symbol");
                    if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                        diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.Name));
                    return definitelyUnassigned.Set(symbol, false);
                case IMemberAccessExpressionSyntax _:
                    return definitelyUnassigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(INameExpressionSyntax nameExpression, VariableFlags definitelyUnassigned)
        {
            return definitelyUnassigned;
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyUnassigned)
        {
            if (variableDeclaration.Initializer == null)
                return definitelyUnassigned;
            return definitelyUnassigned.Set(variableDeclaration, false);
        }
    }
}
