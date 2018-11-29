using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
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

        [NotNull]
        public override FixedList<ISymbol> Lookup([NotNull] SimpleName name)
        {
            var symbols = base.Lookup(name);
            return symbols.Any() ? symbols : ContainingScope.Lookup(name);
        }

        [NotNull]
        public override FixedList<ISymbol> LookupGlobal([NotNull] SimpleName name)
        {
            return ContainingScope.LookupGlobal(name);
        }
    }
}
