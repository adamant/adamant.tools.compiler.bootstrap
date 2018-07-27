using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class NameScopeBuilder
    {
        private readonly Annotations annotations;

        internal NameScopeBuilder(Annotations annotations)
        {
            this.annotations = annotations;
        }

        internal void Build(CompilationUnitSyntax compilationUnit)
        {
            var globalNamespaceSymbol = annotations.Symbol(compilationUnit);
            var globalScope = new NameScope(globalNamespaceSymbol);
            annotations.Add(compilationUnit, globalScope);
            foreach (var declaration in compilationUnit.Declarations)
                Build(globalScope, declaration);
        }

        private void Build(NameScope scope, DeclarationSyntax declaration)
        {
            Match.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var symbol = annotations.Symbol(f);
                    var functionScope = new NameScope(scope, symbol);
                    annotations.Add(f, functionScope);
                }));
        }
    }
}
