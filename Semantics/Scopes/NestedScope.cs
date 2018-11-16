using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public abstract class NestedScope : LexicalScope
    {
        [NotNull] public LexicalScope ContainingScope { get; }

        protected NestedScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] NonTerminal syntax)
            : base(syntax)
        {
            Requires.NotNull(nameof(containingScope), containingScope);
            ContainingScope = containingScope;
            containingScope.Add(this);
        }

        [CanBeNull]
        public override ISymbol Lookup([NotNull] SimpleName name)
        {
            return base.Lookup(name) ?? ContainingScope.Lookup(name);
        }

        [CanBeNull]
        public override ISymbol LookupGlobal([NotNull] Name name)
        {
            return ContainingScope.LookupGlobal(name);
        }
    }
}
