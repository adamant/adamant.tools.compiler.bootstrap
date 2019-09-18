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
            // TODO make a symbol for `Type`
            var stringSymbol = BuildStringSymbol();
            var stringType = stringSymbol.DeclaresType;
            return new List<ISymbol>
            {
                stringSymbol,
                BuildPrintStringSymbol(stringType),
                BuildReadStringSymbol(stringType),

                BuildBoolSymbol(),

                //BuildIntegerTypeSymbol(DataType.Int8, stringType),
                BuildIntegerTypeSymbol(DataType.Byte, stringType),
                //BuildIntegerTypeSymbol(DataType.Int16, stringType),
                //BuildIntegerTypeSymbol(DataType.UInt16, stringType),
                BuildIntegerTypeSymbol(DataType.Int, stringType),
                BuildIntegerTypeSymbol(DataType.UInt, stringType),
                //BuildIntegerTypeSymbol(DataType.Int64, stringType),
                //BuildIntegerTypeSymbol(DataType.UInt64, stringType),

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
            var stringLiteralOperator = PrimitiveSymbol.New(typeName.Qualify(SpecialName.OperatorStringLiteral));
            var equalsOperator = PrimitiveSymbol.New(typeName.Qualify(SpecialName.OperatorEquals));
            var concatFunc = PrimitiveSymbol.New(typeName.Qualify(Name.From("concat")));
            var symbols = new List<ISymbol>()
            {
                // Making these fields for now
                PrimitiveSymbol.New(typeName.Qualify("bytes"), DataType.BytePointer),
                PrimitiveSymbol.New(typeName.Qualify("byte_count"), DataType.Size),
                stringLiteralOperator,
                equalsOperator,
                concatFunc,
            };

            var stringSymbol = PrimitiveSymbol.NewType(typeName, symbols);
            var stringType = UserObjectType.Declaration(stringSymbol, false);
            stringSymbol.DeclaresType = stringType;
            //stringLiteralOperator.Type = new FunctionType(new DataType[] { DataType.Size, DataType.BytePointer }, stringType);
            //equalsOperator.Type = new FunctionType(new[] { stringType, stringType }, DataType.Bool);
            //concatFunc.Type = new FunctionType(new[] { stringType, stringType }, stringType);
            return stringSymbol;
        }

        private static ISymbol BuildPrintStringSymbol(DataType stringType)
        {
            var name = new SimpleName("print_string");
            //var type = new FunctionType(new[] { stringType }, DataType.Void);
            return PrimitiveSymbol.NewFunction(name/*, type*/);
        }

        private static ISymbol BuildReadStringSymbol(DataType stringType)
        {
            var name = new SimpleName("read_string");
            //var type = new FunctionType(Enumerable.Empty<DataType>(), stringType);
            return PrimitiveSymbol.NewFunction(name/*, type*/);
        }

        private static ISymbol BuildBoolSymbol()
        {
            return PrimitiveSymbol.NewSimpleType(DataType.Bool);
        }

        private static ISymbol BuildIntegerTypeSymbol(
            IntegerType integerType,
            DataType stringType)
        {
            var typeName = integerType.Name;
            var symbols = new List<ISymbol>
            {
                PrimitiveSymbol.NewFunction(typeName.Qualify("remainder")/*, new FunctionType(new[] {integerType}, integerType)*/),
                PrimitiveSymbol.NewFunction(typeName.Qualify("to_display_string")/*, new FunctionType(Enumerable.Empty<DataType>(), stringType)*/)
            };
            return PrimitiveSymbol.NewSimpleType(integerType, symbols);
        }
    }
}
