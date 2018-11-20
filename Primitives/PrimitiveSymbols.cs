using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    /// <summary>
    /// Provides a declaration of the primitives for use when name binding etc.
    /// </summary>
    public static class PrimitiveSymbols
    {
        [NotNull] public static readonly FixedList<ISymbol> Instance = BuildPrimitives();

        [NotNull]
        private static FixedList<ISymbol> BuildPrimitives()
        {
            return new List<ISymbol>
            {
                new FixedIntegerSymbol(PrimitiveFixedIntegerType.Byte),
                new FixedIntegerSymbol(PrimitiveFixedIntegerType.Int),
                new FixedIntegerSymbol(PrimitiveFixedIntegerType.UInt)
            }.ToFixedList();
        }
    }
}
