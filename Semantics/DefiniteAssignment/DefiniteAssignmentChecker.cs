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
        private readonly IConcreteCallableDeclarationSyntax callable;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public DefiniteAssignmentChecker(
            IConcreteCallableDeclarationSyntax callable,
            Diagnostics diagnostics)
        {
            this.callable = callable;
            this.file = callable.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            var definitelyAssigned = new VariableFlags(callable, false);
            // All parameters are assigned
            definitelyAssigned = definitelyAssigned.Set(callable.Parameters, true);
            return definitelyAssigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyAssigned)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpressionSyntax identifier:
                    return definitelyAssigned.Set(identifier.ReferencedSymbol, true);
                case IFieldAccessExpressionSyntax _:
                    return definitelyAssigned;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(
            INameExpressionSyntax nameExpression,
            VariableFlags definitelyAssigned)
        {
            if (definitelyAssigned[nameExpression.ReferencedSymbol] == false)
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file,
                    nameExpression.Span, nameExpression.Name));

            return definitelyAssigned;
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyAssigned)
        {
            if (variableDeclaration.Initializer is null)
                return definitelyAssigned;
            return definitelyAssigned.Set(variableDeclaration, true);
        }
    }
}
