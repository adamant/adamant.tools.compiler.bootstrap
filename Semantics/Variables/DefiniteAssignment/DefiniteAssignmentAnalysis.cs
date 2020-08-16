using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.DefiniteAssignment
{
    // TODO check definite assignment of fields in constructors
    internal class DefiniteAssignmentAnalysis : IForwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IExecutableDeclaration declaration;
        private readonly ISymbolTree symbolTree;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public DefiniteAssignmentAnalysis(
            IExecutableDeclaration declaration,
            ISymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            this.declaration = declaration;
            this.symbolTree = symbolTree;
            this.file = declaration.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            var definitelyAssigned = new VariableFlags(declaration, symbolTree, false);
            if (declaration is IInvocableDeclaration invocable)
            {
                // All parameters are assigned
                var namedParameters = invocable.Parameters.OfType<INamedParameter>();
                var parameterSymbols = namedParameters.Select(p => p.Symbol);
                definitelyAssigned = definitelyAssigned.Set(parameterSymbols, true);
            }
            return definitelyAssigned;
        }

        public VariableFlags Assignment(
            IAssignmentExpression assignmentExpression,
            VariableFlags definitelyAssigned)
        {
            return assignmentExpression.LeftOperand switch
            {
                INameExpression identifier =>
                    definitelyAssigned.Set(identifier.ReferencedSymbol, true),
                IFieldAccessExpression _ => definitelyAssigned,
                _ => throw new NotImplementedException("Complex assignments not yet implemented")
            };
        }

        public VariableFlags IdentifierName(
            INameExpression nameExpression,
            VariableFlags definitelyAssigned)
        {
            if (definitelyAssigned[nameExpression.ReferencedSymbol] == false)
                diagnostics.Add(SemanticError.VariableMayNotHaveBeenAssigned(file,
                    nameExpression.Span, nameExpression.ReferencedSymbol.Name));

            return definitelyAssigned;
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatement variableDeclaration,
            VariableFlags definitelyAssigned)
        {
            if (variableDeclaration.Initializer is null)
                return definitelyAssigned;
            return definitelyAssigned.Set(variableDeclaration.Symbol, true);
        }

        public VariableFlags VariableDeclaration(
            IForeachExpression foreachExpression,
            VariableFlags definitelyAssigned)
        {
            return definitelyAssigned.Set(foreachExpression.Symbol, true);
        }
    }
}
