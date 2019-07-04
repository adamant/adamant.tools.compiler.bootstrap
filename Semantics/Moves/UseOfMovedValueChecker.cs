using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Moves
{
    /// <summary>
    /// Uses a data flow analysis of variables that may have their value moved
    /// out of them to check for use of possibly moved value.
    ///
    /// The variable flags used by this checker indicate that a variable may have
    /// its value moved. Variables not yet declared or assigned vacuously haven't
    /// been moved from.
    /// </summary>
    public class UseOfMovedValueChecker : IDataFlowAnalysisChecker<VariableFlags>
    {
        private readonly FunctionDeclarationSyntax function;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        public UseOfMovedValueChecker(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            this.function = function;
            file = function.File;
            this.diagnostics = diagnostics;
        }

        public VariableFlags StartState()
        {
            // All variables start without possibly having their values moved out of them
            return new VariableFlags(function, false);
        }

        public VariableFlags Assignment(AssignmentExpressionSyntax assignmentExpression, VariableFlags possiblyMoved)
        {
            switch (assignmentExpression.LeftOperand)
            {
                case IdentifierNameSyntax identifierName:
                    // We are assigning into this variable so it definitely has a value now
                    var symbol = identifierName.ReferencedSymbol;
                    return possiblyMoved.Set(symbol, false);
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return possiblyMoved;
                default:
                    throw new NotImplementedException("Complex assignments not yet implemented");
            }
        }

        public VariableFlags IdentifierName(IdentifierNameSyntax identifierName, VariableFlags possiblyMoved)
        {
            if (possiblyMoved[identifierName.ReferencedSymbol] == true)
                diagnostics.Add(SemanticError.UseOfPossiblyMovedValue(file, identifierName.Span));

            switch (identifierName.Type.ValueSemantics)
            {
                case ValueSemantics.Move:
                case ValueSemantics.Own:
                    return possiblyMoved.Set(identifierName.ReferencedSymbol, true);
                case ValueSemantics.Copy:
                case ValueSemantics.Borrow:
                case ValueSemantics.Alias:
                case ValueSemantics.Empty:
                    // Not moving value
                    return possiblyMoved;
                default:
                    throw NonExhaustiveMatchException.ForEnum(identifierName.Type.ValueSemantics);
            }
        }

        public VariableFlags VariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration,
            VariableFlags possiblyMoved)
        {
            // No affect on state since it should already be false
            return possiblyMoved;
        }
    }
}
