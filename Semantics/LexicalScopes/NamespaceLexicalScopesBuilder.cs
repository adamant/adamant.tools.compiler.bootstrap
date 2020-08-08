using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class NamespaceLexicalScopesBuilder : SyntaxWalker<LexicalScope<Promise<Symbol?>>>
    {
        private readonly Diagnostics diagnostics;
        private readonly FixedDictionary<NamespaceName, Namespace> namespaces;

        public NamespaceLexicalScopesBuilder(
            Diagnostics diagnostics,
            FixedDictionary<NamespaceName, Namespace> namespaces)
        {
            this.diagnostics = diagnostics;
            this.namespaces = namespaces;
        }

        protected override void WalkNonNull(ISyntax syntax, LexicalScope<Promise<Symbol?>> containingScope)
        {
            switch (syntax)
            {
                case ICompilationUnitSyntax syn:
                    containingScope = BuildNamespaceScopes(syn.ImplicitNamespaceName, containingScope);
                    //containingScope = BuildUsingDirectivesScope(compilationUnit.UsingDirectives, containingScope);
                    WalkChildren(syn, containingScope);
                    break;
                case INamespaceDeclarationSyntax syn:
                    WalkChildren(syn, containingScope);
                    break;
                default:
                    // Ignore
                    return;
            }
        }

        private LexicalScope<Promise<Symbol?>> BuildNamespaceScopes(NamespaceName nsName, LexicalScope<Promise<Symbol?>> containingScope)
        {
            foreach (var name in nsName.NamespaceNames())
                containingScope = BuildNamespaceScope(name, containingScope);

            return containingScope;
        }

        private LexicalScope<Promise<Symbol?>> BuildNamespaceScope(NamespaceName nsName, LexicalScope<Promise<Symbol?>> containingScope)
        {
            var keys = namespaces.Keys.ToList();
            var ns = namespaces[nsName];
            return new NestedScope<Promise<Symbol?>>(containingScope, ns.Symbols, ns.NestedSymbols);
        }
    }
}
