using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public abstract class LexicalScope
    {
        [NotNull] public Syntax Syntax { get; }
        [NotNull, ItemNotNull] public IReadOnlyList<LexicalScope> NestedScopes { get; }
        [NotNull, ItemNotNull] private readonly List<LexicalScope> nestedScopes = new List<LexicalScope>();
        [CanBeNull] private FixedDictionary<SimpleName, ISymbol> declarations;

        protected LexicalScope([NotNull] Syntax syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            NestedScopes = nestedScopes.AsReadOnly().NotNull();
        }

        internal void Add([NotNull] NestedScope nestedScope)
        {
            Requires.That(nameof(nestedScope), nestedScope.ContainingScope == this);
            nestedScopes.Add(nestedScope);
        }

        public void Bind([NotNull] Dictionary<SimpleName, ISymbol> symbolDeclarations)
        {
            declarations = symbolDeclarations.ToFixedDictionary();
        }

        [CanBeNull]
        public virtual ISymbol Lookup([NotNull] SimpleName name)
        {
            return declarations.NotNull().TryGetValue(name, out var declaration) ? declaration : null;
        }

        [CanBeNull]
        public abstract ISymbol LookupGlobal([NotNull] Name name);
    }
}
