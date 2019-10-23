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
        public static readonly FixedList<ISymbol> Instance;
        public static readonly ITypeSymbol StringSymbol;

        static PrimitiveSymbols()
        {
            StringSymbol = BuildStringSymbol();
            var stringType = StringSymbol.DeclaresType;
            Instance = new List<ISymbol>
            {
                StringSymbol,
                PrimitiveFunctionSymbol.New(Name.From("print_string"), ("s", stringType)),
                PrimitiveFunctionSymbol.New(Name.From("read_string"), stringType),

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
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_allocate"), DataType.Size, ("length", DataType.Size)),
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_deallocate"), ("ptr", DataType.Size)),
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_copy"),
                    ("from_ptr", DataType.Size), ("to_ptr", DataType.Size), ("length", DataType.Size)),
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_set_byte"), ("ptr", DataType.Size), ("value", DataType.Byte)),
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "mem_get_byte"), DataType.Byte, ("ptr", DataType.Size)),
                PrimitiveFunctionSymbol.New(Name.From("intrinsics", "print_utf8_bytes"), ("ptr", DataType.Size), ("length", DataType.Size)),
            }.ToFixedList();
        }

        /// <summary>
        /// For now, we are putting string in the runtime library
        /// </summary>
        private static ITypeSymbol BuildStringSymbol()
        {
            var typeName = Name.From("String");
            var concatName = typeName.Qualify("concat");
            var concatFunc = PrimitiveFunctionSymbol.New(concatName);
            var symbols = new List<ISymbol>()
            {
                concatFunc,
            };

            var stringSymbol = PrimitiveTypeSymbol.NewType(typeName, symbols);
            var stringType = UserObjectType.Declaration(stringSymbol, false);
            stringSymbol.DeclaresType = stringType;
            concatFunc.SetParameters(("self", stringType), ("other", stringType));
            concatFunc.ReturnType = stringType;
            return stringSymbol;
        }

        private static ISymbol BuildBoolSymbol()
        {
            return PrimitiveTypeSymbol.NewSimpleType(DataType.Bool);
        }

        private static ISymbol BuildIntegerTypeSymbol(
            IntegerType integerType,
            DataType stringType)
        {
            var typeName = integerType.Name;
            var symbols = new List<ISymbol>
            {
                PrimitiveFunctionSymbol.New(typeName.Qualify("remainder"), integerType, ("self",integerType), ("divisor", integerType)),
                PrimitiveFunctionSymbol.New(typeName.Qualify("to_display_string"), stringType, ("self", integerType)),
            };
            return PrimitiveTypeSymbol.NewSimpleType(integerType, symbols);
        }
    }
}
