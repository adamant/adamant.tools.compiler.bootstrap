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
        private readonly IMethodDeclarationSyntax method;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public DefiniteAssignmentChecker(
            IMethodDeclarationSyntax method,
            Diagnostics diagnostics)
        {
            this.method = method;
            this.file = method.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            var definitelyAssigned = new VariableFlags(method, false);
            // All parameters are assigned
            definitelyAssigned = definitelyAssigned.Set(method.Parameters, true);
            return definitelyAssigned;
        }

        public VariableFlags Assignment(
            AssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyAssigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case NameSyntax identifier:
                    return definitelyAssigned.Set(identifier.ReferencedSymbol, true);
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return definitelyAssigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            NameSyntax name,
            VariableFlags definitelyAssigned)
        {
            if (definitelyAssigned[name.ReferencedSymbol] == false)
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file,
                    name.Span, name.Name));

            return definitelyAssigned;
        }

        public VariableFlags VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyAssigned)
        {
            if (variableDeclaration.Initializer == null)
                return definitelyAssigned;
            return definitelyAssigned.Set(variableDeclaration, true);
        }
    }
}
