using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class GlobalScope : LexicalScope
    {
        public GlobalScope([NotNull, ItemNotNull] IEnumerable<ISymbol> symbols)
            : base(symbols)
        {
        }

        [CanBeNull]
        public override ISymbol LookupGlobal([NotNull] SimpleName name)
        {
            return Lookup(name);
        }
    }
}
