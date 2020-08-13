using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing
{
    /// <summary>
    /// Enforces rules disallowing local variable shadowing
    /// </summary>
    internal class ShadowChecker : SyntaxWalker<BindingScope>
    {
        private readonly IConcreteInvocableDeclarationSyntax invocableDeclaration;
        private readonly Diagnostics diagnostics;

        private ShadowChecker(IConcreteInvocableDeclarationSyntax invocableDeclaration, Diagnostics diagnostics)
        {
            this.invocableDeclaration = invocableDeclaration;
            this.diagnostics = diagnostics;
        }

        public static void Check(IEnumerable<IInvocableDeclarationSyntax> invocableDeclarations, Diagnostics diagnostics)
        {
            foreach (var invocableDeclaration in invocableDeclarations.OfType<IConcreteInvocableDeclarationSyntax>())
                new ShadowChecker(invocableDeclaration, diagnostics).Walk(invocableDeclaration, EmptyBindingScope.Instance);
        }

        protected override void WalkNonNull(ISyntax syntax, BindingScope bindingScope)
        {
            switch (syntax)
            {
                case IConcreteInvocableDeclarationSyntax syn:
                    foreach (var parameter in syn.Parameters)
                        bindingScope = new VariableBindingScope(bindingScope, parameter);
                    break;
                case IBodyOrBlockSyntax syn:
                    foreach (var statement in syn.Statements)
                    {
                        WalkNonNull(statement, bindingScope);
                        // Each variable declaration establishes a new binding scope
                        if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                            bindingScope = new VariableBindingScope(bindingScope, variableDeclaration);
                    }
                    return;
                case IVariableDeclarationStatementSyntax syn:
                {
                    WalkChildren(syn, bindingScope);
                    if (bindingScope.Lookup(syn.FullName.UnqualifiedName, out var binding))
                    {
                        if (binding.MutableBinding)
                            diagnostics.Add(SemanticError.CantRebindMutableBinding(invocableDeclaration.File,
                                syn.NameSpan));
                        else if (syn.IsMutableBinding)
                            diagnostics.Add(SemanticError.CantRebindAsMutableBinding(invocableDeclaration.File,
                                syn.NameSpan));
                    }
                    return;
                }
                case INameExpressionSyntax syn:
                {
                    // This checks for cases where a variable was shadowed, but then used later
                    if (!bindingScope.Lookup(syn.SimpleName, out var binding)) return;
                    if (binding.WasShadowedBy.Any())
                        diagnostics.Add(SemanticError.CantShadow(invocableDeclaration.File, binding.WasShadowedBy[^1].NameSpan, syn.Span));
                    return;
                }
                case IDeclarationSyntax _:
                    throw new InvalidOperationException($"Can't shadow check declaration of type {syntax.GetType().Name}");
                case ITypeSyntax _:
                    // ignore since they can't have shadowed variables
                    return;
            }

            WalkChildren(syntax, bindingScope);
        }
    }
}
