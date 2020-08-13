using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Moves
{
    /// <summary>
    /// Uses a data flow analysis of variables that may have their value moved
    /// out of them to check for use of possibly moved value.
    ///
    /// The variable flags used by this checker indicate that a variable may have
    /// its value moved. Variables not yet declared or assigned vacuously haven't
    /// been moved from.
    /// </summary>
    public class UseOfMovedValueAnalysis : IForwardDataFlowAnalysis<VariableFlags>
    {
        private readonly IConcreteInvocableDeclarationSyntax invocable;
        private readonly ISymbolTree symbolTree;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public UseOfMovedValueAnalysis(IConcreteInvocableDeclarationSyntax invocable, ISymbolTree symbolTree, Diagnostics diagnostics)
        {
            this.invocable = invocable;
            this.symbolTree = symbolTree;
            file = invocable.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start without possibly having their values moved out of them
            return new VariableFlags(invocable, symbolTree, false);
        }

        public VariableFlags Assignment(IAssignmentExpressionSyntax assignmentExpression, VariableFlags possiblyMoved)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case INameExpressionSyntax identifierName:
                    // We are assigning into this variable so it definitely has a value now
                    var symbol = identifierName.ReferencedSymbol.Result ?? throw new InvalidOperationException();
                    return possiblyMoved.Set(symbol, false);
                case IFieldAccessExpressionSyntax _:
                    return possiblyMoved;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(INameExpressionSyntax nameExpression, VariableFlags possiblyMoved)
        {
            var symbol = nameExpression.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            if (possiblyMoved[symbol] == true)
                diagnostics.Add(SemanticError.UseOfPossiblyMovedValue(file, nameExpression.Span));

            var valueSemantics = nameExpression.Semantics;
            // TODO this isn't correct, but for now fields don't have proper move, borrow handling
            //?? nameExpression.Type.Assigned().OldValueSemantics;
            switch (valueSemantics)
            {
                case ExpressionSemantics.Move:
                case ExpressionSemantics.Acquire:
                    return possiblyMoved.Set(symbol, true);
                case ExpressionSemantics.Copy:
                case ExpressionSemantics.Borrow:
                case ExpressionSemantics.Share:
                case ExpressionSemantics.Void:
                case ExpressionSemantics.Never:
                case ExpressionSemantics.CreateReference:
                case null: // If it were move or copy, that would have been set to the ExpressionSemantics
                    // Not moving value
                    return possiblyMoved;
                default:
                    throw ExhaustiveMatch.Failed(valueSemantics);
            }
        }

        public VariableFlags VariableDeclaration(
            IVariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags possiblyMoved)
        {
            // No affect on state since it should already be false
            return possiblyMoved;
        }

        public VariableFlags VariableDeclaration(
            IForeachExpressionSyntax foreachExpression,
            VariableFlags possiblyMoved)
        {
            // No affect on state since it should already be false
            return possiblyMoved;
        }
    }
}
