using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
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
                BuildStringSymbol(),

                BuildIntegerNumericSymbol(DataType.Int8),
                BuildIntegerNumericSymbol(DataType.Byte),
                BuildIntegerNumericSymbol(DataType.Int16),
                BuildIntegerNumericSymbol(DataType.UInt16),
                BuildIntegerNumericSymbol(DataType.Int),
                BuildIntegerNumericSymbol(DataType.UInt),
                BuildIntegerNumericSymbol(DataType.Int64),
                BuildIntegerNumericSymbol(DataType.UInt64),

                BuildIntegerNumericSymbol(DataType.Size),
                BuildIntegerNumericSymbol(DataType.Offset),
            }.ToFixedList();
        }

        /// <summary>
        /// For now, we are putting string in the runtime library
        /// </summary>
        private static ISymbol BuildStringSymbol()
        {
            var name = new SimpleName("String");
            var symbol = new PrimitiveSymbol(name, null, Enumerable.Empty<ISymbol>());
            var objectType = new ObjectType(symbol, true, false);
            symbol.Type = new Metatype(objectType);
            return symbol;
        }

        private static ISymbol BuildIntegerNumericSymbol([NotNull] IntegerType numericType)
        {
            var typeName = numericType.Name;
            var symbols = new List<ISymbol>
            {
                new PrimitiveSymbol(typeName.Qualify("remainder"), new FunctionType(new[] {numericType}, numericType))
            };
            return new PrimitiveSymbol(typeName, new Metatype(numericType), symbols);
        }
    }
}
