using System.Collections.Generic;
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
                BuildBoolSymbol(),

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

        private static ISymbol BuildBoolSymbol()
        {
            var typeName = DataType.Bool.Name;
            var symbols = new List<ISymbol> { };
            return Symbol.NewSimpleType(DataType.Bool, symbols);
        }

        /// <summary>
        /// For now, we are putting string in the runtime library
        /// </summary>
        private static ISymbol BuildStringSymbol()
        {
            var typeName = new SimpleName("String");
            var stringLiteralOperator = Symbol.NewMember(typeName.Qualify(SpecialName.OperatorStringLiteral));
            var symbols = new List<ISymbol>()
            {
                // Making these fields for now
                Symbol.NewMember(typeName.Qualify("bytes"), DataType.BytePointer),
                Symbol.NewMember(typeName.Qualify("byte_count"), DataType.Size),
                stringLiteralOperator
            };

            var stringSymbol = Symbol.NewType(typeName, symbols);
            var stringType = new ObjectType(stringSymbol, false);
            stringSymbol.Type = new Metatype(stringType);
            stringLiteralOperator.Type = new FunctionType(new DataType[] { DataType.Size, DataType.BytePointer }, stringType);
            return stringSymbol;
        }

        private static ISymbol BuildIntegerNumericSymbol([NotNull] IntegerType numericType)
        {
            var typeName = numericType.Name;
            var symbols = new List<ISymbol>
            {
                Symbol.NewMember(typeName.Qualify("remainder"), new FunctionType(new[] {numericType}, numericType))
            };
            return Symbol.NewSimpleType(numericType, symbols);
        }
    }
}
