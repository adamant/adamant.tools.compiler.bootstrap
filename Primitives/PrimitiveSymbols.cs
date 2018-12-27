using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
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
            // TODO make a symbol for `Type`
            var stringSymbol = BuildStringSymbol();
            var stringType = ((Metatype)stringSymbol.Type).Instance;
            return new List<ISymbol>
            {
                stringSymbol,
                BuildPrintStringSymbol(stringType),

                BuildBoolSymbol(),

                BuildIntegerTypeSymbol(DataType.Int8, stringType),
                BuildIntegerTypeSymbol(DataType.Byte, stringType),
                BuildIntegerTypeSymbol(DataType.Int16, stringType),
                BuildIntegerTypeSymbol(DataType.UInt16, stringType),
                BuildIntegerTypeSymbol(DataType.Int, stringType),
                BuildIntegerTypeSymbol(DataType.UInt, stringType),
                BuildIntegerTypeSymbol(DataType.Int64, stringType),
                BuildIntegerTypeSymbol(DataType.UInt64, stringType),

                BuildIntegerTypeSymbol(DataType.Size, stringType),
                BuildIntegerTypeSymbol(DataType.Offset, stringType),
            }.ToFixedList();
        }

        /// <summary>
        /// For now, we are putting string in the runtime library
        /// </summary>
        private static ISymbol BuildStringSymbol()
        {
            var typeName = new SimpleName("String");
            var stringLiteralOperator = Symbol.New(typeName.Qualify(SpecialName.OperatorStringLiteral));
            var symbols = new List<ISymbol>()
            {
                // Making these fields for now
                Symbol.New(typeName.Qualify("bytes"), DataType.BytePointer),
                Symbol.New(typeName.Qualify("byte_count"), DataType.Size),
                stringLiteralOperator
            };

            var stringSymbol = Symbol.NewType(typeName, symbols);
            var stringType = new ObjectType(stringSymbol, false, Lifetime.None);
            stringSymbol.Type = new Metatype(stringType);
            stringLiteralOperator.Type = new FunctionType(new DataType[] { DataType.Size, DataType.BytePointer }, stringType);
            return stringSymbol;
        }

        private static ISymbol BuildPrintStringSymbol(DataType stringType)
        {
            var name = new SimpleName("print_string");
            var type = new FunctionType(new[] { stringType }, DataType.Void);
            return Symbol.NewFunction(name, type);
        }

        private static ISymbol BuildBoolSymbol()
        {
            var typeName = DataType.Bool.Name;
            var symbols = new List<ISymbol> { };
            return Symbol.NewSimpleType(DataType.Bool, symbols);
        }

        private static ISymbol BuildIntegerTypeSymbol(
            IntegerType integerType,
            DataType stringType)
        {
            var typeName = integerType.Name;
            var symbols = new List<ISymbol>
            {
                Symbol.NewFunction(typeName.Qualify("remainder"), new FunctionType(new[] {integerType}, integerType)),
                Symbol.NewFunction(typeName.Qualify("to_display_string"), new FunctionType(Enumerable.Empty<DataType>(), stringType))
            };
            return Symbol.NewSimpleType(integerType, symbols);
        }
    }
}
