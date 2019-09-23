using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing
{
    /// <summary>
    /// Enforces rules disallowing local variable shadowing
    /// </summary>
    internal class ShadowChecker : ExpressionVisitor<BindingScope>
    {
        private readonly FunctionDeclarationSyntax function;
        private readonly Diagnostics diagnostics;

        private ShadowChecker(FunctionDeclarationSyntax function, Diagnostics diagnostics)
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
            if (function.Body == null)
                return;

            var bindingScope = EmptyBindingScope.Instance;
            foreach (var parameter in function.Parameters)
                bindingScope = new VariableBindingScope(bindingScope, parameter);

            var shadowChecker = new ShadowChecker(function, diagnostics);
            foreach (var statement in function.Body)
                shadowChecker.VisitStatement(statement, bindingScope);
        }

        public override void VisitBlock(BlockSyntax block, BindingScope bindingScope)
        {
            foreach (var statement in block.Statements)
            {
                VisitStatement(statement, bindingScope);
                // Each variable declaration establishes a new binding scope
                if (statement is VariableDeclarationStatementSyntax variableDeclaration)
                    bindingScope = new VariableBindingScope(bindingScope, variableDeclaration);
            }
        }

        public override void VisitVariableDeclarationStatement(VariableDeclarationStatementSyntax variableDeclaration, BindingScope bindingScope)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, bindingScope);
            if (bindingScope.Lookup(variableDeclaration.Name, out var binding))
            {
                if (binding.MutableBinding)
                {
                    diagnostics.Add(SemanticError.CantRebindMutableBinding(function.File, variableDeclaration.NameSpan));
                    function.MarkErrored();
                }
                else if (variableDeclaration.IsMutableBinding)
                {
                    diagnostics.Add(SemanticError.CantRebindAsMutableBinding(function.File, variableDeclaration.NameSpan));
                    function.MarkErrored();
                }
            }
        }

        public override void VisitName(NameSyntax name, BindingScope bindingScope)
        {
            if (!bindingScope.Lookup(name.Name, out var binding))
                return;
            var shadowedBy = binding.WasShadowedBy.LastOrDefault();
            if (shadowedBy == null)
                return;
            diagnostics.Add(SemanticError.CantShadow(function.File, shadowedBy.NameSpan, name.Span));
            function.MarkErrored();
        }
    }
}
