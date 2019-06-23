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
            var definitelyAssigned = new VariableFlags(function, false);
            // All parameters are assigned
            definitelyAssigned = definitelyAssigned.Set(function.Parameters, true);
            return definitelyAssigned;
        }

        public VariableFlags Assignment(
            AssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyAssigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case IdentifierNameSyntax identifier:
                    return definitelyAssigned.Set(identifier.ReferencedSymbol, true);
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            IdentifierNameSyntax identifierName,
            VariableFlags definitelyAssigned)
        {
            if (definitelyAssigned[identifierName.ReferencedSymbol] == false)
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file,
                    identifierName.Span, identifierName.Name));

            return definitelyAssigned;
        }

        public VariableFlags VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyAssigned)
        {
            if (variableDeclaration.Initializer == null) return definitelyAssigned;
            return definitelyAssigned.Set(variableDeclaration, true);
        }
    }
}
