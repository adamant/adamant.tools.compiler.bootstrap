using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
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
            // TODO make a symbol for `Type`
            return new List<ISymbol>
            {
                new IntegerNumericSymbol(DataType.Int8),
                new IntegerNumericSymbol(DataType.Byte),
                new IntegerNumericSymbol(DataType.Int16),
                new IntegerNumericSymbol(DataType.UInt16),
                new IntegerNumericSymbol(DataType.Int),
                new IntegerNumericSymbol(DataType.UInt),
                new IntegerNumericSymbol(DataType.Int64),
                new IntegerNumericSymbol(DataType.UInt64),

                new IntegerNumericSymbol(DataType.Size),
                new IntegerNumericSymbol(DataType.Offset),
            }.ToFixedList();
        }
    }
}
