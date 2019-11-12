using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    public static class SymbolExtensions
    {
        public static T Assigned<T>(this T? symbol)
            where T : class, ISymbol
        {
            return symbol ?? throw new InvalidOperationException("Symbol not assigned");
        }
    }
}
