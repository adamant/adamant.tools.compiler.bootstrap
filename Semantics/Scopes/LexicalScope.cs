using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public abstract class LexicalScope
    {
        [NotNull] public SyntaxNode Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<LexicalScope> NestedScopes { get; }
        [NotNull] [ItemNotNull] private readonly List<LexicalScope> nestedScopes = new List<LexicalScope>();

        protected LexicalScope([NotNull] SyntaxNode syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            NestedScopes = nestedScopes.AsReadOnly().AssertNotNull();
        }

        internal void Add([NotNull] DeclarationScope nestedScope)
        {
            Requires.That(nameof(nestedScope), nestedScope.ContainingScope == this);
            nestedScopes.Add(nestedScope);
        }

        [CanBeNull]
        public abstract DeclarationAnalysis Lookup([NotNull] string name);
    }
}
