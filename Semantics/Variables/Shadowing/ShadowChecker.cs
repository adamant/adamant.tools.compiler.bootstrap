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
        private readonly IConcreteCallableDeclarationSyntax callableDeclaration;
        private readonly Diagnostics diagnostics;

        private ShadowChecker(IConcreteCallableDeclarationSyntax callableDeclaration, Diagnostics diagnostics)
        {
            this.callableDeclaration = callableDeclaration;
            this.diagnostics = diagnostics;
        }

        public static void Check(IEnumerable<ICallableDeclarationSyntax> callableDeclarations, Diagnostics diagnostics)
        {
            foreach (var callable in callableDeclarations.OfType<IConcreteCallableDeclarationSyntax>())
                new ShadowChecker(callable, diagnostics).Walk(callable, EmptyBindingScope.Instance);
        }

        protected override void WalkNonNull(ISyntax syntax, BindingScope bindingScope)
        {
            switch (syntax)
            {
                case IConcreteCallableDeclarationSyntax callable:
                    foreach (var parameter in callable.Parameters)
                        bindingScope = new VariableBindingScope(bindingScope, parameter);
                    break;
                case IBodyOrBlockSyntax bodyOrBlock:
                    foreach (var statement in bodyOrBlock.Statements)
                    {
                        WalkNonNull(statement, bindingScope);
                        // Each variable declaration establishes a new binding scope
                        if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                            bindingScope = new VariableBindingScope(bindingScope, variableDeclaration);
                    }
                    return;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                {
                    WalkChildren(variableDeclaration, bindingScope);
                    if (bindingScope.Lookup(variableDeclaration.FullName.UnqualifiedName, out var binding))
                    {
                        if (binding.MutableBinding)
                            diagnostics.Add(SemanticError.CantRebindMutableBinding(callableDeclaration.File,
                                variableDeclaration.NameSpan));
                        else if (variableDeclaration.IsMutableBinding)
                            diagnostics.Add(SemanticError.CantRebindAsMutableBinding(callableDeclaration.File,
                                variableDeclaration.NameSpan));
                    }
                    return;
                }
                case INameExpressionSyntax nameExpression:
                {
                    // This checks for cases where a variable was shadowed, but then used later
                    if (!bindingScope.Lookup(nameExpression.Name, out var binding)) return;
                    if (binding.WasShadowedBy.Any())
                        diagnostics.Add(SemanticError.CantShadow(callableDeclaration.File, binding.WasShadowedBy[^1].NameSpan, nameExpression.Span));
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
