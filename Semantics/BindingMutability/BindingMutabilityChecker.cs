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
        private readonly FunctionDeclarationSyntax function;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public BindingMutabilityChecker(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            this.function = function;
            file = function.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start definitely unassigned
            var definitelyUnassigned = new VariableFlags(function, true);
            // All parameters are assigned
            definitelyUnassigned = definitelyUnassigned.Set(function.Parameters, false);
            return definitelyUnassigned;
        }

        public VariableFlags Assignment(
            AssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyUnassigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case NameSyntax identifier:
                    var symbol = identifier.ReferencedSymbol;
                    //if (!symbol.IsMutableBinding && definitelyUnassigned[symbol] == false)
                    //{
                    //    diagnostics.Add(SemanticError.VariableMayAlreadyBeAssigned(file, identifier.Span, identifier.Name));
                    //}
                    throw new NotImplementedException();
                    return definitelyUnassigned.Set(symbol, false);
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return definitelyUnassigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(NameSyntax name, VariableFlags definitelyUnassigned)
        {
            return definitelyUnassigned;
        }

        public VariableFlags VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyUnassigned)
        {
            if (variableDeclaration.Initializer == null)
                return definitelyUnassigned;
            return definitelyUnassigned.Set(variableDeclaration, false);
        }
    }
}
