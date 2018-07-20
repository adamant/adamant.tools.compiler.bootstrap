using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Binders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze
{
    public class DeclarationBinder
    {
        public void BindDeclarations(PackageSyntaxSymbol packageSymbol, SyntaxAnnotation<Type> typeAnnotations)
        {
            var globalScope = new NameScope(packageSymbol.GlobalNamespace);
            foreach (var tree in packageSymbol.Declaration.SyntaxTrees)
                BindDeclarations(packageSymbol.GlobalNamespace, tree.CompilationUnit, globalScope, typeAnnotations);
        }

        private void BindDeclarations(ISyntaxSymbol parentSymbol, SyntaxBranchNode syntax, NameScope scope, SyntaxAnnotation<Type> typeAnnotations)
        {
            Requires.NotNull(nameof(parentSymbol), parentSymbol);
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(typeAnnotations), typeAnnotations);

            Match.On(syntax).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var parameterSymbols = BindParameters(f, scope, typeAnnotations);
                    //f.ReturnType
                })
            );
        }

        private object BindParameters(FunctionDeclarationSyntax f, NameScope scope, SyntaxAnnotation<Type> typeAnnotations)
        {
            throw new NotImplementedException();
        }
    }
}
