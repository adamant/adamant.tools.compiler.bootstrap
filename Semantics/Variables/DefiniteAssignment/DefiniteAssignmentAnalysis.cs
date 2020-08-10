using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.DefiniteAssignment
{
    // TODO check definite assignment of fields in constructors
    internal class DefiniteAssignmentAnalysis : IForwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IConcreteCallableDeclarationSyntax callable;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public DefiniteAssignmentAnalysis(
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
            return assignmentExpression.LeftOperand switch
            {
                INameExpressionSyntax identifier =>
                    definitelyAssigned.Set(identifier.ReferencedBinding.Assigned(), true),
                IFieldAccessExpressionSyntax _ => definitelyAssigned,
                _ => throw new NotImplementedException("Complex assignments not yet implemented")
            };
        }

        public VariableFlags IdentifierName(
            INameExpressionSyntax nameExpression,
            VariableFlags definitelyAssigned)
        {
            if (definitelyAssigned[nameExpression.ReferencedBinding.Assigned()] == false)
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

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags definitelyAssigned)
        {
            return definitelyAssigned.Set(foreachExpression, true);
        }
    }
}
