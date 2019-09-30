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
        private readonly IMethodDeclarationSyntax method;
        private readonly Diagnostics diagnostics;

        private ShadowChecker(IMethodDeclarationSyntax method, Diagnostics diagnostics)
        {
            this.method = method;
            this.diagnostics = diagnostics;
        }

        public static void Check(IEnumerable<ICallableDeclarationSyntax> callableDeclarations, Diagnostics diagnostics)
        {
            foreach (var declaration in callableDeclarations.OfType<IMethodDeclarationSyntax>())
                Check(declaration, diagnostics);
        }

        private static void Check(IMethodDeclarationSyntax method, Diagnostics diagnostics)
        {
            if (method.Body == null)
                return;

            var bindingScope = EmptyBindingScope.Instance;
            foreach (var parameter in method.Parameters)
                bindingScope = new VariableBindingScope(bindingScope, parameter);

            var shadowChecker = new ShadowChecker(method, diagnostics);
            foreach (var statement in method.Body)
                shadowChecker.VisitStatement(statement, bindingScope);
        }

        public override void VisitBlock(BlockSyntax block, BindingScope bindingScope)
        {
            foreach (var statement in block.Statements)
            {
                VisitStatement(statement, bindingScope);
                // Each variable declaration establishes a new binding scope
                if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                    bindingScope = new VariableBindingScope(bindingScope, variableDeclaration);
            }
        }

        public override void VisitVariableDeclarationStatement(IVariableDeclarationStatementSyntax variableDeclaration, BindingScope bindingScope)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, bindingScope);
            if (bindingScope.Lookup(variableDeclaration.Name, out var binding))
            {
                if (binding.MutableBinding)
                {
                    diagnostics.Add(SemanticError.CantRebindMutableBinding(method.File, variableDeclaration.NameSpan));
                    method.MarkErrored();
                }
                else if (variableDeclaration.IsMutableBinding)
                {
                    diagnostics.Add(SemanticError.CantRebindAsMutableBinding(method.File, variableDeclaration.NameSpan));
                    method.MarkErrored();
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
            diagnostics.Add(SemanticError.CantShadow(method.File, shadowedBy.NameSpan, name.Span));
            method.MarkErrored();
        }
    }
}
