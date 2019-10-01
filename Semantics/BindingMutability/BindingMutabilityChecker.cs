using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    /// <summary>
    /// Uses a data flow analysis of variables that are definitely unassigned to
    /// determine if binding mutability is violated.
    /// </summary>
    public class BindingMutabilityChecker : IDataFlowAnalysisChecker<VariableFlags>
    {
        private readonly IMethodDeclarationSyntax method;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BindingMutabilityChecker(IMethodDeclarationSyntax method, Diagnostics diagnostics)
        {
            this.method = method;
            file = method.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start definitely unassigned
            var definitelyUnassigned = new VariableFlags(method, true);
            // All parameters are assigned
            definitelyUnassigned = definitelyUnassigned.Set(method.Parameters, false);
            return definitelyUnassigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyUnassigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameSyntax identifier:
                    var symbol = identifier.ReferencedSymbol;
                    //if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                    //{
                    //    diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.Name));
                    //}
                    throw new NotImplementedException();
                    return definitelyUnassigned.Set(symbol, false);
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    return definitelyUnassigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(INameSyntax name, VariableFlags definitelyUnassigned)
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
