using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    /// <summary>
    /// Provides a declaration of the primitives for use when name binding etc.
    /// </summary>
    public static class PrimitiveSymbols
    {
        public static readonly FixedList<ISymbol> Instance = DefinePrimitiveSymbols();

        private static FixedList<ISymbol> DefinePrimitiveSymbols()
        {
            var stringType = ObjectType.Declaration(Name.From("String"), false);
            return new List<ISymbol>
            {
                // Simple Types
                BuildBoolSymbol(),

                BuildIntegerTypeSymbol(DataType.Byte, stringType),
                BuildIntegerTypeSymbol(DataType.Int, stringType),
                BuildIntegerTypeSymbol(DataType.UInt, stringType),

                BuildIntegerTypeSymbol(DataType.Size, stringType),
                BuildIntegerTypeSymbol(DataType.Offset, stringType),

                PrimitiveTypeSymbol.NewEmptyType(DataType.Void),
                PrimitiveTypeSymbol.NewEmptyType(DataType.Never),

                // Intrinsic functions used by the standard library
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_allocate"),
                    DataType.Size, ("length", DataType.Size)),

                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_deallocate"),
                    ("ptr", DataType.Size)),

                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_copy"),
                    ("from_ptr", DataType.Size), ("to_ptr", DataType.Size), ("length", DataType.Size)),

                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_set_byte"),
                    ("ptr", DataType.Size), ("value", DataType.Byte)),

                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_get_byte"),
                    DataType.Byte, ("ptr", DataType.Size)),

                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "print_utf8"),
                    ("ptr", DataType.Size), ("length", DataType.Size)),

                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "read_utf8_line"),
                    DataType.Size, ("ptr", DataType.Size), ("length", DataType.Size)),
            }.ToFixedList();
        }

        private static ISymbol BuildBoolSymbol()
        {
            return PrimitiveTypeSymbol.NewSimpleType(DataType.Bool);
        }

        private static ISymbol BuildIntegerTypeSymbol(IntegerType integerType, DataType stringType)
        {
            var typeName = integerType.Name;
            var symbols = new List<ISymbol>
            {
                PrimitiveMethodSymbol.New( typeName.Qualify("remainder"), integerType, integerType, ("divisor", integerType)),
                PrimitiveMethodSymbol.New(typeName.Qualify("to_display_string"), stringType, integerType),
            };
            return PrimitiveTypeSymbol.NewSimpleType(integerType, symbols);
        }
    }
}
