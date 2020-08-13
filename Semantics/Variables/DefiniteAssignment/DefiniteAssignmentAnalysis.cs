using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.DefiniteAssignment
{
    // TODO check definite assignment of fields in constructors
    internal class DefiniteAssignmentAnalysis : IForwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IConcreteInvocableDeclarationSyntax invocable;
        private readonly SymbolTree symbolTree;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public DefiniteAssignmentAnalysis(
            IConcreteInvocableDeclarationSyntax invocable,
            SymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            this.invocable = invocable;
            this.symbolTree = symbolTree;
            this.file = invocable.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            var definitelyAssigned = new VariableFlags(invocable, symbolTree, false);
            // All parameters are assigned
            var namedParameters = invocable.Parameters.OfType<INamedParameterSyntax>();
            var parameterSymbols = namedParameters.Select(p => p.Symbol.Result);
            definitelyAssigned = definitelyAssigned.Set(parameterSymbols, true);
            return definitelyAssigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpressionSyntax assignmentExpression,
            VariableFlags definitelyAssigned)
        {
            return assignmentExpression.LeftOperand switch
            {
                INameExpressionSyntax identifier =>
                    definitelyAssigned.Set(identifier.ReferencedSymbol.Result!, true),
                IFieldAccessExpressionSyntax _ => definitelyAssigned,
                _ => throw new NotImplementedException("Complex assignments not yet implemented")
            };
        }

        public VariableFlags IdentifierName(
            INameExpressionSyntax nameExpression,
            VariableFlags definitelyAssigned)
        {
            if (definitelyAssigned[nameExpression.ReferencedSymbol.Result!] == false)
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file,
                    nameExpression.Span, nameExpression.SimpleName));

            return definitelyAssigned;
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags definitelyAssigned)
        {
            if (variableDeclaration.Initializer is null)
                return definitelyAssigned;
            return definitelyAssigned.Set(variableDeclaration.Symbol.Result, true);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags definitelyAssigned)
        {
            return definitelyAssigned.Set(foreachExpression.Symbol.Result, true);
        }
    }
}
