using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    /// <summary>
    /// Enforces that you can't assign into a variable declared with `let`
    /// </summary>
    internal class BindingMutabilityChecker : ExpressionVisitor<bool>
    {
        private readonly FunctionDeclarationSyntax function;
        private readonly Diagnostics diagnostics;

        private BindingMutabilityChecker(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            this.function = function;
            this.diagnostics = diagnostics;
        }

        public static void Check(IEnumerable<MemberDeclarationSyntax> memberDeclarations, Diagnostics diagnostics)
        {
            foreach (var declaration in memberDeclarations.OfType<FunctionDeclarationSyntax>())
                Check(declaration, diagnostics);
        }

        private static void Check(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            if (function.Body == null) return;

            var moveChecker = new BindingMutabilityChecker(function, diagnostics);
            moveChecker.VisitExpression(function.Body, false);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpression, bool lvalue)
        {
            if (lvalue)
                throw new ArgumentException("Assignment can't be an lvalue", nameof(lvalue));
            VisitExpression(assignmentExpression.LeftOperand, true);
            VisitExpression(assignmentExpression.RightOperand, false);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax identifierName, bool lvalue)
        {
            if (!lvalue || identifierName.ReferencedSymbol.MutableBinding) return;
            diagnostics.Add(SemanticError.CantAssignToImmutable(function.File, identifierName.Span));
            function.Poison();
        }
    }
}
