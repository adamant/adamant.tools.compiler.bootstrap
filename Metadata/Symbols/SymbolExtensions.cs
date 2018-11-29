using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public static class SymbolExtensions
    {
        public static bool IsGlobal([NotNull] this ISymbol symbol)
        {
            return symbol.FullName is SimpleName;
        }
    }
}
