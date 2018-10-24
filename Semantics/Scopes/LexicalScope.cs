using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public abstract class LexicalScope
    {
        [NotNull] public SyntaxNode Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<LexicalScope> NestedScopes { get; }
        [NotNull] [ItemNotNull] private readonly List<LexicalScope> nestedScopes = new List<LexicalScope>();
        [CanBeNull] private IReadOnlyDictionary<string, IDeclarationAnalysis> declarations;

        protected LexicalScope([NotNull] SyntaxNode syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            NestedScopes = nestedScopes.AsReadOnly().AssertNotNull();
        }

        internal void Add([NotNull] NestedScope nestedScope)
        {
            Requires.That(nameof(nestedScope), nestedScope.ContainingScope == this);
            nestedScopes.Add(nestedScope);
        }

        public void Bind([NotNull] Dictionary<string, IDeclarationAnalysis> declarations)
        {
            Requires.NotNull(nameof(declarations), declarations);
            this.declarations = new Dictionary<string, IDeclarationAnalysis>(declarations).AsReadOnly();
        }

        [CanBeNull]
        public virtual IDeclarationAnalysis Lookup([NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            return declarations.AssertNotNull().TryGetValue(name, out var declaration) ? declaration : null;
        }
    }
}
