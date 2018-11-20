using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public static class SymbolExtensions
    {
        public static bool IsGlobal([NotNull] this ISymbol symbol)
        {
            return symbol.Name is SimpleName;
        }
    }
}
