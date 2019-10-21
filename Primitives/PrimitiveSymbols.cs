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
                BuildPrintStringSymbol(stringType),
                BuildReadStringSymbol(stringType),

                BuildBoolSymbol(),

                BuildIntegerTypeSymbol(DataType.Byte, stringType),
                BuildIntegerTypeSymbol(DataType.Int, stringType),
                BuildIntegerTypeSymbol(DataType.UInt, stringType),

                BuildIntegerTypeSymbol(DataType.Size, stringType),
                BuildIntegerTypeSymbol(DataType.Offset, stringType),
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

        private static ISymbol BuildPrintStringSymbol(DataType stringType)
        {
            var name = new SimpleName("print_string");
            return PrimitiveFunctionSymbol.New(name, ("s", stringType));
        }

        private static ISymbol BuildReadStringSymbol(DataType stringType)
        {
            var name = new SimpleName("read_string");
            return PrimitiveFunctionSymbol.New(name, stringType);
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
