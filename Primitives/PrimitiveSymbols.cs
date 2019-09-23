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
        public static readonly FixedList<ISymbol> Instance = BuildPrimitives();

        private static FixedList<ISymbol> BuildPrimitives()
        {
            var stringSymbol = BuildStringSymbol();
            var stringType = stringSymbol.DeclaresType;
            return new List<ISymbol>
            {
                stringSymbol,
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
            var concatFunc = Function.New(concatName);
            var literalConstructor = Function.New(typeName.Qualify(SpecialName.New), ("size", DataType.Size),
                ("value", DataType.StringConstant));
            var symbols = new List<ISymbol>()
            {
                literalConstructor,
                concatFunc,
            };

            var stringSymbol = Type.NewType(typeName, symbols);
            var stringType = UserObjectType.Declaration(stringSymbol, false);
            stringSymbol.DeclaresType = stringType;
            concatFunc.SetParameters(("other", stringType));
            concatFunc.ReturnType = stringType;
            literalConstructor.ReturnType = stringType;
            return stringSymbol;
        }

        private static ISymbol BuildPrintStringSymbol(DataType stringType)
        {
            var name = new SimpleName("print_string");
            return Function.New(name, ("s", stringType));
        }

        private static ISymbol BuildReadStringSymbol(DataType stringType)
        {
            var name = new SimpleName("read_string");
            return Function.New(name, stringType);
        }

        private static ISymbol BuildBoolSymbol()
        {
            return Type.NewSimpleType(DataType.Bool);
        }

        private static ISymbol BuildIntegerTypeSymbol(
            IntegerType integerType,
            DataType stringType)
        {
            var typeName = integerType.Name;
            var symbols = new List<ISymbol>
            {
                Function.New(typeName.Qualify("remainder"), integerType, ("divisor", integerType)),
                Function.New(typeName.Qualify("to_display_string"), stringType)
            };
            return Type.NewSimpleType(integerType, symbols);
        }
    }
}
