using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class NestedScope : LexicalScope
    {
        [NotNull] public LexicalScope ContainingScope { get; }

        public NestedScope(
            [NotNull] LexicalScope containingScope,
            [NotNull, ItemNotNull] IEnumerable<ISymbol> symbols)
            : base(symbols)
        {
            ContainingScope = containingScope;
        }

        [CanBeNull]
        public override ISymbol Lookup([NotNull] SimpleName name)
        {
            return base.Lookup(name) ?? ContainingScope.Lookup(name);
        }

        [CanBeNull]
        public override ISymbol LookupGlobal([NotNull] SimpleName name)
        {
            return ContainingScope.LookupGlobal(name);
        }
    }
}
