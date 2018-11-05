using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public abstract class NestedScope : LexicalScope
    {
        [NotNull] public LexicalScope ContainingScope { get; }

        protected NestedScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] SyntaxNode syntax)
            : base(syntax)
        {
            Requires.NotNull(nameof(containingScope), containingScope);
            ContainingScope = containingScope;
            containingScope.Add(this);
        }

        public override ISymbol Lookup(string name)
        {
            return base.Lookup(name) ?? ContainingScope.Lookup(name);
        }
    }
}
